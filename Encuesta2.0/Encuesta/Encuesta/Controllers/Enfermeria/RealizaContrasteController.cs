using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Enfermeria
{
    [Authorize(Roles = "4")]
    public class RealizaContrasteController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        // GET: /ContrasteExamenViewModel/
        public ActionResult ListaEspera()
        {
            return View();
        }
        //lista de examenes pendiestes async
        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {


            var listaEncuestas = getExamenes();
            IEnumerable<ContrasteExamenViewModel> listaEncuestasFilter;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //determinamos las columnas a filtar
                var examenFilter = Convert.ToString(Request["sSearch_0"]);
                var pacienteFilter = Convert.ToString(Request["sSearch_1"]);
                var enfermeraFilter = Convert.ToString(Request["sSearch_4"]);
                var estudioFilter = Convert.ToString(Request["sSearch_5"]);
                var equipoFilter = Convert.ToString(Request["sSearch_6"]);


                //Optionally check whether the columns are searchable at all 
                var isexamenSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
                var ispacienteSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var isenfermeraSearchable = Convert.ToBoolean(Request["bSearchable_4"]);
                var isestudioSearchable = Convert.ToBoolean(Request["bSearchable_5"]);
                var isequipoSearchable = Convert.ToBoolean(Request["bSearchable_6"]);


                listaEncuestasFilter = getExamenes()
                   .Where(c => isexamenSearchable && c.num_examen.ToString().Contains(param.sSearch.ToLower())
                               ||
                               ispacienteSearchable && c.nom_paciente.ToLower().Contains(param.sSearch.ToLower())
                                ||
                               isenfermeraSearchable && c.enfermera.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isestudioSearchable && c.nom_estudio.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isequipoSearchable && c.nom_equipo.ToLower().Contains(param.sSearch.ToLower())

                               );
            }
            else
            {
                listaEncuestasFilter = listaEncuestas;
            }

            var isexamenSortable = Convert.ToBoolean(Request["bSortable_0"]);
            var ispacienteSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var ishadminSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isminSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var isenfermeraSortable = Convert.ToBoolean(Request["bSortable_4"]);
            var isestudioSortable = Convert.ToBoolean(Request["bSortable_5"]);
            var isequipoSortable = Convert.ToBoolean(Request["bSortable_6"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ContrasteExamenViewModel, string> orderingFunction = (
                c => sortColumnIndex == 0 && isexamenSortable ? c.num_examen.ToString() :
                    sortColumnIndex == 1 && ispacienteSortable ? c.nom_paciente :
                    sortColumnIndex == 2 && ishadminSortable ? c.hora_admision.ToString() :
                    sortColumnIndex == 3 && isminSortable ? c.min_transcurri :
                    sortColumnIndex == 4 && isenfermeraSortable ? c.condicion :
                    sortColumnIndex == 5 && isestudioSortable ? c.nom_estudio :
                    sortColumnIndex == 6 && isequipoSortable ? c.nom_equipo :
                    "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                    Convert.ToString(c.num_examen),//0
                    c.nom_paciente,//1
                    Convert.ToString(c.hora_admision),//2
                    Convert.ToString(c.min_transcurri),//3
                    c.enfermera,//4
                    c.nom_estudio,//5
                    c.nom_equipo,//6
                    Convert.ToString(c.idcontraste),//7
                    Convert.ToString(c.isVIP),//8
                    Convert.ToString(c.isSedacion ),//9
                    ""
                };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = listaEncuestas.Count(),
                iTotalDisplayRecords = listaEncuestasFilter.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }
        //metodo que obtiene la data
        private IList<ContrasteExamenViewModel> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            IList<ContrasteExamenViewModel> allSupervisiones = (from ea in db.EXAMENXATENCION
                                                                join e in db.EQUIPO on ea.equipoAsignado equals e.codigoequipo into e_join
                                                                from e in e_join.DefaultIfEmpty()
                                                                join p in db.PACIENTE on ea.codigopaciente equals p.codigopaciente
                                                                join en in db.Encuesta on ea.codigo equals en.numeroexamen
                                                                join co in db.Contraste on ea.codigo equals co.numeroexamen
                                                                join su in db.SUCURSAL on new { codigounidad = ea.codigoestudio.Substring(0, 1), codigosucursal = ea.codigoestudio.Substring(2, 1) }
                                                                equals new
                                                                {
                                                                    codigounidad = SqlFunctions.StringConvert((double)(su.codigounidad)).Trim(),
                                                                    codigosucursal = SqlFunctions.StringConvert((double)
                                                                        (su.codigosucursal)).Trim()
                                                                }
                                                                where co.estado == 0 &&
                                                                    (user.sucursales).Contains(ea.codigoestudio.Substring(1 - 1, 3)) //sucursales asignadas

                                                                orderby ea.codigo
                                                                select new ContrasteExamenViewModel
                                                                {
                                                                    idcontraste = co.idcontraste,
                                                                    num_examen = ea.codigo,
                                                                    nom_paciente = p.apellidos + " " + p.nombres,
                                                                    sede = su.UNIDADNEGOCIO.nombre + " - " + su.ShortDesc,
                                                                    hora_cita = ea.horaatencion,
                                                                    hora_admision = ea.ATENCION.fechayhora,
                                                                    min_transcurri =
                                                                    SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ?
                                                                    (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " mes(es)") :
                                                                    SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ?
                                                                    (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " día(s)") :
                                                                    SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", ea.ATENCION.fechayhora, SqlFunctions.GetDate())),
                                                                    condicion = ea.ATENCION.CITA.categoria == "AMBULATORIO" ? "" : ea.ATENCION.CITA.categoria,
                                                                    nom_estudio = ea.ESTUDIO.nombreestudio,
                                                                    nom_equipo = e.nombreequipo == null ? "No Asignado" : e.nombreequipo,

                                                                    enfermera = co.USUARIO2.ShortName
                                                                }).AsParallel().ToList();

            return allSupervisiones;
        }
        // GET: /Contraste/Create
        public ActionResult RealizarContraste(int contraste)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            ContrasteExamenViewModel model = new ContrasteExamenViewModel();
            var _contraste = db.Contraste.Where(x => x.idcontraste == contraste).SingleOrDefault();
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == _contraste.numeroexamen select x).SingleOrDefault();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
            var _encuesta = (from x in db.Encuesta where x.numeroexamen == _examen.codigo select x).SingleOrDefault();
            //Contraste
            model.contraste = _contraste;
            model.idcontraste = _contraste.idcontraste;
            model.insumos = getDetalleInsumos(_contraste.idcontraste);
            model.cantidadContrasteCaja = 1;
            //PACIENTE
            model.paciente = _paciente;
            //EXAMENXATENCION
            model.examen = _examen;
            //INSUMOS ENFERMERIA
            if (_examen.ESTUDIO.nombreestudio.ToLower().Contains("artro"))


                ViewBag.tecnicas = getListaInsumos(-1);

            else

                ViewBag.tecnicas = getListaInsumos(int.Parse(_examen.codigoestudio.Substring(4, 1)));
            //if (_examen.ESTUDIO.nombreestudio.ToLower().Contains("artro"))
            //{
            //    ViewBag.tecnicas = (from insu in db.Insumo_Cum_Sunasa
            //                        where insu.isactivo == true &&
            //                        insu.isSedacion == false
            //                        orderby insu.producto
            //                        select insu).ToList();
            //}
            //else
            //{
            //    bool isRM = _examen.codigoestudio.Substring(4, 1) == "1";
            //    bool isTEM = _examen.codigoestudio.Substring(4, 1) == "2";
            //    ViewBag.tecnicas = (from insu in db.Insumo_Cum_Sunasa
            //                        where insu.isactivo == true &&
            //                        insu.isRM == isRM &&
            //                        insu.isTEM == isTEM &&
            //                        insu.isSedacion == false
            //                        orderby insu.producto
            //                        select insu).ToList();
            //}

            //INSUMOS CAJA
            ViewBag.insumoContraste = new SelectList((from x in db.ESTUDIO
                                                      where x.nombreestudio.Contains("ampliacion") == false &&
                                                      x.nombreestudio.Contains("contraste") &&
                                                      x.codigoestudio.Substring(0, 1) == _examen.codigoestudio.Substring(0, 1) &&
                                                      x.codigoestudio.Substring(1, 2) == _examen.codigoestudio.Substring(1, 2) &&
                                                      x.codigoestudio.Substring(3, 2) == _examen.codigoestudio.Substring(3, 2)
                                                      select x).ToList(), "codigoestudio", "nombreestudio");

            //TIPO INYECTOR
            ViewBag.inyector = new SelectList((new List<Object> { new { codigo = "Manual" }, new { codigo = "Inyector" } }), "codigo", "codigo");
            ViewBag.Error = new SelectList(new Variable().getCancelacionesContraste(), "codigo", "codigo");

            return View(model);
        }
        //Ingresar Insumo
        public ActionResult IngresarInsumo(int contraste, int insumo, double cant, bool isaplicado, string lote, string error)
        {
            bool result = true;
            string msj = "";

            #region Registar Tecnica
            Detalle_Contraste dc = new Detalle_Contraste();
            dc.id_contraste = contraste;
            dc.Al_InsumId = insumo;
            //dc.id_insumo = insumo;
            dc.cantidad = (decimal)cant;
            dc.isaplicado = !isaplicado;
            dc.lote = lote;
            dc.tipoerroneo = error;
            db.Detalle_Contraste.Add(dc);

            #endregion
            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }
        //Listar insumos
        public ActionResult ListarInsumos(int idcontraste)
        {
            List<DetalleInsumosViewModel> lista = getDetalleInsumos(idcontraste);
            return PartialView("_ListInsumos", lista);
        }

        public List<DetalleInsumosViewModel> getDetalleInsumos(int idcontraste)
        {
            List<DetalleInsumosViewModel> lista = new List<DetalleInsumosViewModel>();

            string sqlPost = @"select d.idDetalle_contras,case when d.id_insumo is null then  (	select i.Nombre from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=d.Al_InsumId) else (select i.producto from Insumo_Cum_Sunasa i where i.idInsumo=d.id_insumo)  end insumo,d.cantidad,d.lote,d.isaplicado, isnull(d.tipoerroneo,'') tipoerroneo  from Detalle_Contraste d where d.id_contraste='{0}'";

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, idcontraste.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {

                        while (reader.Read())
                        {
                            var item = new DetalleInsumosViewModel();

                            item.iddetalle = Convert.ToInt32(reader["idDetalle_contras"]);
                            item.nom_insumo = (reader["insumo"]).ToString();
                            item.cantidad = Convert.ToDouble(reader["cantidad"].ToString());
                            item.frasco = (reader["lote"]).ToString();
                            item.aplicado = Convert.ToBoolean(reader["isaplicado"].ToString());
                            item.tipoerroneo = (reader["tipoerroneo"]).ToString();
                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        reader.Close();

                        connection.Close();
                    }
                }
            }
            return lista;

        }
        //Elimiar insumo
        public ActionResult EliminarInsumo(int nco)
        {
            bool result = true;
            string msj = "";
            var dc = (from ci in db.Detalle_Contraste
                      where ci.idDetalle_contras == nco
                      select ci).Single();

            db.Detalle_Contraste.Remove(dc);
            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RealizarContraste(ContrasteExamenViewModel item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var id = item.idcontraste;
            var con = db.Contraste.Where(x => x.idcontraste == id).SingleOrDefault();
            if (con != null)
            {
                var _examen = (from x in db.EXAMENXATENCION where x.codigo == con.numeroexamen select x).SingleOrDefault();
                var _tecnologo = (from x in db.HONORARIOTECNOLOGO where x.codigohonorariotecnologo == con.numeroexamen select x).SingleOrDefault();
                var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
                var _encuesta = (from x in db.Encuesta where x.numeroexamen == _examen.codigo select x).SingleOrDefault();
                var nombreestudio = _examen.ESTUDIO.nombreestudio;
                //Contraste
                item.contraste = con;
                item.idcontraste = con.idcontraste;
                item.insumos = getDetalleInsumos(con.idcontraste);
                //PACIENTE
                item.paciente = _paciente;
                //EXAMENXATENCION
                item.examen = _examen;
                ViewBag.insumoContraste = new SelectList((from x in db.ESTUDIO
                                                          where x.nombreestudio.Contains("ampliacion") == false &&
                                                          x.nombreestudio.Contains("contraste") &&
                                                          x.codigoestudio.Substring(0, 1) == _examen.codigoestudio.Substring(0, 1) &&
                                                          x.codigoestudio.Substring(1, 2) == _examen.codigoestudio.Substring(1, 2) &&
                                                          x.codigoestudio.Substring(3, 2) == _examen.codigoestudio.Substring(3, 2)
                                                          select x).ToList(), "codigoestudio", "nombreestudio");
                //INSUMOS

                if (_examen.ESTUDIO.nombreestudio.ToLower().Contains("artro"))


                    ViewBag.tecnicas = getListaInsumos(-1);

                else

                    ViewBag.tecnicas = getListaInsumos(int.Parse(_examen.codigoestudio.Substring(4, 1)));

                //if (_examen.ESTUDIO.nombreestudio.ToLower().Contains("artro"))
                //{
                //    ViewBag.tecnicas = (from insu in db.Insumo_Cum_Sunasa
                //                        where insu.isactivo == true &&
                //                        insu.isSedacion == false
                //                        orderby insu.producto
                //                        select insu).ToList();
                //}
                //else
                //{
                //    bool isRM = _examen.codigoestudio.Substring(4, 1) == "1";
                //    bool isTEM = _examen.codigoestudio.Substring(4, 1) == "2";
                //    ViewBag.tecnicas = (from insu in db.Insumo_Cum_Sunasa
                //                        where insu.isactivo == true &&
                //                        insu.isRM == isRM &&
                //                        insu.isTEM == isTEM &&
                //                        insu.isSedacion == false
                //                        orderby insu.producto
                //                        select insu).ToList();
                //}
                //TIPO INYECTOR
                ViewBag.inyector = new SelectList((new List<Object> { new { codigo = "Manual" }, new { codigo = "Inyector" } }), "codigo", "codigo");
                ViewBag.Error = new Variable().getCancelacionesContraste();

                #region Registar Tecnica Contraste
                var cita = (from ci in db.EXAMENXCITA
                            where ci.numerocita == _examen.numerocita &&
                            ci.codigoexamencita == (db.EXAMENXCITA.Where(ci1 => ci1.numerocita == _examen.numerocita).Max(ci1 => ci1.codigoexamencita))
                            select ci).Single();

                for (int i = 0; i < item.cantidadContrasteCaja; i++)
                {
                    EXAMENXCITA exc = new EXAMENXCITA();
                    exc.numerocita = cita.numerocita;
                    exc.codigopaciente = cita.codigopaciente;
                    exc.horacita = cita.horacita;
                    if (!nombreestudio.Contains("Artro"))
                    {
                        exc.precioestudio = (from cia in db.ESTUDIO_COMPANIA where cia.codigocompaniaseguro == cita.codigocompaniaseguro && cia.codigoestudio == item.contrasteFacturacion select cia.preciobruto).SingleOrDefault();
                    }
                    else
                    {
                        exc.precioestudio = 0;
                    }
                    exc.codigocompaniaseguro = cita.codigocompaniaseguro;
                    exc.ruc = cita.ruc;
                    exc.codigoequipo = cita.codigoequipo;
                    exc.codigoestudio = item.contrasteFacturacion;
                    exc.codigoclase = int.Parse(item.contrasteFacturacion.Substring(6, 1));
                    exc.codigomodalidad = cita.codigomodalidad;
                    exc.codigounidad = cita.codigounidad;
                    exc.codigomoneda = cita.codigomoneda;
                    if (_examen.estadoestudio != "X")
                    {
                        exc.estadoestudio = "R";
                    }
                    else
                    {
                        exc.estadoestudio = "X";
                    }
                    db.EXAMENXCITA.Add(exc);

                    if (exc.codigoestudio.Substring(7, 2) == "99")
                    {
                        REGISTRODETECNICA reg = new REGISTRODETECNICA();
                        reg.numeroestudio = _examen.codigo;
                        reg.codigoestudio = _examen.codigoestudio;
                        reg.codigotecnica = item.contrasteFacturacion;
                        reg.nombretecnica = db.ESTUDIO.Where(x => x.codigoestudio == item.contrasteFacturacion).SingleOrDefault().nombreestudio;
                        reg.codigoequipo = _tecnologo.codequiporealizo;
                        reg.codigoclase = int.Parse(item.contrasteFacturacion.Substring(6, 1));
                        db.REGISTRODETECNICA.Add(reg);
                    }
                }
                #endregion


                con.insumo_medico = item.contrasteFacturacion;
                con.usuario = user.ProviderUserKey.ToString();
                con.tipo_aplicacion_contraste = item.tipoInyector;
                con.estado = 1;
                con.fecha_fin = DateTime.Now;
                con.consentimiento = item.consentimiento;
                db.SaveChanges();
                return RedirectToAction("ListaEspera");


            }

            return View(item);
        }
        public ActionResult Delete(int id)
        {
            var _contraste = db.Contraste.SingleOrDefault(x => x.idcontraste == id);
            _contraste.estado = 9;
            db.SaveChanges();
            return RedirectToAction("ListaEspera");
        }
        public ActionResult AddContraste(int examen)
        {
            var _examen = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            if (_examen != null)
                if (_examen.estadoestudio == "X")
                {
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    #region Agregar Tabla Contraste
                    Contraste enf = new Contraste();
                    enf.numeroexamen = _examen.codigo;

                    enf.tecnologo = user.ProviderUserKey.ToString();
                    enf.fecha_inicio = DateTime.Now;
                    enf.paciente = _examen.codigopaciente;
                    enf.estado = 0;
                    enf.tipo_contraste = "";
                    enf.atencion = _examen.numeroatencion;
                    db.Contraste.Add(enf);
                    db.SaveChanges();
                    #endregion
                    RedirectToAction("RealizarContraste", "RealizaContraste", new { contraste = enf.idcontraste });
                }

            return RedirectToAction("ListaEspera");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public List<Insumo> getListaInsumos(int clase)
        {

            List<Insumo> lista = new List<Insumo>();
            string condicion = "";
            if (clase == -1)
                condicion = " in ('1','2')";
            else
                condicion = " = " + clase.ToString();

            string sqlPost = @"select i.Al_InsumId, i.Nombre nomInsumo, SC.IN_SubClaseId,sc.Nombre  nomClase
            from AL_Insum i inner join AL_Enfermeria e on i.Al_InsumId = e.Al_InsumoId
            inner join IN_SubClase sc on  i.IN_SubClaseId = sc.IN_SubClaseId
            where e.isVisibleEnfermeria = '1' and sc.IN_SubClaseId {0} ";

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, condicion), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {

                        while (reader.Read())
                        {
                            var item = new Insumo();

                            item.insumoid = Convert.ToInt32(reader["Al_InsumId"]);
                            item.nombre = (reader["nomInsumo"]).ToString();
                            item.subclase = Convert.ToInt32(reader["IN_SubClaseId"]);
                            item.nomclase = (reader["nomClase"]).ToString();

                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        reader.Close();

                        connection.Close();
                    }
                }
            }
            return lista;
        }

        public ActionResult getListaFrascos(int idinsumo, string almacen)
        {

            List<Object> lista = new List<Object>();


            string sqlPost = @"select a.AL_Stock_ItemID,a.Correlativo,a.usado from(
			select si.AL_Stock_ItemID,si.Correlativo,so.FecSalida,si.AL_InsumId,
			 (select isnull(sum(dc.cantidad),0) from Detalle_Contraste dc where dc.lote = convert(varchar,si.Correlativo)and dc.Al_InsumId = si.AL_Stock_ItemID) usado,(select ii.volumen from AL_Stock_Item sii inner join AL_Insum ii on sii.Al_InsumId = ii.Al_InsumId where sii.AL_Stock_ItemID = si.AL_Stock_ItemID) total
            from AL_Stock_Item si 
            inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId 
            inner join IN_UnidadMedida u on i.VolumenUM = u.IN_UnidadMedidaId
            inner join AL_Enfermeria e on i.Al_InsumId = e.Al_InsumoId
            inner join IN_UnidadMedida um on i.ConcentracionUM = um.IN_UnidadMedidaId
            inner join IN_SubClase sc on  i.IN_SubClaseId = sc.IN_SubClaseId
            inner join Al_Stock_Consumo so on si.Al_Stock_ConsumoID=so.Al_Stock_ConsumoID
            inner join AL_Almacen al on si.AL_AlmacenId = al.AL_AlmacenId
             where e.isVisibleEnfermeria = '1' and
            (so.AL_TipoConsumoId is null or so.AL_TipoConsumoId = 1)
			and si.Al_InsumId = '{0}' and al.CodigoEquivalente ='{1}' and convert(date,so.FecSalida) > '15/01/2016' 
			)as A where 

