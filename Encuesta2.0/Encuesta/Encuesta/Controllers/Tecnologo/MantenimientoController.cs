using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Tecnologo
{
         [Authorize(Roles = "3")]
    public class MantenimientoController : Controller
    {
        //
        // GET: /Mantenimiento/
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();
         #region  Incidente

         // GET: Mantenimiento
         public ActionResult RegistrarIncidente()
         {
             List<EquipoMantenimiento> lstEquipoMantenimiento = new List<EquipoMantenimiento>();
             List<Object> lstTipoIncidente = new List<Object>();
             string queryString = @"select u.nombre empresa,e.codigoequipo,e.codigounidad2 unidad,e.codigosucursal2 sede,u.NombreSmall+'-'+s.ShortDesc sucursal,e.nombreequipo,e.ShortDesc,e.codigomodalidad2,m.abreviatura from EQUIPO e
inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad 
inner join SUCURSAL s on e.codigounidad2=s.codigounidad and e.codigosucursal2=s.codigosucursal
inner join MODALIDAD m on e.codigomodalidad2=m.codigomodalidad and e.codigounidad2=m.codigounidad
where e.iseliminado=0
order by e.codigounidad2,e.codigosucursal2;

select idTipoIndicente,Nombre from TipoIncidente where isActivo=1;";
             using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
             {

                 SqlCommand command = new SqlCommand(queryString, connection);
                 connection.Open();
                 SqlDataReader reader = command.ExecuteReader();
                 try
                 {
                     if (reader.HasRows)
                     {
                         while (reader.Read())
                         {
                             EquipoMantenimiento item = new EquipoMantenimiento();
                             item.idEquipo = Convert.ToInt32(reader["codigoequipo"].ToString());
                             item.idEmpresa = Convert.ToInt32(reader["unidad"].ToString());
                             item.empresa = (reader["empresa"].ToString());
                             item.idSede = Convert.ToInt32(reader["sede"].ToString());
                             item.sucursal = (reader["sucursal"].ToString());
                             item.nombreEquipo = (reader["nombreequipo"].ToString());
                             item.shortNombreequipo = (reader["ShortDesc"].ToString());
                             item.idModalidad = Convert.ToInt32(reader["codigomodalidad2"].ToString());
                             item.modalidad = (reader["abreviatura"].ToString());
                             lstEquipoMantenimiento.Add(item);
                         }
                         if (reader.NextResult())
                             while (reader.Read())
                             {
                                 lstTipoIncidente.Add(new { codigo = Convert.ToInt32(reader["idTipoIndicente"].ToString()), nombre = (reader["Nombre"].ToString()) });
                             }
                     }

                 }
                 finally
                 {
                     // Always call Close when done reading.
                     reader.Close();
                     connection.Close();

                 }
             }
             ViewBag.TipoIncidente = new SelectList(lstTipoIncidente, "codigo", "nombre");
             ViewBag.Sedes = new SelectList(lstEquipoMantenimiento.Select(x => new { codigo = x.idEmpresa * 100 + x.idSede, nombre =  x.empresa+" - "+x.sucursal }).Distinct().OrderBy(x => x.codigo).ToList(), "codigo", "nombre");
             ViewBag.Equipo = lstEquipoMantenimiento.ToArray();
             ViewBag.Vacio = new SelectList(new Variable().getVacio(), "id", "nombre");
             var mantenimiento = new MantenimientoViewModel();
             mantenimiento.listaRevisiones = new List<DetalleMantenimiento>();
             mantenimiento.listaRevisiones.Add(new DetalleMantenimiento());
             return View(mantenimiento);

         }
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult RegistrarIncidente(MantenimientoViewModel mante)
         {
             try
             {
                 CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                 IncidenteEquipo inci = new IncidenteEquipo();
                 inci.idEquipo = mante.idEquipo;
                 inci.idTipoIncidente = mante.idTipoIncidente;
                 inci.user_detecta = user.ProviderUserKey.ToString();
                 inci.fecha_detecta =  Variable.getDatetime();
                 inci.falla_detecta = mante.falla_detectada;
                 inci.isSolucionado = mante.isSolucionado;
                 inci.telefono = mante.telefono;
                 if (inci.isSolucionado)
                 {
                     inci.fecha_solucionado = Variable.getDatetime();
                     RevisionIncidente revi = new RevisionIncidente();
                     revi.idIncidente = inci.idIncidente;
                     revi.fecha_revision = Variable.getDatetime();
                     revi.user_revision = user.ProviderUserKey.ToString();
                     revi.revision = mante.solucion;
                     revi.isCambioPiezas = false;
                     db.RevisionIncidente.Add(revi);
                 }
                 db.IncidenteEquipo.Add(inci);
                 db.SaveChanges();
                 var fecha = Variable.getDatetime();
                 var equipo = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == mante.idEquipo);
                 if (equipo != null)
                 {

                     string cuerpoCorreo = "Se registro un incidente a las " + fecha.ToShortTimeString() + " del " + fecha.ToShortDateString() +
                         @":<br/>
<b>Datos del Equipo:</b>
<ul>
<li><b>Marca: </b>" + equipo.marca + @"</li>
<li><b>Modelo: </b>" + equipo.modelo + @"</li>
<li><b>Serie: </b>" + equipo.serienumber + @"</li>
</ul>
<br/>
<b>Datos del Usuario:</b>
<ul>
<li><b>Usuario: </b>" + user.UserName.ToString() + @"</li>
<li><b>Teléfono: </b>" + mante.telefono + @"</li>
</ul>
<br/>
<b>Incidente Reportado</b>
<p><i>" + mante.falla_detectada + @"</i></p>";
                     if (inci.isSolucionado)
                     {
                         cuerpoCorreo += @"<b>Solución al Incidente Reportado</b>
<p><i>" + mante.solucion + @"</i></p>";
                     }
                     var empresa = db.EmpresaServicio.SingleOrDefault(x => x.idEmpresa == equipo.idEmpresa);

                     string destinatarios = "mantenimiento@resocentro.com;";
                     if (empresa != null && mante.idTipoIncidente != 2)
                         destinatarios += (empresa.correoSoporte) + "";

                     cuerpoCorreo += "<br/><br/><small style=\"color:red;\"><i>Puede acceder mediante este </i><a href=\"http://extranet.resocentro.com:5024/Mantenimiento/DetalleIncidente?Incidente=" + inci.idIncidente + "\">Link</a></small>";
                     new Variable().sendCorreo("Incidente Reportado N°" + inci.idIncidente + " Equipo: " + equipo.nombreequipo + " S/N: " + equipo.serienumber, destinatarios, "", cuerpoCorreo, "");

                 }



                 return RedirectToAction("UploadFileIncidente", new { incidente = inci.idIncidente, revision = 0 });
             }
             catch (Exception ex)
             {
                 ex = ex;
                 List<EquipoMantenimiento> lstEquipoMantenimiento = new List<EquipoMantenimiento>();
                 List<Object> lstTipoIncidente = new List<Object>();
                 string queryString = @"select u.nombre empresa,e.codigoequipo,e.codigounidad2 unidad,e.codigosucursal2 sede,u.NombreSmall+'-'+s.ShortDesc sucursal,e.nombreequipo,e.ShortDesc,e.codigomodalidad2,m.abreviatura from EQUIPO e
inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad 
inner join SUCURSAL s on e.codigounidad2=s.codigounidad and e.codigosucursal2=s.codigosucursal
inner join MODALIDAD m on e.codigomodalidad2=m.codigomodalidad and e.codigounidad2=m.codigounidad
where e.iseliminado=0
order by e.codigounidad2,e.codigosucursal2;

select idTipoIndicente,Nombre from TipoIncidente where isActivo=1;";
                 using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                 {

                     SqlCommand command = new SqlCommand(queryString, connection);
                     connection.Open();
                     SqlDataReader reader = command.ExecuteReader();
                     try
                     {
                         if (reader.HasRows)
                         {
                             while (reader.Read())
                             {
                                 EquipoMantenimiento item = new EquipoMantenimiento();
                                 item.idEquipo = Convert.ToInt32(reader["codigoequipo"].ToString());
                                 item.idEmpresa = Convert.ToInt32(reader["unidad"].ToString());
                                 item.empresa = (reader["empresa"].ToString());
                                 item.idSede = Convert.ToInt32(reader["sede"].ToString());
                                 item.sucursal = (reader["sucursal"].ToString());
                                 item.nombreEquipo = (reader["nombreequipo"].ToString());
                                 item.shortNombreequipo = (reader["ShortDesc"].ToString());
                                 item.idModalidad = Convert.ToInt32(reader["codigomodalidad2"].ToString());
                                 item.modalidad = (reader["abreviatura"].ToString());
                                 lstEquipoMantenimiento.Add(item);
                             }
                             if (reader.NextResult())
                                 while (reader.Read())
                                 {
                                     lstTipoIncidente.Add(new { codigo = Convert.ToInt32(reader["idTipoIndicente"].ToString()), nombre = (reader["Nombre"].ToString()) });
                                 }
                         }

                     }
                     finally
                     {
                         // Always call Close when done reading.
                         reader.Close();
                         connection.Close();

                     }
                 }
                 ViewBag.TipoIncidente = new SelectList(lstTipoIncidente, "codigo", "nombre");
                 ViewBag.Sedes = new SelectList(lstEquipoMantenimiento.Select(x => new { codigo = x.idEmpresa * 100 + x.idSede, nombre = x.empresa + " - " + x.sucursal }).Distinct().OrderBy(x => x.codigo).ToList(), "codigo", "nombre");
                 ViewBag.Equipo = lstEquipoMantenimiento.ToArray();
                 ViewBag.Vacio = new SelectList(new Variable().getVacio(), "id", "nombre");
                 return View(new MantenimientoViewModel());
             }



         }
         public ActionResult UploadFileIncidente(int incidente, int revision = 0)
         {
             ViewBag.Incidente = incidente;
             ViewBag.Revision = revision;
             return View();
         }
         [HttpPost]
         public ActionResult UploadFileMantenimiento(int incidente, int revision)
         {

             CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
             string _ruta = Variable.PathAdjuntosMantenimiento + incidente.ToString();
             if (revision > 0)
                 _ruta += "\\Revision_" + revision;
             if (!Directory.Exists(_ruta))
                 Directory.CreateDirectory(_ruta);
             var r = new List<UploadFilesResult>();
             foreach (string file in Request.Files)
             {
                 HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                 string pathFile = _ruta + "\\" + Path.GetFileName(hpf.FileName);
                 if (hpf.ContentLength == 0)
                     continue;

                 hpf.SaveAs(pathFile);
                 AdjuntoIncidente adj = new AdjuntoIncidente();
                 adj.idIncidente = incidente;
                 if (revision > 0)
                     adj.idRevision = revision;
                 adj.pathFile = pathFile;
                 adj.fecha_upload = Variable.getDatetime();
                 adj.usuario = user.ProviderUserKey.ToString();
                 db.AdjuntoIncidente.Add(adj);
                 db.SaveChanges();

                 r.Add(new UploadFilesResult()
                 {
                     Name = hpf.FileName,
                     Length = hpf.ContentLength,
                     Type = hpf.ContentType
                 });
             }
             // Returns json
             return Content("{\"name\":\"" + r[0].Name + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");
         }
         public ActionResult ListFileMantenimiento(int incidente)
         {
             ViewBag.Incidente = incidente;
             return View();
         }
         public ActionResult ListFileIncidente(int incidente)
         {
             string filePath = Variable.PathAdjuntosMantenimiento + incidente.ToString();
             string result = "";
             string path = filePath + @"\" + Server.UrlDecode(Request.Params["dir"]);
             System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
             if (di.Exists)
             {
                 DirectoryInfo[] directories = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                 result += ("<ul class=\"jqueryFileTree\" >\n");
                 //Itera sobre os subdiretórios de cada driver
                 foreach (System.IO.DirectoryInfo di_child in directories)
                 {
                     result += ("\t<li class=\"directory collapsed\"><a href=\"#\" rel=\"" + di_child.Name + "/\">" + di_child.Name + "</a>\n");
                 }
                 foreach (System.IO.FileInfo fi in di.GetFiles())
                 {
                     string ext = "";
                     if (fi.Extension.Length > 1)
                     {
                         ext = fi.Extension.Substring(1).ToLower();
                     }

                     result += ("\t<li class=\"file ext_" + ext + "\"><a href=\"#\" rel=\"" + fi.FullName.Replace("\\\\serverweb", "http://extranet.resocentro.com:5050") + "\">" + fi.Name + "</a></li>\n");
                 }// Arquivos 
                 result += ("</ul></li>");
             }

             if (result == "")
                 result = "<b>No hay archivos que mostrar</b>";
             return Json(result); ;
         }
         public ActionResult ListaIncidente(string tipo)
         {
             List<MantenimientoViewModel> lstIncidente = new List<MantenimientoViewModel>();
             CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
             string codigousuario = user.ProviderUserKey.ToString();
             EmpresaServicio empresa = null;// db.EmpresaServicio.SingleOrDefault(x => x.codigousuario == codigousuario);
             List<Object> lstTipoIncidente = new List<Object>();
             string queryString = @"select u.NombreSmall empresa,su.ShortDesc sucursal,ie.idIncidente,ie.idEquipo,e.nombreequipo,
	mo.abreviatura modalidad,e.marca,e.modelo,
	e.idEmpresa,es.razonsocial nombreempresa,ie.idTipoIncidente
	,ti.Nombre tipoincidente,ie.fecha_detecta 
	from IncidenteEquipo ie
	inner join EQUIPO e on ie.idEquipo=e.codigoequipo
	inner join EmpresaServicio es on e.idEmpresa=es.idEmpresa
	inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad
	inner join SUCURSAL su on e.codigounidad2=su.codigounidad and e.codigosucursal2=su.codigosucursal
	inner join MODALIDAD mo on e.codigomodalidad2=mo.codigomodalidad and e.codigounidad2=mo.codigounidad
	inner join TipoIncidente ti on ie.idTipoIncidente=ti.idTipoIndicente
	where isSolucionado=0 {0} order by ie.fecha_detecta desc; select idTipoIndicente,Nombre from TipoIncidente";
             if (empresa != null)
                 codigousuario = " and es.idEmpresa=" + empresa.idEmpresa.ToString();
             else
                 codigousuario = "";
             using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
             {

                 SqlCommand command = new SqlCommand(string.Format(queryString, codigousuario), connection);
                 connection.Open();
                 SqlDataReader reader = command.ExecuteReader();
                 try
                 {
                     if (reader.HasRows)
                     {
                         while (reader.Read())
                         {
                             MantenimientoViewModel item = new MantenimientoViewModel();
                             item.idIncidente = Convert.ToInt32(reader["idIncidente"].ToString());
                             item.idEquipo = Convert.ToInt32(reader["idEquipo"].ToString());
                             item.equipo = new EquipoMantenimiento();
                             item.equipo.idEquipo = item.idEquipo;
                             item.equipo.marca = (reader["marca"].ToString());
                             item.equipo.modelo = (reader["modelo"].ToString());
                             item.equipo.nombreEquipo = (reader["nombreequipo"].ToString());
                             item.equipo.modalidad = (reader["modalidad"].ToString());
                             item.equipo.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                             item.equipo.empresaMantenimiento = new EmpresaMantenimiento();
                             item.equipo.empresaMantenimiento.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                             item.equipo.empresa = (reader["empresa"].ToString());
                             item.equipo.sucursal = (reader["sucursal"].ToString());
                             item.equipo.empresaMantenimiento.razonsocial = (reader["nombreempresa"].ToString());
                             item.fecha_detecta = Convert.ToDateTime(reader["fecha_detecta"].ToString());
                             item.idTipoIncidente = Convert.ToInt32(reader["idTipoIncidente"].ToString());
                             item.tipoIncidente = (reader["tipoincidente"].ToString());
                             lstIncidente.Add(item);
                         }
                         if (reader.NextResult())
                             while (reader.Read())
                             {
                                 lstTipoIncidente.Add(new { codigo = Convert.ToInt32(reader["idTipoIndicente"].ToString()), nombre = (reader["Nombre"].ToString()) });
                             }
                     }

                 }
                 finally
                 {
                     // Always call Close when done reading.
                     reader.Close();
                     connection.Close();

                 }
             }
             return View(lstIncidente);
         }
         public ActionResult RegistrarRevision(int incidente)
         {
             MantenimientoViewModel item = new MantenimientoViewModel();
             List<Object> lstTipoIncidente = new List<Object>();
             string queryString = @"select u.NombreSmall empresa,su.ShortDesc sucursal,ie.idIncidente,ie.idEquipo,e.nombreequipo,ie.falla_detecta,e.marca,e.modelo,
	e.idEmpresa,es.razonsocial nombreempresa,ie.idTipoIncidente,us.ShortName
	,ti.Nombre tipoincidente,ie.fecha_detecta 
	from IncidenteEquipo ie
	inner join EQUIPO e on ie.idEquipo=e.codigoequipo
	inner join EmpresaServicio es on e.idEmpresa=es.idEmpresa
inner join USUARIO us on ie.user_detecta=us.codigousuario
	inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad
	inner join SUCURSAL su on e.codigounidad2=su.codigounidad and e.codigosucursal2=su.codigosucursal
	inner join TipoIncidente ti on ie.idTipoIncidente=ti.idTipoIndicente
	where ie.idIncidente=" + incidente.ToString() + "; select idRevision,revision, fecha_revision,isCambioPiezas,u.ShortName from RevisionIncidente ri inner join USUARIO u on ri.user_revision=u.codigousuario where idIncidente=" + incidente.ToString() + " order by fecha_revision";
             using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
             {

                 SqlCommand command = new SqlCommand(queryString, connection);
                 connection.Open();
                 SqlDataReader reader = command.ExecuteReader();
                 try
                 {
                     if (reader.HasRows)
                     {
                         while (reader.Read())
                         {
                             item.idIncidente = Convert.ToInt32(reader["idIncidente"].ToString());
                             item.idEquipo = Convert.ToInt32(reader["idEquipo"].ToString());
                             item.equipo = new EquipoMantenimiento();
                             item.equipo.idEquipo = item.idEquipo;
                             item.equipo.marca = (reader["marca"].ToString());
                             item.equipo.modelo = (reader["modelo"].ToString());
                             item.equipo.nombreEquipo = (reader["nombreequipo"].ToString());
                             item.equipo.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                             item.equipo.empresaMantenimiento = new EmpresaMantenimiento();
                             item.equipo.empresaMantenimiento.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                             item.equipo.empresa = (reader["empresa"].ToString());
                             item.equipo.sucursal = (reader["sucursal"].ToString());
                             item.equipo.empresaMantenimiento.razonsocial = (reader["nombreempresa"].ToString());
                             item.fecha_detecta = Convert.ToDateTime(reader["fecha_detecta"].ToString());
                             item.falla_detectada = (reader["falla_detecta"].ToString());
                             item.idTipoIncidente = Convert.ToInt32(reader["idTipoIncidente"].ToString());
                             item.tipoIncidente = (reader["tipoincidente"].ToString());
                             item.usuario_detecta = (reader["ShortName"].ToString());
                             item.listaRevisiones = new List<DetalleMantenimiento>();
                         }
                         if (reader.NextResult())
                             while (reader.Read())
                             {
                                 DetalleMantenimiento dm = new DetalleMantenimiento();
                                 dm.idIncidente = item.idIncidente;
                                 dm.idDetalleMantenimiento = Convert.ToInt32(reader["idRevision"].ToString());
                                 dm.fecha_revision = Convert.ToDateTime(reader["fecha_revision"].ToString());
                                 dm.revision = (reader["revision"].ToString());
                                 dm.isCambioPieza = Convert.ToBoolean(reader["isCambioPiezas"].ToString());
                                 dm.usuario_revisa = (reader["ShortName"].ToString());
                                 dm.listaPiezas = new List<PiezasMantenimiento>();
                                 item.listaRevisiones.Add(dm);
                                 if (dm.isCambioPieza)
                                 {
                                     dm.listaPiezas.AddRange(db.PiezasRevision.Where(x => x.idRevision == dm.idDetalleMantenimiento).Select(x => new PiezasMantenimiento { nombre = x.nombre, serie = x.serie }).ToList());
                                 }
                             }
                     }

                 }
                 finally
                 {
                     // Always call Close when done reading.
                     reader.Close();
                     connection.Close();

                 }
             }
             ViewBag.Mantenimiento = item;
             ViewBag.EstadoEquipo = new SelectList(new Variable().getEstadoEquipoMantenimiento(), "nombre", "nombre");

             DetalleMantenimiento revision = new DetalleMantenimiento();
             revision.idIncidente = item.idIncidente;

             return View(revision);
         }
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult RegistrarRevision(DetalleMantenimiento detallemantenimiento)
         {
             try
             {
                 var incidente = db.IncidenteEquipo.SingleOrDefault(x => x.idIncidente == detallemantenimiento.idIncidente);
                 RevisionIncidente rev = new RevisionIncidente();
                 if (incidente != null)
                 {
                     CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

                     rev.idIncidente = detallemantenimiento.idIncidente;
                     rev.fecha_revision = Variable.getDatetime();
                     rev.user_revision = user.ProviderUserKey.ToString();
                     rev.tecnico = detallemantenimiento.tecnico;
                     rev.revision = detallemantenimiento.revision;
                     rev.estadoequipo = detallemantenimiento.estadoequipo;
                     incidente.isSolucionado = detallemantenimiento.isSolucionado;
                     if (incidente.isSolucionado)
                     {
                         incidente.fecha_solucionado = Variable.getDatetime();
                     }

                     if (detallemantenimiento.piezas_array != "")
                         detallemantenimiento.listaPiezas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PiezasMantenimiento>>(detallemantenimiento.piezas_array);
                     else
                         detallemantenimiento.listaPiezas = new List<PiezasMantenimiento>();
                     rev.isCambioPiezas = detallemantenimiento.listaPiezas.Count > 0;
                     foreach (var item in detallemantenimiento.listaPiezas)
                     {
                         PiezasRevision pieza = new PiezasRevision();
                         pieza.idRevision = rev.idRevision;
                         pieza.nombre = item.nombre;
                         pieza.serie = item.serie;
                         db.PiezasRevision.Add(pieza);
                     }
                     db.RevisionIncidente.Add(rev);
                     db.SaveChanges();

                     var fecha = Variable.getDatetime();
                     var equipo = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == incidente.idEquipo);
                     if (equipo != null)
                     {
                         var usuario_detecta = db.USUARIO.SingleOrDefault(x => x.codigousuario == incidente.user_detecta);

                         string cuerpoCorreo = "Se reviso el Incidente N°" + incidente.idIncidente + " a las " + fecha.ToShortTimeString() + " del " + fecha.ToShortDateString() +
                             @":<br/>
<b>Datos del Equipo:</b>
<ul>
<li><b>Marca: </b>" + equipo.marca + @"</li>
<li><b>Modelo: </b>" + equipo.modelo + @"</li>
<li><b>Serie: </b>" + equipo.serienumber + @"</li>
</ul>
<br/>
<b>Datos del Usuario:</b>
<ul>
<li><b>Usuario: </b>" + usuario_detecta.ShortName + @"</li>
<li><b>Teléfono: </b>" + incidente.telefono + @"</li>
</ul>
<br/>
<b>Datos del Técnico/Ingeniero:</b>
<ul>
<li><b>Usuario: </b>" + detallemantenimiento.tecnico.ToString() + @"</li>
</ul>
<br/>
<b>Incidente Reportado" + (incidente.isSolucionado ? " (SOLUCIONADO)" : "") + @"</b>
<p><i>" + incidente.falla_detecta + @"</i></p>";
                         if (rev.isCambioPiezas)
                         {
                             cuerpoCorreo += "<table class=\"table compact table-striped  table-hover\"><thead><tr><th>Pieza</th><th>Serie</th></tr></thead><tbody id=\"tab_body_pieza\">";

                             foreach (var item in detallemantenimiento.listaPiezas)
                             {
                                 cuerpoCorreo += "<tr> <td>" + item.nombre + "</td><td>" + item.serie + "</td></tr>";
                             }
                             cuerpoCorreo += "</tbody></table>";
                         }

                         cuerpoCorreo += @"<br/>
<b>Revisión Realizada</b>
<p><i>" + detallemantenimiento.revision + @"</i></p>
<br/>
<p>El estado actual del Equipo es: <b>" + detallemantenimiento.estadoequipo.ToUpper() + @"</b></p>
<br/>
<br/>"
 + "<small style=\"color:red;\"><i>Puede acceder mediante este </i><a href=\"http://extranet.resocentro.com:5024/Mantenimiento/DetalleIncidente?Incidente=" + incidente.idIncidente + "\">Link</a></small>";
                         var empresa = db.EmpresaServicio.SingleOrDefault(x => x.idEmpresa == equipo.idEmpresa);

                         string destinatarios = "mantenimiento@resocentro.com;";
                         if (empresa != null && incidente.idTipoIncidente != 2)
                             destinatarios += (empresa.correoSoporte) + "";
                         new Variable().sendCorreo("Revisión de Incidente N°" + incidente.idIncidente + " Equipo: " + equipo.nombreequipo + " S/N: " + equipo.serienumber, destinatarios, "", cuerpoCorreo, "");
                     }
                 }



                 return RedirectToAction("UploadFileIncidente", new { incidente = detallemantenimiento.idIncidente, revision = rev.idRevision });
             }
             catch (Exception ex)
             {
                 MantenimientoViewModel item = new MantenimientoViewModel();
                 List<Object> lstTipoIncidente = new List<Object>();
                 string queryString = @"select u.NombreSmall empresa,su.ShortDesc sucursal,ie.idIncidente,ie.idEquipo,e.nombreequipo,ie.falla_detecta,e.marca,e.modelo,
	e.idEmpresa,es.razonsocial nombreempresa,ie.idTipoIncidente,us.ShortName
	,ti.Nombre tipoincidente,ie.fecha_detecta 
	from IncidenteEquipo ie
	inner join EQUIPO e on ie.idEquipo=e.codigoequipo
	inner join EmpresaServicio es on e.idEmpresa=es.idEmpresa
inner join USUARIO us on ie.user_detecta=us.codigousuario
	inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad
	inner join SUCURSAL su on e.codigounidad2=su.codigounidad and e.codigosucursal2=su.codigosucursal
	inner join TipoIncidente ti on ie.idTipoIncidente=ti.idTipoIndicente
	where ie.idIncidente=" + detallemantenimiento.idIncidente.ToString() + "; select idRevision,revision, fecha_revision,isCambioPiezas,u.ShortName from RevisionIncidente ri inner join USUARIO u on ri.user_revision=u.codigousuario where idIncidente=" + detallemantenimiento.idIncidente.ToString() + " order by fecha_revision";
                 using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                 {

                     SqlCommand command = new SqlCommand(queryString, connection);
                     connection.Open();
                     SqlDataReader reader = command.ExecuteReader();
                     try
                     {
                         if (reader.HasRows)
                         {
                             while (reader.Read())
                             {
                                 item.idIncidente = Convert.ToInt32(reader["idIncidente"].ToString());
                                 item.idEquipo = Convert.ToInt32(reader["idEquipo"].ToString());
                                 item.equipo = new EquipoMantenimiento();
                                 item.equipo.idEquipo = item.idEquipo;
                                 item.equipo.marca = (reader["marca"].ToString());
                                 item.equipo.modelo = (reader["modelo"].ToString());
                                 item.equipo.nombreEquipo = (reader["nombreequipo"].ToString());
                                 item.equipo.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                                 item.equipo.empresaMantenimiento = new EmpresaMantenimiento();
                                 item.equipo.empresaMantenimiento.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                                 item.equipo.empresa = (reader["empresa"].ToString());
                                 item.equipo.sucursal = (reader["sucursal"].ToString());
                                 item.equipo.empresaMantenimiento.razonsocial = (reader["nombreempresa"].ToString());
                                 item.fecha_detecta = Convert.ToDateTime(reader["fecha_detecta"].ToString());
                                 item.falla_detectada = (reader["falla_detecta"].ToString());
                                 item.idTipoIncidente = Convert.ToInt32(reader["idTipoIncidente"].ToString());
                                 item.tipoIncidente = (reader["tipoincidente"].ToString());
                                 item.usuario_detecta = (reader["ShortName"].ToString());
                                 item.listaRevisiones = new List<DetalleMantenimiento>();
                             }
                             if (reader.NextResult())
                                 while (reader.Read())
                                 {
                                     DetalleMantenimiento dm = new DetalleMantenimiento();
                                     dm.idIncidente = item.idIncidente;
                                     dm.idDetalleMantenimiento = Convert.ToInt32(reader["idRevision"].ToString());
                                     dm.fecha_revision = Convert.ToDateTime(reader["fecha_revision"].ToString());
                                     dm.revision = (reader["revision"].ToString());
                                     dm.isCambioPieza = Convert.ToBoolean(reader["isCambioPiezas"].ToString());
                                     dm.usuario_revisa = (reader["ShortName"].ToString());
                                     dm.listaPiezas = new List<PiezasMantenimiento>();
                                     item.listaRevisiones.Add(dm);
                                     if (dm.isCambioPieza)
                                     {
                                         dm.listaPiezas.AddRange(db.PiezasRevision.Where(x => x.idRevision == dm.idDetalleMantenimiento).Select(x => new PiezasMantenimiento { nombre = x.nombre, serie = x.serie }).ToList());
                                     }
                                 }
                         }

                     }
                     finally
                     {
                         // Always call Close when done reading.
                         reader.Close();
                         connection.Close();

                     }
                 }
                 ViewBag.Mantenimiento = item;
                 ViewBag.EstadoEquipo = new SelectList(new Variable().getEstadoEquipoMantenimiento(), "nombre", "nombre");

                 DetalleMantenimiento revision = new DetalleMantenimiento();
                 revision.idIncidente = item.idIncidente;

                 return View(revision);
             }

         }
         public ActionResult ListaIncidenteEquipo(int equipo)
         {
             List<MantenimientoViewModel> lstIncidente = new List<MantenimientoViewModel>();
             List<Object> lstTipoIncidente = new List<Object>();
             string queryString = @"select ie.idIncidente,ie.idEquipo,e.nombreequipo,
	e.marca,e.modelo,u.ShortName,
	e.idEmpresa,es.razonsocial nombreempresa,ie.idTipoIncidente
	,ti.Nombre tipoincidente,ie.fecha_detecta ,ie.isSolucionado,ie.fecha_solucionado
	from IncidenteEquipo ie
	inner join EQUIPO e on ie.idEquipo=e.codigoequipo
	inner join USUARIO u on ie.user_detecta=u.codigousuario
	inner join EmpresaServicio es on e.idEmpresa=es.idEmpresa	
	inner join TipoIncidente ti on ie.idTipoIncidente=ti.idTipoIndicente
	where ie.idEquipo=" + equipo.ToString();
             using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
             {

                 SqlCommand command = new SqlCommand(queryString, connection);
                 connection.Open();
                 SqlDataReader reader = command.ExecuteReader();
                 try
                 {
                     if (reader.HasRows)
                     {
                         while (reader.Read())
                         {
                             MantenimientoViewModel item = new MantenimientoViewModel();
                             item.idIncidente = Convert.ToInt32(reader["idIncidente"].ToString());
                             item.idEquipo = Convert.ToInt32(reader["idEquipo"].ToString());
                             item.equipo = new EquipoMantenimiento();
                             item.equipo.idEquipo = item.idEquipo;
                             item.equipo.marca = (reader["marca"].ToString());
                             item.equipo.modelo = (reader["modelo"].ToString());
                             item.equipo.nombreEquipo = (reader["nombreequipo"].ToString());
                             item.equipo.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                             item.equipo.empresaMantenimiento = new EmpresaMantenimiento();
                             item.equipo.empresaMantenimiento.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                             item.equipo.empresaMantenimiento.razonsocial = (reader["nombreempresa"].ToString());
                             item.fecha_detecta = Convert.ToDateTime(reader["fecha_detecta"].ToString());
                             item.idTipoIncidente = Convert.ToInt32(reader["idTipoIncidente"].ToString());
                             item.tipoIncidente = (reader["tipoincidente"].ToString());
                             item.isSolucionado = Convert.ToBoolean(reader["isSolucionado"].ToString());
                             item.usuario_detecta = (reader["ShortName"].ToString());
                             if (reader["fecha_solucionado"] != DBNull.Value)
                                 item.fecha_solucionado = Convert.ToDateTime(reader["fecha_solucionado"].ToString());
                             lstIncidente.Add(item);
                         }
                     }

                 }
                 finally
                 {
                     // Always call Close when done reading.
                     reader.Close();
                     connection.Close();

                 }
             }
             ViewBag.Equipo = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == equipo).nombreequipo;
             return View(lstIncidente);
         }
         public ActionResult DetalleIncidente(int incidente)
         {
             MantenimientoViewModel item = new MantenimientoViewModel();
             string queryString = @"select u.NombreSmall empresa,su.ShortDesc sucursal,ie.idIncidente,ie.idEquipo,e.nombreequipo,ie.falla_detecta,e.marca,e.modelo,
	e.idEmpresa,es.razonsocial nombreempresa,ie.idTipoIncidente
	,ti.Nombre tipoincidente,ie.fecha_detecta ,us.ShortName,ie.isSolucionado,ie.fecha_solucionado
	from IncidenteEquipo ie
	inner join EQUIPO e on ie.idEquipo=e.codigoequipo
inner join USUARIO us on ie.user_detecta=us.codigousuario
	inner join EmpresaServicio es on e.idEmpresa=es.idEmpresa
	inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad
	inner join SUCURSAL su on e.codigounidad2=su.codigounidad and e.codigosucursal2=su.codigosucursal
	inner join TipoIncidente ti on ie.idTipoIncidente=ti.idTipoIndicente
	where ie.idIncidente=" + incidente.ToString() + "; select idRevision,revision, fecha_revision,isCambioPiezas,u.ShortName from RevisionIncidente ri inner join USUARIO u on ri.user_revision=u.codigousuario where idIncidente=" + incidente.ToString() + " order by fecha_revision";
             using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
             {

                 SqlCommand command = new SqlCommand(queryString, connection);
                 connection.Open();
                 SqlDataReader reader = command.ExecuteReader();
                 try
                 {
                     if (reader.HasRows)
                     {
                         while (reader.Read())
                         {
                             item.idIncidente = Convert.ToInt32(reader["idIncidente"].ToString());
                             item.idEquipo = Convert.ToInt32(reader["idEquipo"].ToString());
                             item.equipo = new EquipoMantenimiento();
                             item.equipo.idEquipo = item.idEquipo;
                             item.equipo.marca = (reader["marca"].ToString());
                             item.equipo.modelo = (reader["modelo"].ToString());
                             item.equipo.nombreEquipo = (reader["nombreequipo"].ToString());
                             item.equipo.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                             item.equipo.empresaMantenimiento = new EmpresaMantenimiento();
                             item.equipo.empresaMantenimiento.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                             item.equipo.empresa = (reader["empresa"].ToString());
                             item.equipo.sucursal = (reader["sucursal"].ToString());
                             item.equipo.empresaMantenimiento.razonsocial = (reader["nombreempresa"].ToString());
                             item.fecha_detecta = Convert.ToDateTime(reader["fecha_detecta"].ToString());
                             item.falla_detectada = (reader["falla_detecta"].ToString());
                             item.idTipoIncidente = Convert.ToInt32(reader["idTipoIncidente"].ToString());
                             item.tipoIncidente = (reader["tipoincidente"].ToString());
                             item.usuario_detecta = (reader["ShortName"].ToString());
                             item.isSolucionado = Convert.ToBoolean(reader["isSolucionado"].ToString());
                             if (reader["fecha_solucionado"] != DBNull.Value)
                                 item.fecha_solucionado = Convert.ToDateTime(reader["fecha_solucionado"].ToString());
                             item.listaRevisiones = new List<DetalleMantenimiento>();
                         }
                         if (reader.NextResult())
                             while (reader.Read())
                             {
                                 DetalleMantenimiento dm = new DetalleMantenimiento();
                                 dm.idIncidente = item.idIncidente;
                                 dm.idDetalleMantenimiento = Convert.ToInt32(reader["idRevision"].ToString());
                                 dm.fecha_revision = Convert.ToDateTime(reader["fecha_revision"].ToString());
                                 dm.revision = (reader["revision"].ToString());
                                 dm.isCambioPieza = Convert.ToBoolean(reader["isCambioPiezas"].ToString());
                                 dm.usuario_revisa = (reader["ShortName"].ToString());
                                 dm.listaPiezas = new List<PiezasMantenimiento>();
                                 item.listaRevisiones.Add(dm);
                                 if (dm.isCambioPieza)
                                 {
                                     dm.listaPiezas.AddRange(db.PiezasRevision.Where(x => x.idRevision == dm.idDetalleMantenimiento).Select(x => new PiezasMantenimiento { nombre = x.nombre, serie = x.serie }).ToList());
                                 }
                             }
                     }

                 }
                 finally
                 {
                     // Always call Close when done reading.
                     reader.Close();
                     connection.Close();

                 }
             }
             ViewBag.Mantenimiento = item;

             DetalleMantenimiento revision = new DetalleMantenimiento();
             revision.idIncidente = item.idIncidente;

             return View(revision);
         }

         #endregion

    }
}
