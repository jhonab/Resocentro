using SistemaResocentro.Controllers.Sistemas;
using SistemaResocentro.Member;
using SistemaResocentro.Models;
using SistemaResocentro.ViewModel.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaResocentro.Controllers.Mantenimiento
{
    [Authorize]
    public class MantenimientoController : Controller
    {

        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
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
            ViewBag.Sedes = new SelectList(lstEquipoMantenimiento.Select(x => new { codigo = x.idEmpresa * 100 + x.idSede, nombre = x.sucursal, empresa = x.empresa }).Distinct().OrderBy(x => x.codigo).ToList(), "codigo", "nombre", "empresa", 1);
            ViewBag.Equipo = lstEquipoMantenimiento.ToArray();
            ViewBag.Vacio = new SelectList(new Variable().getVacio(), "id", "nombre");
            var mantenimiento = new MantenimientoViewModel();
            mantenimiento.listaRevisiones = new List<DetalleMantenimiento>();
            mantenimiento.listaRevisiones.Add(new DetalleMantenimiento());
            ViewBag.EstadoEquipo = new SelectList(new Variable().getEstadoEquipoMantenimiento(), "nombre", "nombre");
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
                inci.fecha_detecta = Variable.getDatetime();
                inci.falla_detecta = mante.falla_detectada;
                inci.isSolucionado = mante.isSolucionado;
                inci.estadoequipo = mante.estadoequipo;
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
<li><b>Equipo: </b>" + equipo.nombreequipo + @"</li>
</ul>
<br/>
<b>Datos del Usuario:</b>
<ul>
<li><b>Usuario: </b>" + user.UserName.ToString() + @"</li>
<li><b>Teléfono: </b>" + mante.telefono + @"</li>
</ul>
<br/>
<b>Incidente Reportado</b>
<p><i>" + mante.falla_detectada + @"</i></p>
<br/>
<b>Estado Equipo</b>
<p><i>" + mante.estadoequipo + @"</i></p>";
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
                    new Variable().sendCorreo("Reporte de Incidente N°" + inci.idIncidente + " Equipo: " + equipo.nombreequipo + " S/N: " + equipo.serienumber, destinatarios, "", cuerpoCorreo, "");

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
                ViewBag.Sedes = new SelectList(lstEquipoMantenimiento.Select(x => new { codigo = x.idEmpresa * 100 + x.idSede, nombre = x.sucursal, empresa = x.empresa }).Distinct().OrderBy(x => x.codigo).ToList(), "codigo", "nombre", "empresa", 1);
                ViewBag.Equipo = lstEquipoMantenimiento.ToArray();
                ViewBag.Vacio = new SelectList(new Variable().getVacio(), "id", "nombre");
                ViewBag.EstadoEquipo = new SelectList(new Variable().getEstadoEquipoMantenimiento(), "nombre", "nombre");
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
            var empresa = db.EmpresaServicio.SingleOrDefault(x => x.codigousuario == codigousuario);
            List<Object> lstTipoIncidente = new List<Object>();
            string queryString = @"select u.NombreSmall empresa,su.ShortDesc sucursal,ie.idIncidente,ie.idEquipo,e.nombreequipo,
	mo.abreviatura modalidad,e.marca,e.modelo,
	e.idEmpresa,es.razonsocial nombreempresa,ie.idTipoIncidente
	,ti.Nombre tipoincidente,ie.fecha_detecta, e.serienumber
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
                            item.equipo.serie = (reader["serienumber"].ToString());
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
	e.idEmpresa,es.razonsocial nombreempresa,ie.idTipoIncidente,us.ShortName,ie.telefono
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
                            item.telefono = (reader["telefono"].ToString());
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
<li><b>Equipo: </b>" + equipo.nombreequipo + @"</li>
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
<p><i>" + incidente.falla_detecta + @"</i></p><br/>
<b>Revisión Realizada</b>
<p><i>" + detallemantenimiento.revision + @"</i></p><br/>";
                        if (rev.isCambioPiezas)
                        {
                            cuerpoCorreo += "<table class=\"table compact table-striped  table-hover\"><thead><tr><th>Pieza</th><th>Serie</th></tr></thead><tbody id=\"tab_body_pieza\">";

                            foreach (var item in detallemantenimiento.listaPiezas)
                            {
                                cuerpoCorreo += "<tr> <td>" + item.nombre + "</td><td>" + item.serie + "</td></tr>";
                            }
                            cuerpoCorreo += "</tbody></table>";
                        }

                        cuerpoCorreo += @"
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
	where ie.idIncidente=" + incidente.ToString() + "; select idRevision,revision, fecha_revision,isCambioPiezas,u.ShortName from RevisionIncidente ri inner join USUARIO u on ri.user_revision=u.codigousuario where idIncidente=" + incidente.ToString() + " order by fecha_revision ";
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
                            item.listaConversaciones = new List<ConversacionesMantenimiento>();
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
                                if (dm.isCambioPieza)
                                {
                                    dm.listaPiezas.AddRange(db.PiezasRevision.Where(x => x.idRevision == dm.idDetalleMantenimiento).Select(x => new PiezasMantenimiento { nombre = x.nombre, serie = x.serie }).ToList());
                                }
                                item.listaRevisiones.Add(dm);
                            }
                        /*if (reader.NextResult())
                            while (reader.Read())
                            {
                                ConversacionesMantenimiento dm = new ConversacionesMantenimiento();
                                dm.idConversacion = Convert.ToInt32(reader["idconversacion"].ToString());
                                dm.idIncidente = item.idIncidente;
                                dm.fecha = Convert.ToDateTime(reader["fecha_reg"].ToString());
                                dm.usuario = (reader["nombre"].ToString());
                                dm.mensaje = (reader["mensaje"].ToString());
                                item.listaConversaciones.Add(dm);
                            }
                         */
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

        public ActionResult ListaConversacionIncidente(int incidente)
        {
            List<ConversacionesMantenimiento> lstConversaciones = new List<ConversacionesMantenimiento>();
            string queryString = @"select idconversacion,nombre,mensaje,fecha_reg from conversacionincidente where idincidente=" + incidente.ToString() + " and isdelete =0 order by fecha_reg desc";
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
                            ConversacionesMantenimiento dm = new ConversacionesMantenimiento();
                            dm.idConversacion = Convert.ToInt32(reader["idconversacion"].ToString());
                            dm.idIncidente = incidente;
                            dm.fecha = Convert.ToDateTime(reader["fecha_reg"].ToString());
                            dm.usuario = (reader["nombre"].ToString());
                            dm.mensaje = (reader["mensaje"].ToString());
                            lstConversaciones.Add(dm);
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
            return View(lstConversaciones);
        }

        public ActionResult RegistrarConversacionIncidente(string data)
        {
            try
            {
                ConversacionesMantenimiento incidente = Newtonsoft.Json.JsonConvert.DeserializeObject<ConversacionesMantenimiento>(data);
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                ConversacionIncidente con = new ConversacionIncidente();
                con.idIncidente = incidente.idIncidente;
                con.usuario = user.ProviderUserKey.ToString();
                con.nombre = incidente.usuario;
                con.fecha_reg = Variable.getDatetime();
                con.mensaje = incidente.mensaje.Replace("&lt;", "<").Replace("&gt;", ">"); ;
                con.isDelete = false;
                db.ConversacionIncidente.Add(con);
                db.SaveChanges();
                var fecha = Variable.getDatetime();
                var inci = db.IncidenteEquipo.SingleOrDefault(x => x.idIncidente == incidente.idIncidente);
                if (incidente != null)
                {
                    var equipo = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == inci.idEquipo);
                    if (equipo != null)
                    {

                        string cuerpoCorreo = "Se comento el Incidente N° "+incidente.idIncidente+" del Equipo: " + equipo.nombreequipo + " S/N: " + equipo.serienumber +", por favor ingrese a nuestra web mediante el siguiente <a href=http://extranet.resocentro.com:5024/Mantenimiento/DetalleIncidente?Incidente="+incidente.idIncidente+">Enlace</a>.";

                        var empresa = db.EmpresaServicio.SingleOrDefault(x => x.idEmpresa == equipo.idEmpresa);
                        string destinatarios = "mantenimiento@resocentro.com;";
                        if (empresa != null && inci.idTipoIncidente != 2)
                            destinatarios += (empresa.correoSoporte) + "";
                       
                        new Variable().sendCorreo("Nuevo Comentario - Incidente N°" + incidente.idIncidente + " Equipo: " + equipo.nombreequipo + " S/N: " + equipo.serienumber, destinatarios, "", cuerpoCorreo, "");

                    }
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                ex = ex;
                return Json(false);
            }

        }
        public ActionResult activarIncidente(int id)
        {
            try
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                var incidente = db.IncidenteEquipo.SingleOrDefault(x => x.idIncidente == id);
                if (incidente != null)
                {
                    incidente.isSolucionado = false;
                    db.SaveChanges();
                    if (incidente != null)
                    {
                        var equipo = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == incidente.idEquipo);
                        if (equipo != null)
                        {

                            string cuerpoCorreo = "Se re-activo el Incidente N° " + incidente.idIncidente + " del Equipo: " + equipo.nombreequipo + " S/N: " + equipo.serienumber + ", por el usuario "+user.ProviderName.ToString()+" por favor ingrese a nuestra web mediante el siguiente <a href=http://extranet.resocentro.com:5024/Mantenimiento/DetalleIncidente?Incidente=" + incidente.idIncidente + ">Enlace</a>.";

                            var empresa = db.EmpresaServicio.SingleOrDefault(x => x.idEmpresa == equipo.idEmpresa);
                            string destinatarios = "mantenimiento@resocentro.com;";
                            if (empresa != null && incidente.idTipoIncidente != 2)
                                destinatarios += (empresa.correoSoporte) + "";

                            new Variable().sendCorreo("Nuevo Comentario - Incidente N°" + incidente.idIncidente + " Equipo: " + equipo.nombreequipo + " S/N: " + equipo.serienumber, destinatarios, "", cuerpoCorreo, "");

                        }
                    }
                    return Json(true);
                }
                else
                    return Json(false);
            }
            catch (Exception)
            {
                return Json(false);
            }

        }


        #endregion

        #region Empresa Servicio
        public ActionResult CrearEmpresa()
        {
            List<Telefono_EmpresaMantenimiento> lstTipoTelefono = new List<Telefono_EmpresaMantenimiento>();
            string queryString = @"select idTipoTelefono,nombre from TipoTelefono";
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
                            Telefono_EmpresaMantenimiento item = new Telefono_EmpresaMantenimiento();
                            item.idTipoTelefono = Convert.ToInt32(reader["idTipoTelefono"].ToString());
                            item.descripcion = (reader["nombre"].ToString());
                            lstTipoTelefono.Add(item);
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
            ViewBag.TipoIncidente = new SelectList(lstTipoTelefono, "idTipoTelefono", "descripcion");
            return View(new EmpresaMantenimiento());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEmpresa(EmpresaMantenimiento empresa)
        {
            try
            {
                if (empresa.telefonos_array != "")
                    empresa.telefonos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Telefono_EmpresaMantenimiento>>(empresa.telefonos_array);
                else
                    empresa.telefonos = new List<Telefono_EmpresaMantenimiento>();
                EmpresaServicio item = new EmpresaServicio();
                item.razonsocial = empresa.razonsocial;
                item.ruc = empresa.ruc;
                item.correoMantenimiento = empresa.correoMantenimiento;
                item.correoSoporte = empresa.correoSoporte;
                db.EmpresaServicio.Add(item);
                foreach (var te in empresa.telefonos)
                {
                    TelefonoEmpresa telefono = new TelefonoEmpresa();
                    telefono.idEmpresa = item.idEmpresa;
                    telefono.numero = te.numero;
                    telefono.idTipoTelefono = te.idTipoTelefono;
                    db.TelefonoEmpresa.Add(telefono);
                }
                db.SaveChanges();
                return RedirectToAction("ListaEmpresa");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                ex = ex;

                List<Telefono_EmpresaMantenimiento> lstTipoTelefono = new List<Telefono_EmpresaMantenimiento>();
                string queryString = @"select idTipoTelefono,nombre from TipoTelefono";
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
                                Telefono_EmpresaMantenimiento item = new Telefono_EmpresaMantenimiento();
                                item.idTipoTelefono = Convert.ToInt32(reader["idTipoTelefono"].ToString());
                                item.descripcion = (reader["nombre"].ToString());
                                lstTipoTelefono.Add(item);
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
                ViewBag.TipoIncidente = new SelectList(lstTipoTelefono, "idTipoTelefono", "descripcion");
                return View(new EmpresaMantenimiento());
            }
        }

        public ActionResult ListaEmpresa()
        {
            List<EmpresaMantenimiento> lstEmpresa = new List<EmpresaMantenimiento>();
            string queryString = @"select idEmpresa,razonsocial,ruc,correoMantenimiento,correoSoporte from EmpresaServicio";
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
                            EmpresaMantenimiento item = new EmpresaMantenimiento();
                            item.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                            item.razonsocial = (reader["razonsocial"].ToString());
                            item.ruc = (reader["ruc"].ToString());
                            item.correoMantenimiento = (reader["correoMantenimiento"].ToString());
                            item.correoSoporte = (reader["correoSoporte"].ToString());
                            lstEmpresa.Add(item);
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
            return View(lstEmpresa);
        }
        public ActionResult EditarEmpresa(int idEmpresa)
        {
            EmpresaMantenimiento empresa = new EmpresaMantenimiento();
            List<Telefono_EmpresaMantenimiento> lstTipoTelefono = new List<Telefono_EmpresaMantenimiento>();
            string queryString = @"select idEmpresa,razonsocial,ruc,correoMantenimiento,correoSoporte from EmpresaServicio where idEmpresa=" + idEmpresa.ToString() + "; select t.idTelefono,t.numero, t.idTipoTelefono, tt.nombre from TelefonoEmpresa t inner join TipoTelefono tt on t.idTipoTelefono=tt.idTipoTelefono where t.idEmpresa=" + idEmpresa.ToString() + ";select idTipoTelefono,nombre from TipoTelefono";
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

                            empresa.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                            empresa.razonsocial = (reader["razonsocial"].ToString());
                            empresa.ruc = (reader["ruc"].ToString());
                            empresa.correoMantenimiento = (reader["correoMantenimiento"].ToString());
                            empresa.correoSoporte = (reader["correoSoporte"].ToString());
                            empresa.telefonos = new List<Telefono_EmpresaMantenimiento>();
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                empresa.telefonos.Add(new Telefono_EmpresaMantenimiento
                                {
                                    idTelefono = Convert.ToInt32(reader["idTelefono"].ToString()),
                                    numero = reader["numero"].ToString(),
                                    idTipoTelefono = Convert.ToInt32(reader["idTipoTelefono"].ToString()),
                                    descripcion = reader["nombre"].ToString()
                                });
                            }
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                Telefono_EmpresaMantenimiento item = new Telefono_EmpresaMantenimiento();
                                item.idTipoTelefono = Convert.ToInt32(reader["idTipoTelefono"].ToString());
                                item.descripcion = (reader["nombre"].ToString());
                                lstTipoTelefono.Add(item);
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
            ViewBag.TipoIncidente = new SelectList(lstTipoTelefono, "idTipoTelefono", "descripcion");
            return View(empresa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEmpresa(EmpresaMantenimiento empresa)
        {
            try
            {
                if (empresa.telefonos_array != "")
                    empresa.telefonos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Telefono_EmpresaMantenimiento>>(empresa.telefonos_array);
                else
                    empresa.telefonos = new List<Telefono_EmpresaMantenimiento>();

                EmpresaServicio item = db.EmpresaServicio.SingleOrDefault(x => x.idEmpresa == empresa.idEmpresaMantenimiento);
                item.razonsocial = empresa.razonsocial;
                item.ruc = empresa.ruc;
                item.correoMantenimiento = empresa.correoMantenimiento;
                item.correoSoporte = empresa.correoSoporte;
                db.TelefonoEmpresa.RemoveRange(db.TelefonoEmpresa.Where(x => x.idEmpresa == item.idEmpresa).ToList());
                foreach (var te in empresa.telefonos)
                {
                    TelefonoEmpresa telefono = new TelefonoEmpresa();
                    telefono.idEmpresa = item.idEmpresa;
                    telefono.numero = te.numero;
                    telefono.idTipoTelefono = te.idTipoTelefono;
                    db.TelefonoEmpresa.Add(telefono);
                }
                db.SaveChanges();
                return RedirectToAction("ListaEmpresa");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                ex = ex;

                List<Telefono_EmpresaMantenimiento> lstTipoTelefono = new List<Telefono_EmpresaMantenimiento>();
                string queryString = @"select idTipoTelefono,nombre from TipoTelefono";
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
                                Telefono_EmpresaMantenimiento item = new Telefono_EmpresaMantenimiento();
                                item.idTipoTelefono = Convert.ToInt32(reader["idTipoTelefono"].ToString());
                                item.descripcion = (reader["nombre"].ToString());
                                lstTipoTelefono.Add(item);
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
                ViewBag.TipoIncidente = new SelectList(lstTipoTelefono, "idTipoTelefono", "descripcion");
                return View(new EmpresaMantenimiento());
            }
        }
        #endregion

        #region Equipo

        public ActionResult CrearEquipo()
        {
            ViewBag.Vacio = new SelectList(new Variable().getVacio(), "id", "nombre");

            ViewBag.Frecuencia = new SelectList(db.FrecuenciaMantenimiento.Select(x => new { codigo = x.idFrecuencia, nombre = x.nombre + " (" + x.cant_meses + ")" }).ToList(), "codigo", "nombre");

            ViewBag.Unidad = new SelectList(db.UNIDADNEGOCIO.Select(x => new { codigo = x.codigounidad, nombre = x.nombre }).ToList(), "codigo", "nombre");

            ViewBag.Sedes = db.SUCURSAL.Where(x => x.IsEnabled == true).Select(x => new { x.codigounidad, x.codigosucursal, x.descripcion }).ToArray();

            ViewBag.Modalidad = db.MODALIDAD.Select(x => new { x.codigounidad, x.codigomodalidad, abreviatura = x.abreviatura + " - " + x.nombre, radiacion = x.isRadiacion }).ToArray();

            ViewBag.Empresa = new SelectList(db.EmpresaServicio.Select(x => new { codigo = x.idEmpresa, nombre = x.razonsocial }).ToList(), "codigo", "nombre");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEquipo(EquipoMantenimiento item)
        {
            try
            {
                EQUIPO equipo = new EQUIPO();
                equipo.codigoequipo = db.EQUIPO.Count() + 1;
                equipo.fechaadquirio = item.fechaAdquirio;
                equipo.intensidad = float.Parse(item.intensidad.ToString());
                equipo.modelo = item.modelo;
                equipo.marca = item.marca;
                equipo.nombreequipo = item.nombreEquipo;
                equipo.codigosucursal2 = item.idSede;
                equipo.codigounidad2 = item.idEmpresa;
                equipo.codigomodalidad2 = item.idModalidad;
                equipo.estado = item.isActivo ? "1" : "0";
                equipo.ShortDesc = item.shortNombreequipo;
                equipo.LongDesc = item.LongNombreequipo;
                equipo.AETitleConsulta = item.aeTitle;
                equipo.serienumber = item.serie;
                equipo.StationName = item.stationName;
                equipo.anofabricacion = item.añoFabricacion;
                equipo.idfrecuencia = item.idfrecuencia;
                equipo.idEmpresa = item.idEmpresaMantenimiento;
                equipo.isIntegrator = item.isIntegrador;
                equipo.EstadoActual = "O";
                equipo.fec_emision_Licencia = item.fecha_emision_licencia;
                equipo.fec_vencimiento_Licencia = item.fecha_vencimiento_licencia;
                equipo.licencia = "";
                equipo.isEliminado = false;
                db.EQUIPO.Add(equipo);
                db.SaveChanges();
                return Redirect("ListaEquipo");
            }
            catch (Exception ex)
            {
                ViewBag.Vacio = new SelectList(new Variable().getVacio(), "id", "nombre");

                ViewBag.Frecuencia = new SelectList(db.FrecuenciaMantenimiento.Select(x => new { codigo = x.idFrecuencia, nombre = x.nombre + " (" + x.cant_meses + ")" }).ToList(), "codigo", "nombre");

                ViewBag.Unidad = new SelectList(db.UNIDADNEGOCIO.Select(x => new { codigo = x.codigounidad, nombre = x.nombre }).ToList(), "codigo", "nombre");

                ViewBag.Sedes = db.SUCURSAL.Where(x => x.IsEnabled == true).Select(x => new { x.codigounidad, x.codigosucursal, x.descripcion }).ToArray();

                ViewBag.Modalidad = db.MODALIDAD.Select(x => new { x.codigounidad, x.codigomodalidad, abreviatura = x.abreviatura + " - " + x.nombre, radiacion = x.isRadiacion }).ToArray();

                ViewBag.Empresa = new SelectList(db.EmpresaServicio.Select(x => new { codigo = x.idEmpresa, nombre = x.razonsocial }).ToList(), "codigo", "nombre");
                return View(item);
            }

        }
        public ActionResult ListaEquipo()
        {
            List<EquipoMantenimiento> lista = new List<EquipoMantenimiento>();
            string queryString = @"select codigoequipo,marca,modelo,nombreequipo,u.NombreSmall nombre,su.ShortDesc descripcion,estado,mo.abreviatura,f.nombre frecuencia,e.serienumber from EQUIPO e
inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad
inner join FrecuenciaMantenimiento f on e.idfrecuencia=f.idFrecuencia
inner join SUCURSAL su on e.codigounidad2=su.codigounidad and e.codigosucursal2=su.codigosucursal
inner join MODALIDAD mo on e.codigounidad2=mo.codigounidad and e.codigomodalidad2=mo.codigomodalidad 
where e.iseliminado=0
order by e.codigounidad2,e.codigosucursal2";
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
                            item.serie = (reader["serienumber"].ToString());
                            item.marca = (reader["marca"].ToString());
                            item.modelo = (reader["modelo"].ToString());
                            item.nombreEquipo = (reader["nombreequipo"].ToString());
                            item.modalidad = (reader["abreviatura"].ToString());
                            item.empresa = (reader["nombre"].ToString());
                            item.sucursal = (reader["descripcion"].ToString());
                            item.frecuencia = (reader["frecuencia"].ToString());
                            item.isActivo = (reader["estado"].ToString()) == "1" ? true : false;
                            lista.Add(item);
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
            return View(lista);
        }
        public ActionResult EditarEquipo(int idequipo)
        {
            string queryString = @"select codigoequipo,marca,modelo,nombreequipo,e.ShortDesc,e.LongDesc,e.fechaadquirio,e.intensidad,e.codigounidad2,e.codigosucursal2,e.codigomodalidad2,convert(bit,e.estado)estado,e.isIntegrator,isnull(e.AETitleConsulta,'')AETitleConsulta,isnull(e.StationName,'')StationName,e.anofabricacion,e.idfrecuencia,e.idEmpresa,e.fec_emision_Licencia,e.fec_vencimiento_Licencia,serienumber from EQUIPO e
where e.codigoequipo=" + idequipo.ToString();
            EquipoMantenimiento item = new EquipoMantenimiento();
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
                            item.idEquipo = Convert.ToInt32(reader["codigoequipo"].ToString());
                            item.marca = (reader["marca"].ToString());
                            item.modelo = (reader["modelo"].ToString());
                            item.nombreEquipo = (reader["nombreequipo"].ToString());
                            item.shortNombreequipo = (reader["ShortDesc"].ToString());
                            item.LongNombreequipo = (reader["LongDesc"].ToString());
                            item.fechaAdquirio = Convert.ToDateTime(reader["fechaadquirio"].ToString());
                            item.intensidad = Convert.ToDouble(reader["intensidad"].ToString());
                            item.idEmpresa = Convert.ToInt32(reader["codigounidad2"].ToString());
                            item.idSede = Convert.ToInt32(reader["codigosucursal2"].ToString());
                            item.idModalidad = Convert.ToInt32(reader["codigomodalidad2"].ToString());
                            item.isActivo = Convert.ToBoolean(reader["estado"].ToString());
                            item.isIntegrador = Convert.ToBoolean(reader["isIntegrator"].ToString());
                            item.aeTitle = (reader["AETitleConsulta"].ToString());
                            item.stationName = (reader["StationName"].ToString());
                            item.añoFabricacion = Convert.ToInt32(reader["anofabricacion"].ToString());
                            item.idfrecuencia = Convert.ToInt32(reader["idfrecuencia"].ToString());
                            item.serie = (reader["serienumber"].ToString());
                            item.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                            item.fecha_emision_licencia = Convert.ToDateTime(reader["fec_emision_Licencia"].ToString());
                            item.fecha_vencimiento_licencia = Convert.ToDateTime(reader["fec_vencimiento_Licencia"].ToString());
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
            ViewBag.Vacio = new SelectList(new Variable().getVacio(), "id", "nombre");

            ViewBag.Frecuencia = new SelectList(db.FrecuenciaMantenimiento.Select(x => new { codigo = x.idFrecuencia, nombre = x.nombre + " (" + x.cant_meses + ")" }).ToList(), "codigo", "nombre");

            ViewBag.Unidad = new SelectList(db.UNIDADNEGOCIO.Select(x => new { codigo = x.codigounidad, nombre = x.nombre }).ToList(), "codigo", "nombre");

            ViewBag.Sedes = db.SUCURSAL.Where(x => x.IsEnabled == true).Select(x => new { x.codigounidad, x.codigosucursal, x.descripcion }).ToArray();

            ViewBag.Modalidad = db.MODALIDAD.Select(x => new { x.codigounidad, x.codigomodalidad, abreviatura = x.abreviatura + " - " + x.nombre, radiacion = x.isRadiacion }).ToArray();

            ViewBag.Empresa = new SelectList(db.EmpresaServicio.Select(x => new { codigo = x.idEmpresa, nombre = x.razonsocial }).ToList(), "codigo", "nombre");
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEquipo(EquipoMantenimiento item)
        {
            try
            {
                var equipo = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == item.idEquipo);
                if (equipo != null)
                {
                    equipo.fechaadquirio = item.fechaAdquirio;
                    equipo.intensidad = float.Parse(item.intensidad.ToString());
                    equipo.modelo = item.modelo;
                    equipo.marca = item.marca;
                    equipo.nombreequipo = item.nombreEquipo;
                    equipo.codigosucursal2 = item.idSede;
                    equipo.codigounidad2 = item.idEmpresa;
                    equipo.codigomodalidad2 = item.idModalidad;
                    equipo.estado = item.isActivo ? "1" : "0";
                    equipo.ShortDesc = item.shortNombreequipo;
                    equipo.serienumber = item.serie;
                    equipo.LongDesc = item.LongNombreequipo;
                    equipo.AETitleConsulta = item.aeTitle;
                    equipo.StationName = item.stationName;
                    equipo.anofabricacion = item.añoFabricacion;
                    equipo.idfrecuencia = item.idfrecuencia;
                    equipo.idEmpresa = item.idEmpresaMantenimiento;
                    equipo.isIntegrator = item.isIntegrador;
                    equipo.EstadoActual = "O";
                    equipo.isEliminado = item.isEliminado;
                    equipo.fec_emision_Licencia = item.fecha_emision_licencia;
                    equipo.fec_vencimiento_Licencia = item.fecha_vencimiento_licencia;
                    equipo.licencia = "";
                    db.SaveChanges();
                    return Redirect("ListaEquipo");
                }
                else
                    throw new Exception("");
            }
            catch (Exception ex)
            {
                ViewBag.Vacio = new SelectList(new Variable().getVacio(), "id", "nombre");

                ViewBag.Frecuencia = new SelectList(db.FrecuenciaMantenimiento.Select(x => new { codigo = x.idFrecuencia, nombre = x.nombre + " (" + x.cant_meses + ")" }).ToList(), "codigo", "nombre");

                ViewBag.Unidad = new SelectList(db.UNIDADNEGOCIO.Select(x => new { codigo = x.codigounidad, nombre = x.nombre }).ToList(), "codigo", "nombre");

                ViewBag.Sedes = db.SUCURSAL.Where(x => x.IsEnabled == true).Select(x => new { x.codigounidad, x.codigosucursal, x.descripcion }).ToArray();

                ViewBag.Modalidad = db.MODALIDAD.Select(x => new { x.codigounidad, x.codigomodalidad, abreviatura = x.abreviatura + " - " + x.nombre, radiacion = x.isRadiacion }).ToArray();

                ViewBag.Empresa = new SelectList(db.EmpresaServicio.Select(x => new { codigo = x.idEmpresa, nombre = x.razonsocial }).ToList(), "codigo", "nombre");
                return View(item);
            }

        }


        #endregion

        #region Mantenimiento

        public ActionResult ListaMantenimiento()
        {
            List<EquipoMantenimiento> lista = new List<EquipoMantenimiento>();
            string queryString = @"select codigoequipo,marca,modelo,nombreequipo,u.NombreSmall nombre,su.ShortDesc descripcion,mo.abreviatura,f.nombre frecuencia,
DATEADD(month,f.cant_meses,max(i.fecha_detecta)) prox_mant,dbo.getEstadoMantenimientoxEquipo(e.codigoequipo) estado
from EQUIPO e
inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad
inner join FrecuenciaMantenimiento f on e.idfrecuencia=f.idFrecuencia
inner join SUCURSAL su on e.codigounidad2=su.codigounidad and e.codigosucursal2=su.codigosucursal
inner join MODALIDAD mo on e.codigounidad2=mo.codigounidad and e.codigomodalidad2=mo.codigomodalidad 
left join IncidenteEquipo i on e.codigoequipo=i.idEquipo and i.idTipoIncidente in (4)
where e.iseliminado=0
and e.idfrecuencia !=7
group by e.codigoequipo,marca,modelo,nombreequipo,u.NombreSmall ,su.ShortDesc ,estado,mo.abreviatura,f.nombre,f.cant_meses,e.codigounidad2,e.codigosucursal2
order by e.codigounidad2,e.codigosucursal2

";
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
                            item.marca = (reader["marca"].ToString());
                            item.modelo = (reader["modelo"].ToString());
                            item.nombreEquipo = (reader["nombreequipo"].ToString());
                            item.modalidad = (reader["abreviatura"].ToString());
                            item.empresa = (reader["nombre"].ToString());
                            item.sucursal = (reader["descripcion"].ToString());
                            item.frecuencia = (reader["frecuencia"].ToString());
                            if (reader["prox_mant"] != DBNull.Value)
                                item.fecha_proximo_mantenimiento = Convert.ToDateTime(reader["prox_mant"].ToString());
                            item.estado_mantenimiento = (reader["estado"].ToString());
                            lista.Add(item);
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
            return View(lista);
        }
        public ActionResult RegistrarMantenimiento(int equipo)
        {
            EquipoMantenimiento item = new EquipoMantenimiento();
            List<MantenimientoViewModel> lstIncidente = new List<MantenimientoViewModel>();
            string queryString = @"select u.nombre empresa,e.codigoequipo,e.codigounidad2 unidad,e.codigosucursal2 sede,u.NombreSmall+'-'+s.ShortDesc sucursal,e.nombreequipo,e.ShortDesc,e.codigomodalidad2,m.abreviatura,e.marca,e.modelo,f.nombre,e.serienumber,f.cant_meses
from EQUIPO e
inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad 
inner join SUCURSAL s on e.codigounidad2=s.codigounidad and e.codigosucursal2=s.codigosucursal
inner join MODALIDAD m on e.codigomodalidad2=m.codigomodalidad and e.codigounidad2=m.codigounidad
inner join FrecuenciaMantenimiento f on e.idfrecuencia=f.idFrecuencia where e.codigoequipo=" + equipo.ToString() + @" 
order by e.codigounidad2,e.codigosucursal2;

select ie.idIncidente,ie.idEquipo,e.nombreequipo,
	e.marca,e.modelo,u.ShortName,
	e.idEmpresa,es.razonsocial nombreempresa,ie.idTipoIncidente
	,ti.Nombre tipoincidente,ie.fecha_detecta ,ie.isSolucionado,ie.fecha_solucionado
	from IncidenteEquipo ie
	inner join EQUIPO e on ie.idEquipo=e.codigoequipo
	inner join USUARIO u on ie.user_detecta=u.codigousuario
	inner join EmpresaServicio es on e.idEmpresa=es.idEmpresa	
	inner join TipoIncidente ti on ie.idTipoIncidente=ti.idTipoIndicente
	where ie.idTipoIncidente=4 and ie.idEquipo=" + equipo.ToString();
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
                            item.idEquipo = Convert.ToInt32(reader["codigoequipo"].ToString());
                            item.idEmpresa = Convert.ToInt32(reader["unidad"].ToString());
                            item.empresa = (reader["empresa"].ToString());
                            item.marca = (reader["marca"].ToString());
                            item.modelo = (reader["modelo"].ToString());
                            item.serie = (reader["serienumber"].ToString());
                            item.idSede = Convert.ToInt32(reader["sede"].ToString());
                            item.sucursal = (reader["sucursal"].ToString());
                            item.nombreEquipo = (reader["nombreequipo"].ToString());
                            item.shortNombreequipo = (reader["ShortDesc"].ToString());
                            item.frecuencia = (reader["nombre"].ToString());
                            item.cant_mantenimientos = Convert.ToInt32(reader["cant_meses"].ToString());
                            item.idModalidad = Convert.ToInt32(reader["codigomodalidad2"].ToString());
                            item.modalidad = (reader["abreviatura"].ToString());
                        }
                        if(reader.NextResult())
                            while (reader.Read())
                            {
                                MantenimientoViewModel mant = new MantenimientoViewModel();
                                mant.idIncidente = Convert.ToInt32(reader["idIncidente"].ToString());
                                mant.idEquipo = Convert.ToInt32(reader["idEquipo"].ToString());
                                mant.equipo = new EquipoMantenimiento();
                                mant.equipo.idEquipo = item.idEquipo;
                                mant.equipo.marca = (reader["marca"].ToString());
                                mant.equipo.modelo = (reader["modelo"].ToString());
                                mant.equipo.nombreEquipo = (reader["nombreequipo"].ToString());
                                mant.equipo.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                                mant.equipo.empresaMantenimiento = new EmpresaMantenimiento();
                                mant.equipo.empresaMantenimiento.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                                mant.equipo.empresaMantenimiento.razonsocial = (reader["nombreempresa"].ToString());
                                mant.fecha_detecta = Convert.ToDateTime(reader["fecha_detecta"].ToString());
                                mant.idTipoIncidente = Convert.ToInt32(reader["idTipoIncidente"].ToString());
                                mant.tipoIncidente = (reader["tipoincidente"].ToString());
                                mant.isSolucionado = Convert.ToBoolean(reader["isSolucionado"].ToString());
                                mant.usuario_detecta = (reader["ShortName"].ToString());
                                if (reader["fecha_solucionado"] != DBNull.Value)
                                    mant.fecha_solucionado = Convert.ToDateTime(reader["fecha_solucionado"].ToString());
                                lstIncidente.Add(mant);
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
            ViewBag.Equipo = item;
            ViewBag.Incidente = lstIncidente;
            ViewBag.EstadoEquipo = new SelectList(new Variable().getEstadoEquipoMantenimiento(), "nombre", "nombre");
            DetalleMantenimiento revision = new DetalleMantenimiento();
            revision.idEquipo = item.idEquipo;
            return View(revision);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarMantenimiento(DetalleMantenimiento mante)
        {
            try
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                IncidenteEquipo inci = new IncidenteEquipo();
                inci.idEquipo = mante.idEquipo;
                inci.idTipoIncidente = 4;
                inci.user_detecta = user.ProviderUserKey.ToString();
                inci.fecha_detecta = mante.fecha_revision;
                inci.numero_informe = mante.num_informe;
                inci.estadoequipo = mante.estadoequipo;
                inci.falla_detecta = "Mantenimiento Preventivo";
                inci.isSolucionado = true;
                inci.telefono = "";
                if (inci.isSolucionado)
                {
                    inci.fecha_solucionado = Variable.getDatetime();
                    RevisionIncidente revi = new RevisionIncidente();
                    revi.idIncidente = inci.idIncidente;
                    revi.tecnico = mante.tecnico;
                    revi.estadoequipo = mante.estadoequipo;
                    revi.fecha_revision = Variable.getDatetime();
                    revi.user_revision = user.ProviderUserKey.ToString();
                    revi.revision = mante.revision;
                    if (mante.piezas_array != "")
                        mante.listaPiezas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PiezasMantenimiento>>(mante.piezas_array);
                    else
                        mante.listaPiezas = new List<PiezasMantenimiento>();
                    revi.isCambioPiezas = mante.listaPiezas.Count > 0;
                    foreach (var item in mante.listaPiezas)
                    {
                        PiezasRevision pieza = new PiezasRevision();
                        pieza.idRevision = revi.idRevision;
                        pieza.nombre = item.nombre;
                        pieza.serie = item.serie;
                        db.PiezasRevision.Add(pieza);
                    }
                    db.RevisionIncidente.Add(revi);
                }
                db.IncidenteEquipo.Add(inci);
                db.SaveChanges();
                return RedirectToAction("UploadFileIncidente", new { incidente = inci.idIncidente, revision = 0 });
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return View(mante);
            }
            catch (Exception ex)
            {
                ex = ex;
               


                EquipoMantenimiento item = new EquipoMantenimiento();
                List<MantenimientoViewModel> lstIncidente = new List<MantenimientoViewModel>();
                string queryString = @"select u.nombre empresa,e.codigoequipo,e.codigounidad2 unidad,e.codigosucursal2 sede,u.NombreSmall+'-'+s.ShortDesc sucursal,e.nombreequipo,e.ShortDesc,e.codigomodalidad2,m.abreviatura,e.marca,e.modelo,f.nombre,e.serienumber,f.cant_meses
from EQUIPO e
inner join UNIDADNEGOCIO u on e.codigounidad2=u.codigounidad 
inner join SUCURSAL s on e.codigounidad2=s.codigounidad and e.codigosucursal2=s.codigosucursal
inner join MODALIDAD m on e.codigomodalidad2=m.codigomodalidad and e.codigounidad2=m.codigounidad
inner join FrecuenciaMantenimiento f on e.idfrecuencia=f.idFrecuencia where e.codigoequipo=" + mante.idEquipo.ToString() + @" 
order by e.codigounidad2,e.codigosucursal2;

select ie.idIncidente,ie.idEquipo,e.nombreequipo,
	e.marca,e.modelo,u.ShortName,
	e.idEmpresa,es.razonsocial nombreempresa,ie.idTipoIncidente
	,ti.Nombre tipoincidente,ie.fecha_detecta ,ie.isSolucionado,ie.fecha_solucionado
	from IncidenteEquipo ie
	inner join EQUIPO e on ie.idEquipo=e.codigoequipo
	inner join USUARIO u on ie.user_detecta=u.codigousuario
	inner join EmpresaServicio es on e.idEmpresa=es.idEmpresa	
	inner join TipoIncidente ti on ie.idTipoIncidente=ti.idTipoIndicente
	where ie.idTipoIncidente=4 and ie.idEquipo=" + mante.idEquipo.ToString();
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
                                item.idEquipo = Convert.ToInt32(reader["codigoequipo"].ToString());
                                item.idEmpresa = Convert.ToInt32(reader["unidad"].ToString());
                                item.empresa = (reader["empresa"].ToString());
                                item.marca = (reader["marca"].ToString());
                                item.modelo = (reader["modelo"].ToString());
                                item.serie = (reader["serienumber"].ToString());
                                item.idSede = Convert.ToInt32(reader["sede"].ToString());
                                item.sucursal = (reader["sucursal"].ToString());
                                item.nombreEquipo = (reader["nombreequipo"].ToString());
                                item.shortNombreequipo = (reader["ShortDesc"].ToString());
                                item.frecuencia = (reader["nombre"].ToString());
                                item.cant_mantenimientos = Convert.ToInt32(reader["cant_meses"].ToString());
                                item.idModalidad = Convert.ToInt32(reader["codigomodalidad2"].ToString());
                                item.modalidad = (reader["abreviatura"].ToString());
                                
                            }
                            if (reader.NextResult())
                                while (reader.Read())
                                {
                                    MantenimientoViewModel mant = new MantenimientoViewModel();
                                    mant.idIncidente = Convert.ToInt32(reader["idIncidente"].ToString());
                                    mant.idEquipo = Convert.ToInt32(reader["idEquipo"].ToString());
                                    mant.equipo = new EquipoMantenimiento();
                                    mant.equipo.idEquipo = item.idEquipo;
                                    mant.equipo.marca = (reader["marca"].ToString());
                                    mant.equipo.modelo = (reader["modelo"].ToString());
                                    mant.equipo.nombreEquipo = (reader["nombreequipo"].ToString());
                                    mant.equipo.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                                    mant.equipo.empresaMantenimiento = new EmpresaMantenimiento();
                                    mant.equipo.empresaMantenimiento.idEmpresaMantenimiento = Convert.ToInt32(reader["idEmpresa"].ToString());
                                    mant.equipo.empresaMantenimiento.razonsocial = (reader["nombreempresa"].ToString());
                                    mant.fecha_detecta = Convert.ToDateTime(reader["fecha_detecta"].ToString());
                                    mant.idTipoIncidente = Convert.ToInt32(reader["idTipoIncidente"].ToString());
                                    mant.tipoIncidente = (reader["tipoincidente"].ToString());
                                    mant.isSolucionado = Convert.ToBoolean(reader["isSolucionado"].ToString());
                                    mant.usuario_detecta = (reader["ShortName"].ToString());
                                    if (reader["fecha_solucionado"] != DBNull.Value)
                                        mant.fecha_solucionado = Convert.ToDateTime(reader["fecha_solucionado"].ToString());
                                    lstIncidente.Add(mant);
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
                ViewBag.Equipo = item;
                ViewBag.EstadoEquipo = new SelectList(new Variable().getEstadoEquipoMantenimiento(), "nombre", "nombre");
                ViewBag.Incidente = lstIncidente;
                return View(mante);
            }



        }

        #endregion
    }
}