--si esta cerrado
((a.usado=0 ) or 

--si esta abierto y tiene menos de 24 horas
((a.total > a.usado )and (convert(date,(select min(fecha_inicio) from Contraste c inner join Detalle_Contraste dc on c.idcontraste=dc.id_contraste where dc.Al_InsumId=a.AL_Stock_ItemID))

between convert(date,DATEADD(day,-1,GETDATE())) and  convert(date,GETDATE()))))-- and  convert(date,GETDATE())))) DATEADD(day,-1,GETDATE()) and  GETDATE())))

or
--conray no tiene fecha de vencimiento
(((a.total > a.usado ) and a.AL_InsumId = '1020' /*and (convert(date,a.FecSalida) between convert(date,DATEADD(MONTH,-1,GETDATE())) and  convert(date,GETDATE()))*/  ))
/*
or

((a.total > a.usado ) and (convert(date,a.FecSalida) between convert(date,DATEADD(day,-1,GETDATE())) and  convert(date,GETDATE()) ))*/
order by a.Correlativo";

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, idinsumo.ToString(), almacen), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {

                        while (reader.Read())
                        {

                            lista.Add(new
                            {
                                id = reader["AL_Stock_ItemID"].ToString(),
                                frasco = reader["Correlativo"].ToString()
                            });
                        }
                    }
                    finally
                    {
                        reader.Close();

                        connection.Close();
                    }
                }
            }
            return Json(new SelectList(lista, "id", "frasco"), JsonRequestBehavior.AllowGet);

        }

        public ActionResult getValidacionContraste(int frasco, int stock, double cantidad)
        {

            string sqlPost = @"select 
            (select isnull(sum(cantidad),0) from Detalle_Contraste dc where lote = '{0}' and dc.Al_InsumId = '{1}') usado,
            (select i.volumen from AL_Stock_Item si inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId where si.AL_Stock_ItemID = '{1}') total, (select u.Acronimo  from AL_Stock_Item si inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId inner join IN_UnidadMedida u on i.VolumenUM  = u.IN_UnidadMedidaId  where si.AL_Stock_ItemID = '{1}') unidadMedida";
            double usado = 0;
            double total = 0;
            bool result = false;
            double faltante = 0;
            string medida = "";

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, frasco.ToString(), stock.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            usado = Convert.ToDouble(reader["usado"].ToString());
                            total = Convert.ToDouble(reader["total"].ToString());
                            medida = reader["unidadMedida"].ToString();
                        }

                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }

                    if (usado < total)
                    {
                        if (usado + cantidad <= total)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                            faltante = total - usado;
                        }

                    }
                    else
                    {
                        result = false;
                        faltante = total - usado;
                    }

                }
            }
            return Json(new { result, faltante, medida }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult getStockContraste(int frasco, int stock)
        {

            string sqlPost = @"select 
            (select isnull(sum(cantidad),0) from Detalle_Contraste dc where lote = '{0}' and dc.Al_InsumId = '{1}') usado,
            (select i.volumen from AL_Stock_Item si inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId where si.AL_Stock_ItemID = '{1}') total, (select u.Acronimo  from AL_Stock_Item si inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId inner join IN_UnidadMedida u on i.VolumenUM  = u.IN_UnidadMedidaId  where si.AL_Stock_ItemID = '{1}') unidadMedida";
            double usado = 0;
            double total = 0;
            string medida = "";

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, frasco.ToString(), stock.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            usado = Convert.ToDouble(reader["usado"].ToString());
                            total = Convert.ToDouble(reader["total"].ToString());
                            medida = reader["unidadMedida"].ToString();
                        }

                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }



                }
            }
            return Json("Stock Disponible: " + (total - usado).ToString() + " " + medida, JsonRequestBehavior.AllowGet);

        }

        public ActionResult RegistraContraste()
        {
            return View();
        }
        [Authorize(Roles = "24")]
        public ActionResult InsertarContraste(int codigo)
        {
            var encuesta = db.Encuesta.SingleOrDefault(x => x.numeroexamen == codigo);
            if (encuesta != null)
            {
                var contraste = db.Contraste.SingleOrDefault(x => x.numeroexamen == encuesta.numeroexamen);
                var ht = db.HONORARIOTECNOLOGO.SingleOrDefault(x => x.codigohonorariotecnologo == codigo);
                if (ht != null)
                    ht.contraste = true;
                if (contraste == null)
                {
                    var examen = db.EXAMENXATENCION.Where(x => x.codigo == codigo).Single();

                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    #region Agregar Tabla Contraste
                    Contraste enf = new Contraste();
                    enf.numeroexamen = examen.codigo;

                    enf.tecnologo = user.ProviderUserKey.ToString();
                    enf.fecha_inicio = DateTime.Now;
                    enf.paciente = examen.codigopaciente;
                    enf.estado = 0;
                    enf.tipo_contraste = "";
                    enf.atencion = examen.numeroatencion;
                    db.Contraste.Add(enf);
                }
                else
                {
                    contraste.estado = 0;
                }
                db.SaveChanges();
                    #endregion
                return Json(true);
            }
            else return Json(false);

        }

    }
}