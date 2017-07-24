using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Herramientas
{
    [Authorize]
    public class HerramientasController : Controller
    {

        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        //
        // GET: /Herramientas/
        [Authorize(Roles = "12")]
        public ActionResult Permisos()
        {
            ViewBag.usuarios = new SelectList(db.USUARIO.Where(x => x.bloqueado == false && x.dni != null).OrderBy(x => x.ShortName).ToList(), "codigousuario", "shortname");
            ViewBag.sedes = new SelectList(db.SUCURSAL.Where(x => x.codigounidad == 1).Select(x => new { codigo = (x.codigounidad * 10) + x.codigosucursal, nombre = x.descripcion }).OrderBy(x => x.nombre).ToList(), "codigo", "nombre");
            ViewBag.permisos = new SelectList(db.Tipo_Permiso.Where(x => x.idaplicativo == 1).OrderBy(x => x.idPermiso).ToList(), "idPermiso", "permiso");
            return View();
        }
        public ActionResult getEquipo()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            return Json(new SelectList((from eq in db.EQUIPO
                                        where eq.estado == "1" &&
                                        (user.sucursales_int).Contains(eq.codigounidad2 * 100 + eq.codigosucursal2) //sucursales asignadas

                                        select eq).AsQueryable(), "ShortDesc ", "nombreequipo"), JsonRequestBehavior.AllowGet);

        }
        #region Permiso
        [Authorize(Roles = "12")]
        public ActionResult SedesxUsuario(string idusuario)
        {
            var _sedes = (from x in db.SUCURSALXUSUARIO where x.codigousuario == idusuario orderby x.codigosucursal select x).ToList();

            return PartialView("_ListSede", _sedes);
        }

        [Authorize(Roles = "12")]
        public ActionResult AddSede(int unidad, int sede, string user)
        {
            bool resul = true;
            string msj = "";
            var _su = db.SUCURSALXUSUARIO.Where(x => x.codigosucursal == sede && x.codigounidad == unidad && x.codigousuario == user).SingleOrDefault();
            if (_su == null)
            {
                SUCURSALXUSUARIO su = new SUCURSALXUSUARIO();
                su.codigousuario = user;
                su.codigounidad = unidad;
                su.codigosucursal = sede;
                su.estado = "1";
                db.SUCURSALXUSUARIO.Add(su);

            }
            else
            {
                resul = false;
                msj = "Ya esta asignada la Sede seleccionada";
            }
            db.SaveChanges();
            return Json(new { result = resul, msj = msj }, JsonRequestBehavior.DenyGet);
        }

        [Authorize(Roles = "12")]
        public ActionResult DeleteSede(int unidad, int sede, string user)
        {
            var su = db.SUCURSALXUSUARIO.Where(x => x.codigosucursal == sede && x.codigounidad == unidad && x.codigousuario == user).Single();
            db.SUCURSALXUSUARIO.Remove(su);
            db.SaveChanges();
            return SedesxUsuario(user);
        }

        [Authorize(Roles = "12")]
        public ActionResult AddPermiso(string user, int tipo, string per)
        {
            bool resul = true;
            string msj = "";
            var _pe = db.Permiso.Where(x => x.codigousuario == user && x.tipo_permiso == tipo && x.aplicativo == "ENCUESTA").SingleOrDefault();
            if (_pe == null)
            {
                Permiso pe = new Permiso();
                pe.codigousuario = user;
                pe.aplicativo = "ENCUESTA";
                pe.tipo_permiso = tipo;
                pe.descripcion = per;
                db.Permiso.Add(pe);

            }
            else
            {
                resul = false;
                msj = "Ya esta asignado el permiso seleccionado";
            }
            db.SaveChanges();
            return Json(new { result = resul, msj = msj }, JsonRequestBehavior.DenyGet);
        }

        [Authorize(Roles = "12")]
        public ActionResult DeletePermiso(int id)
        {
            var su = db.Permiso.Where(x => x.idpermiso == id && x.aplicativo == "ENCUESTA").SingleOrDefault();
            db.Permiso.Remove(su);
            db.SaveChanges();
            return PermisoxUsuario(su.codigousuario);
        }

        [Authorize(Roles = "12")]
        public ActionResult PermisoxUsuario(string idusuario)
        {
            var _permiso = db.Permiso.Where(x => x.codigousuario == idusuario && x.aplicativo == "ENCUESTA").OrderBy(x => x.descripcion).ToList();
            return PartialView("_ListPermiso", _permiso);
        }
        #endregion

        [Authorize(Roles = "14")]
        public ActionResult CambiarMedico()
        {
            return View();
        }

        [Authorize(Roles = "15")]
        public ActionResult CambiarTecnologo()
        {
            return View();
        }

        public ActionResult GetMedicos(int tipo)
        {
            return Json(new SelectList((from x in db.Permiso
                                        join u in db.USUARIO on x.codigousuario equals u.codigousuario
                                        where x.aplicativo == "ENCUESTA" && x.tipo_permiso == tipo
                                        orderby u.ShortName
                                        select u).AsQueryable(), "codigousuario", "ShortName"));
        }

        class lst_encuesta
        {
            public int nexamen { get; set; }
            public DateTime fecha { get; set; }
            public string paciente { get; set; }
            public string estudio { get; set; }
            public string encuestador { get; set; }
            public string tecnologo { get; set; }
            public string supervisor { get; set; }
            public EncuestaType tipo_encues { get; set; }
            public string sede { get; set; }
        }
        public ActionResult GetEncuesta(int examen, string paciente)
        {

            string msj = "";
            bool rsl = true;

            IQueryable<lst_encuesta> sql = (from ea in db.EXAMENXATENCION
                                            join p in db.PACIENTE on ea.codigopaciente equals p.codigopaciente
                                            join en in db.Encuesta on ea.codigo equals en.numeroexamen
                                            join us_e in db.USUARIO on en.usu_reg_encu equals us_e.codigousuario into ue_e_join
                                            from us_e in ue_e_join.DefaultIfEmpty()
                                            join us_s in db.USUARIO on en.usu_reg_super equals us_s.codigousuario into us_s_join
                                            from us_s in us_s_join.DefaultIfEmpty()
                                            join us_t in db.USUARIO on en.usu_reg_tecno equals us_t.codigousuario into us_t_join
                                            from us_t in us_t_join.DefaultIfEmpty()
                                            join su in db.SUCURSAL on new { codigounidad = ea.codigoestudio.Substring(0, 1), codigosucursal = ea.codigoestudio.Substring(2, 1) } equals new { codigounidad = SqlFunctions.StringConvert((double)(su.codigounidad)).Trim(), codigosucursal = SqlFunctions.StringConvert((double)(su.codigosucursal)).Trim() }
                                            select new lst_encuesta
                                       {
                                           nexamen = ea.codigo,
                                           fecha = ea.ATENCION.fechayhora,
                                           paciente = p.apellidos + " " + p.nombres,
                                           estudio = ea.ESTUDIO.nombreestudio,
                                           encuestador = us_e.ShortName,
                                           tecnologo = us_t.ShortName,
                                           supervisor = us_s.ShortName,
                                           tipo_encues = (EncuestaType)en.tipo_encu,
                                           sede = su.UNIDADNEGOCIO.nombre + " - " + su.ShortDesc
                                       });
            List<lst_encuesta> _encu = new List<lst_encuesta>();
            if (examen > 0 && paciente == "")
            {
                _encu = sql.Where(x => x.nexamen == examen).ToList();
            }
            else if (examen == 0 && paciente != "")
            {
                _encu = sql.Where(x => x.paciente.Contains(paciente)).ToList();
            }
            else if (examen > 0 && paciente != "")
            {
                _encu = sql.Where(x => x.paciente.Contains(paciente) && x.nexamen == examen).ToList();
            }




            if (_encu.Count > 0)
            {
                string data = "";
                foreach (var item in _encu)
                {
                    data +=
                   "<tr>" +
                       "<td>" + item.nexamen + "</td>" +
                       "<td>" + item.sede + "</td>" +
                       "<td>" + item.fecha.ToShortDateString() + "</td>" +
                       "<td>" + item.paciente + "</td>" +
                       "<td>" + item.estudio + "</td>" +
                       "<td>" + Enum.GetName(typeof(EncuestaType), item.tipo_encues) + "</td>" +
                       "<td>" + item.encuestador + "</td>" +
                       "<td>" + item.tecnologo + "</td>" +
                       "<td>" + item.supervisor + "</td>" +
                       "<td><button onclick='verEncuesta(" + item.nexamen + ")'  class='btn btn-outline btn-info' ><i class='fa fa-file-text-o'></i>  Encuesta</button></td>" +
                   "</tr>";
                }
                msj =
                    "<table class='table table-striped compact table-bordered table-hover'>" +
                        "<thead>" +
                            "<tr>" +
                                "<th>N°</th>" +
                                "<th>Sede</th>" +
                                "<th>Fecha</th>" +
                                "<th>Paciente</th>" +
                                "<th>Estudio</th>" +
                                "<th>Encuesta</th>" +
                                "<th>Encuestador</th>" +
                                "<th>Tecnologo</th>" +
                                "<th>Supervisor</th>" +
                                "<th></th>" +
                            "</tr>" +
                        "</thead>" +
                        "<tbody>" +
                                 data +
                        "</tbody>" +
                         "</table>";
            }
            else
            {
                rsl = false;
                msj = "";
            }
            return Json(new { result = rsl, msj = msj }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult GetHistorialEncuesta(int nexamen)
        {
            string msj =

                           "<div class='col-md-12'>" +
                               "<div class='panel panel-info' data-collapsed='0'>" +
                                   "<div class='panel-heading'>" +
                                       "<h2 class='panel-title'>Historial de Encuestas </h2>" +
                                   "</div>" +
                                   "<div class='panel-body'>" +
                                       "El paciente no tiene estudio previos " +
                                   "</div>" +
                               "</div>" +

                         "</div>";//AcceptVerbsAttribute-+A
            bool rsl = true;
            var _paciente = db.EXAMENXATENCION.Where(x => x.codigo == nexamen).SingleOrDefault();
            if (_paciente != null)
            {
                var _encu = (from ea in db.EXAMENXATENCION
                             join p in db.PACIENTE on ea.codigopaciente equals p.codigopaciente
                             where ea.codigopaciente == _paciente.codigopaciente
                             && ea.estadoestudio != "A"
                             && ea.estadoestudio != "X"
                             && ea.codigo != nexamen
                             orderby ea.codigo
                             select new
                             {
                                 idestudio = ea.codigoestudio,
                                 ampliacion = ea.IsAmpliInDemand,
                                 ea.AmpliMotivo,
                                 nexamen = ea.codigo,
                                 fecha = ea.ATENCION.fechayhora,
                                 paciente = p.apellidos + " " + p.nombres,
                                 estudio = ea.ESTUDIO.nombreestudio,
                                 estado = ea.estadoestudio,
                                 encuesta = ea.equipoAsignado == null ? "" : "<button onclick='verEncuesta(" + SqlFunctions.StringConvert((Double)ea.codigo) + ")' class='btn btn-outline btn-info btn-xs' ><i class='fa fa-file-text-o'></i>  Encuesta</button>",
                                 imagen = ea.UrlXero == null ? "" : "<a href='" + ea.UrlXero + "' class='btn btn-outline btn-primary  btn-xs' ><i class='fa fa-file-text-o'></i> Imagenes</a>"
                             }).ToList();
                if (_encu.Count > 0)
                {
                    string data = "";
                    foreach (var item in _encu)
                    {
                        if (item.ampliacion == true)
                        {
                            data +=
                         "<tr class='info alert-info'>" +
                             "<td>" + item.nexamen + "</td>" +
                             "<td>" + item.fecha.ToShortDateString() + "</td>" +
                             "<td>" + item.paciente + "</td>" +
                             "<td>" + item.estudio + "</td>" +
                             "<td>" + item.estado + "</td>" +
                             "<td>" + item.encuesta + " <button onclick='recuperarExamen(" + item.nexamen + ")' class='btn btn-outline btn-default btn-xs' ><i class='fa fa-file-text-o'></i>  Recuperar Examen</button>" + item.imagen + "</td>" +
                             "<td>" + item.AmpliMotivo + "</td>" +
                              "</tr>";
                        }
                        else
                        {
                            data +=
                           "<tr>" +
                               "<td>" + item.nexamen + "</td>" +
                               "<td>" + item.fecha.ToShortDateString() + "</td>" +
                               "<td>" + item.paciente + "</td>" +
                               "<td>" + item.estudio + "</td>" +
                               "<td>" + item.estado + "</td>" +
                               "<td>" + item.encuesta + " <button onclick='recuperarExamen(" + item.nexamen + ")' class='btn btn-outline btn-default btn-xs' ><i class='fa fa-file-text-o'></i>  Recuperar Examen</button>" + item.imagen + "</td>" +
                               "<td>" + item.AmpliMotivo + "</td>" +
                                "</tr>";
                        }
                    }
                    msj =

                            "<div class='col-md-12'>" +
                                "<div class='panel panel-info' data-collapsed='0'>" +
                                    "<div class='panel-heading'>" +
                                        "<h2 class='panel-title'>Historial de Encuestas </h2>" +
                                    "</div>" +
                                    "<div class='panel-body'>" +
                                        "<table class='table compact table-striped table-bordered table-hover'>" +
                                            "<thead>" +
                                                "<tr>" +
                                                    "<th>N° Examen</th>" +
                                                    "<th>Fecha</th>" +
                                                    "<th>Paciente</th>" +
                                                    "<th>Estudio</th>" +
                                                    "<th>Estado</th>" +
                                                    "<th>Encuesta</th>" +
                                                    "<th>Motivo Ampliación</th>" +
                                                "</tr>" +
                                            "</thead>" +
                                            "<tbody>" +
                                                data +
                                            "</tbody>" +
                                        "</table>" +
                                    "</div>" +
                                "</div>" +

                          "</div>";
                }
                else
                {
                    rsl = false;

                }
            }
            return Json(new { result = rsl, msj = msj }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult SetMedico(int examen, int tipo, string medico)
        {
            string msj = "";
            bool rsl = true;
            var _encu = (from en in db.Encuesta
                         where en.numeroexamen == examen
                         select en).SingleOrDefault();
            if (_encu != null)
            {
                if (tipo == 1)
                {
                    _encu.usu_reg_encu = medico;
                    using (RepositorioJhonEntities db1 = new RepositorioJhonEntities())
                    {
                        var rep = db1.ReporteContabilidad.SingleOrDefault(x => x.codigo == examen);
                        if (rep != null)
                        {
                            var _usu = db.USUARIO.Where(x => x.codigousuario == medico).SingleOrDefault();
                            rep.encuestador = _usu.ShortName;
                            db1.SaveChanges();
                        }
                    }
                }
                if (tipo == 2)
                {
                    _encu.usu_reg_super = medico;
                    using (RepositorioJhonEntities db1 = new RepositorioJhonEntities())
                    {
                        var rep = db1.ReporteContabilidad.SingleOrDefault(x => x.codigo == examen);
                        if (rep != null)
                        {
                            var _usu = db.USUARIO.Where(x => x.codigousuario == medico).SingleOrDefault();
                            rep.supervisor = _usu.ShortName;
                            db1.SaveChanges();
                        }
                    }
                }
                if (tipo == 3)
                {
                    _encu.usu_reg_tecno = medico;
                    _encu.usu_Solicitavalidacion = medico;
                    var ht = db.HONORARIOTECNOLOGO.Where(x => x.codigohonorariotecnologo == examen).SingleOrDefault();
                    if (ht != null)
                    {
                        var _usu = db.USUARIO.Where(x => x.codigousuario == medico).SingleOrDefault();
                        if (_usu != null)
                            ht.tecnologoturno = _usu.siglas;
                        using (RepositorioJhonEntities db1 = new RepositorioJhonEntities())
                        {
                            var rep = db1.ReporteContabilidad.SingleOrDefault(x => x.codigo == examen);
                            if (rep != null)
                            {
                                rep.tecnologo = _usu.ShortName;
                                db1.SaveChanges();
                            }
                        }
                    }


                }
                db.SaveChanges();
            }
            else
            {
                rsl = false;
                msj = "";
            }
            return Json(new { result = rsl, msj = msj }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult SolicitarImportacionExamen(int examen)
        {
            var tec = db.HONORARIOTECNOLOGO.SingleOrDefault(x => x.codigohonorariotecnologo == examen);
            if (tec != null)
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                string body = @"Recuperar Examen"
                    + "<br/><br/><br/> N° Examen:&nbsp;&nbsp;<b>" + examen + "</b>"
                    + "<br/> Equipo:&nbsp;&nbsp;<b>" + tec.EQUIPO.ShortDesc + "</b>"
                     + "<br/> Fecha Examen:&nbsp;&nbsp;<b>" + tec.fechaestudio.ToShortDateString() + "</b>"
                     + "<br/> Solicitante:&nbsp;&nbsp;<b>" + user.UserName + "</b>"
                     + "<br/> Fecha Solicitud:&nbsp;&nbsp;<b>" + DateTime.Now + "</b>";
                new Variable().sendCorreo("Recuperar Examen", "sistemas@resocentro.com", "", body, "");
            }
            return Json(true);
        }

        public ActionResult DeleteEncuesta()
        {
            return View();
        }

        public ActionResult GetCantItems(int tipo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            int cant = 0;
            try
            {
                switch (tipo)
                {
                    case 1://encuesta
                        cant = (from ea in db.EXAMENXATENCION
                                where ea.equipoAsignado == null
                                && ea.estadoestudio == "A"
                                  && (user.sucursales).Contains(ea.codigoestudio.Substring(1 - 1, 3)) //sucursales asignadas
                                select ea).ToList().Count();
                        break;
                    case 2: //supervision
                        cant = (from en in db.Encuesta
                                join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                                where en.SolicitarValidacion == true
                                && en.fec_ini_supervisa == null
                                && ea.estadoestudio == "A"
                                  && (user.sucursales).Contains(ea.codigoestudio.Substring(1 - 1, 3)) //sucursales asignadas
                                select en).ToList().Count();
                        break;
                    case 3: //tecnologo
                        cant = (from en in db.Encuesta
                                join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                                where en.estado == 1
                                && en.fec_ini_tecno == null
                                && ea.estadoestudio == "A"
                                  && (user.sucursales).Contains(ea.codigoestudio.Substring(1 - 1, 3)) //sucursales asignadas
                                select en).ToList().Count();
                        break;
                    case 4: //postproceso
                        cant = (from post in db.POSTPROCESO
                                join ea in db.EXAMENXATENCION on post.numeroestudio equals ea.codigo
                                where ea.estadoestudio != "X"
                                  && (new string[] { "N", "P" }).Contains(post.estado)
                                                            && post.tipo == "E"
                                select post).ToList().Count();
                        break;
                    case 5: //contraste
                        cant = (from cont in db.Contraste
                                join ea in db.EXAMENXATENCION on cont.numeroexamen equals ea.codigo
                                where ea.estadoestudio != "X"
                                  && cont.estado == 0
                                select cont).ToList().Count();
                        break;
                    case 6: //sedacion
                        cant = (from sed in db.Sedacion
                                join ea in db.EXAMENXATENCION on sed.numeroexamen equals ea.codigo
                                where ea.estadoestudio != "X"
                                  && sed.estado == 0
                                select sed).ToList().Count();
                        break;
                    default:
                        cant = 0;
                        break;
                }
            }
            catch (Exception)
            {
            }

            if (cant > 99)
                return Json("99+");
            else if (cant > 0)
                return Json(cant.ToString());
            else
                return Json("");

        }

        public ActionResult BorrarEncuesta(int examen, int tipo)
        {
            string msj = "";
            bool rsl = true;
            var _en = db.Encuesta.Where(x => x.numeroexamen == examen).Single();

            if (tipo == 0 || tipo == 2)//si el tipo es TODO o SUPERVISOR
            {
                if (_en.fec_supervisa != null)//si hay registro de Supervisor
                {
                    var _su = db.SupervisarEncuesta.Where(x => x.numeroexamen == examen).Single();
                    db.SupervisarEncuesta.Remove(_su);
                }
                _en.estado = 1;
                _en.fec_ini_supervisa = null;
                _en.fec_supervisa = null;
                _en.usu_reg_super = null;
            }

            if (tipo == 0 || tipo == 3)//si el tipo es TODO o TECNOLOGO
            {
                if (_en.fec_fin_tecno != null)//eliminamos los registro de tecnologo
                {
                    var _ht = db.HONORARIOTECNOLOGO.Where(x => x.codigohonorariotecnologo == examen).Single();
                    db.HONORARIOTECNOLOGO.Remove(_ht);
                }
                //ELIMINAMOS DEL INTEGRADO MYSQL
                //pacsdbEntities dbMysql = new pacsdbEntities();
                string _ex = examen.ToString() + ".1";
                new Variable().eliminarIntegrador(_ex);
                // mwl_item _int = (from x in dbMysql.mwl_item where x.sps_id == _ex select x).SingleOrDefault();
                // if (_int != null)
                //  dbMysql.mwl_item.Remove(_int);
                //ELIMINAMOS DE INTEGRADOR SQL
                var _inte = db.INTEGRACION.Where(x => x.numero_estudio == examen).SingleOrDefault();
                if (_inte != null)
                    db.INTEGRACION.Remove(_inte);
                _en.estado = 1;
                _en.fec_ini_tecno = null;
                _en.fec_fin_tecno = null;
                _en.usu_reg_tecno = null;
                _en.SolicitarValidacion = null;
                _en.fec_Solicitavalidacion = null;
                _en.usu_Solicitavalidacion = null;

            }
            if (tipo == 0 || tipo == 1)//si el tipo es TODO o ENCUESTA
            {
                var _exa = db.EXAMENXATENCION.Where(x => x.codigo == _en.numeroexamen).SingleOrDefault();
                if (_exa != null)
                {
                    var _pe = db.MED_PRE_ENTREVISTA.Where(x => x.numeroatencion == _exa.numeroatencion).SingleOrDefault();
                    if (_pe != null)
                        db.MED_PRE_ENTREVISTA.Remove(_pe);
                    if (_en.tipo_encu != null)
                        eliminarHistoriaEncuesta(examen, (EncuestaType)_en.tipo_encu);
                    db.Encuesta.Remove(_en);

                    _exa.equipoAsignado = null;
                }
            }


            var _ea = db.EXAMENXATENCION.Where(x => x.codigo == examen).Single();
            _ea.estadoestudio = "A";
            db.SaveChanges();
            return Json(new { result = rsl, msj = msj }, JsonRequestBehavior.DenyGet);
        }

        private void eliminarHistoriaEncuesta(int examen, EncuestaType tipoencuesta)
        {

            {
                switch (tipoencuesta)
                {
                    #region ELIMINAR HISTORIA CLINIcA
                    //CEREBRO
                    case EncuestaType.Cerebro:
                        var _enc1 = db.HISTORIA_CLINICA_CEREBRO.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc1 != null)
                            db.HISTORIA_CLINICA_CEREBRO.Remove(_enc1);
                        break;
                    //HOMBRO
                    case EncuestaType.HombroDerecho:
                    case EncuestaType.HombroIzquierdo:
                        var _enc2 = db.HISTORIA_CLINICA_HOMBRO.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc2 != null)
                            db.HISTORIA_CLINICA_HOMBRO.Remove(_enc2);
                        break;
                    //RODILLA
                    case EncuestaType.RodillaDerecha:
                    case EncuestaType.RodillaIzquierda:
                        var _enc3 = db.HISTORIA_CLINICA_RODILLA.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc3 != null)
                            db.HISTORIA_CLINICA_RODILLA.Remove(_enc3);
                        break;
                    //CLOMUNA A
                    case EncuestaType.ColumnaCervical:
                    case EncuestaType.ColumnaCervicoDorsal:
                        var _enc4 = db.HISTORIA_CLINICA_COLUMNA_A.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc4 != null)
                            db.HISTORIA_CLINICA_COLUMNA_A.Remove(_enc4);
                        break;
                    //CLOMUNA B
                    case EncuestaType.ColumnaDorsal:
                    case EncuestaType.ColumnaDorsoLumbar:
                    case EncuestaType.ColumnaLumboSacra:
                        var _enc5 = db.HISTORIA_CLINICA_COLUMNA_B.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc5 != null)
                            db.HISTORIA_CLINICA_COLUMNA_B.Remove(_enc5);
                        break;
                    //COLUMNA C
                    case EncuestaType.ColumnaSacroCoxis:
                        var _enc6 = db.HISTORIA_CLINICA_COLUMNA_C.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc6 != null)
                            db.HISTORIA_CLINICA_COLUMNA_C.Remove(_enc6);
                        break;
                    //CADERA
                    case EncuestaType.Caderaderecha:
                    case EncuestaType.Caderaizquierda:
                        var _enc7 = db.HISTORIA_CLINICA_CADERA.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc7 != null)
                            db.HISTORIA_CLINICA_CADERA.Remove(_enc7);
                        break;
                    //CODO
                    case EncuestaType.Cododerecha:
                    case EncuestaType.Codoizquierda:
                        var _enc8 = db.HISTORIA_CLINICA_CODO.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc8 != null)
                            db.HISTORIA_CLINICA_CODO.Remove(_enc8);
                        break;
                    //PIE
                    case EncuestaType.TobilloDerecho:
                    case EncuestaType.TobilloIzquierdo:
                    case EncuestaType.DedosDerecho:
                    case EncuestaType.DedosIzquierdo:
                    case EncuestaType.PieDerecho:
                    case EncuestaType.PieIzquierdo:
                        var _enc9 = db.HISTORIA_CLINICA_PIE.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc9 != null)
                            db.HISTORIA_CLINICA_PIE.Remove(_enc9);
                        break;
                    //MANO
                    case EncuestaType.MeniqueDerecho:
                    case EncuestaType.AnularDerecho:
                    case EncuestaType.MedioDerecho:
                    case EncuestaType.IndiceDerecho:
                    case EncuestaType.PulgarDerecho:
                    case EncuestaType.ManoDerecha:
                    case EncuestaType.MunecaDerecha:
                    case EncuestaType.MeniqueIzquierdo:
                    case EncuestaType.AnularIzquierdo:
                    case EncuestaType.MedioIzquierdo:
                    case EncuestaType.IndiceIzquierdo:
                    case EncuestaType.PulgarIzquierdo:
                    case EncuestaType.ManoIzquierdo:
                    case EncuestaType.MunecaIzquierdo:
                        var _enc10 = db.HISTORIA_CLINICA_MANO.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc10 != null)
                            db.HISTORIA_CLINICA_MANO.Remove(_enc10);
                        break;
                    //ONCOLOGICO
                    case EncuestaType.Oncologico:
                        var _enc11 = db.HISTORIA_CLINICA_ONCOLOGICO.Where(x => x.numeroestudio == examen).SingleOrDefault();
                        if (_enc11 != null)
                            db.HISTORIA_CLINICA_ONCOLOGICO.Remove(_enc11);
                        break;
                    //COLONOSCOPIA
                    case EncuestaType.Colonoscopia:
                        var _enc12 = db.HISTORIA_CLINICA_TEM_COLONOSCOPIA.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc12 != null)
                            db.HISTORIA_CLINICA_TEM_COLONOSCOPIA.Remove(_enc12);
                        break;
                    //ARTERIAS
                    case EncuestaType.AngiotemCoronario:
                        var _enc13 = db.HISTORIA_CLINICA_TEM_ARTERIAS.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc13 != null)
                            db.HISTORIA_CLINICA_TEM_ARTERIAS.Remove(_enc13);
                        break;
                    //AORTA
                    case EncuestaType.AngiotemAorta:
                        var _enc14 = db.HISTORIA_CLINICA_TEM_AORTA.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc14 != null)
                            db.HISTORIA_CLINICA_TEM_AORTA.Remove(_enc14);
                        break;
                    //CEREBRAL
                    case EncuestaType.AngiotemCerebral:
                        var _enc15 = db.HISTORIA_CLINICA_TEM_CEREBRAL.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc15 != null)
                            db.HISTORIA_CLINICA_TEM_CEREBRAL.Remove(_enc15);
                        break;
                    //MUSCULO ESQUELETICO
                    case EncuestaType.MusculoEsqueletico:
                        var _enc16 = db.HISTORIA_CLINICA_TEM_MUSCULO.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc16 != null)
                            db.HISTORIA_CLINICA_TEM_MUSCULO.Remove(_enc16);
                        break;
                    //ABDOMEN
                    case EncuestaType.Abdomen:
                        var _enc17 = db.HISTORIA_CLINICA_ABDOMEN.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc17 != null)
                            db.HISTORIA_CLINICA_ABDOMEN.Remove(_enc17);
                        break;
                    //EXTREMIDADES
                    case EncuestaType.BrazoDerecho:
                    case EncuestaType.AntebrazoDerecho:
                    case EncuestaType.MusloDerecho:
                    case EncuestaType.PiernaDerecho:
                    case EncuestaType.BrazoIzquierdo:
                    case EncuestaType.AntebrazoIzquierdo:
                    case EncuestaType.MusloIzquierdo:
                    case EncuestaType.PiernaIzquierdo:
                        var _enc18 = db.HISTORIA_CLINICA_EXTREMIDAD.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc18 != null)
                            db.HISTORIA_CLINICA_EXTREMIDAD.Remove(_enc18);
                        break;
                    //NEUROCEREBRO
                    case EncuestaType.NeuroCerebro:
                        var _enc19 = db.HISTORIA_CLINICA_TEM_NEUROCEREBRO.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc19 != null)
                            db.HISTORIA_CLINICA_TEM_NEUROCEREBRO.Remove(_enc19);
                        break;
                    //GENERICA
                    case EncuestaType.Generica:
                        var _enc20 = db.HISTORIA_CLINICA_TEM_GENERICA.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc20 != null)
                            db.HISTORIA_CLINICA_TEM_GENERICA.Remove(_enc20);
                        break;
                    //MAMA
                    case EncuestaType.MamaDerecha:
                    case EncuestaType.MamaIzquierda:
                        var _enc21 = db.HISTORIA_CLINICA_MAMA.Where(x => x.numeroexamen == examen).SingleOrDefault();
                        if (_enc21 != null)
                            db.HISTORIA_CLINICA_MAMA.Remove(_enc21);
                        break;
                    default:
                        break;
                    #endregion
                }
                db.SaveChanges();
            }
        }

        //OBTENER LISTA DE CANCELACIONES

        public ActionResult getCancelaciones()
        {
            Variable _u = new Variable();
            return Json(new SelectList(_u.getCancelaciones(), "codigo", "nombre"));
        }


        public ActionResult MoveEncuesta()
        {
            return View();
        }
        public ActionResult CambioEncuesta(int examenold, int examennew)
        {
            var exaold = db.Encuesta.SingleOrDefault(x => x.numeroexamen == examenold);
            var exanew = db.EXAMENXATENCION.SingleOrDefault(x => x.codigo == examennew);
            if (exaold != null && exanew != null)
            {
                List<sp_mover_encuesta_Result> lista = db.sp_mover_encuesta(examennew, examenold).ToList();
                return Json(new { result = true, msj = "" }, JsonRequestBehavior.DenyGet);
            }
            else
                return Json(new { result = false, msj = "No existen uno de los N° de examenes ingresados" }, JsonRequestBehavior.DenyGet);
        }
        public ActionResult DeleteCalificacion()
        {
            return View();
        }
        public ActionResult EliminarCalificacion(int examen, int tipo)
        {
            var _califi = db.SupervisarEncuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            try
            {
                if (_califi != null)
                {
                    if (tipo == 1)
                    {
                        _califi.p1_informante = null;
                        _califi.p1_1_informante = null;
                        _califi.p2_informante = null;
                        _califi.p2_1_informante = null;
                        _califi.p3_informante = null;
                        _califi.p3_1_informante = null;
                        _califi.p4_informante = null;
                        _califi.p4_1_informante = null;
                        _califi.p5_1_informante = null;
                        _califi.p5_1_informante = null;
                        _califi.diagnostico_info = null;
                        _califi.diagnostico_inf = null;
                        _califi.usu_reg_inf = null;
                        _califi.fec_reg_inf = null;
                    }
                    else
                    {
                        _califi.p1_validador = null;
                        _califi.p1_1_validador = null;
                        _califi.p2_validador = null;
                        _califi.p2_1_validador = null;
                        _califi.p3_validador = null;
                        _califi.p3_1_validador = null;
                        _califi.p4_ivalidador = null;
                        _califi.p4_1_validador = null;
                        _califi.p5_validador = null;
                        _califi.p5_1_validador = null;
                        _califi.diagnostico_vali = null;
                        _califi.diagnostico_val = null;
                        _califi.usu_reg_val = null;
                        _califi.fec_reg_val = null;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json(new { result = false, msj = "" }, JsonRequestBehavior.DenyGet);
            }
            return Json(new { result = true, msj = "" }, JsonRequestBehavior.DenyGet);
        }
        public ActionResult SolucionarProblemasEncuesta()
        {
            return View();
        }

        public ActionResult SolucionarEncuesta(int examen)
        {
            string msj = SolucionarProblemasEncuestaInternal(examen);
            return Json(new { result = true, msj = msj }, JsonRequestBehavior.DenyGet);
        }

        public string SolucionarProblemasEncuestaInternal(int examen)
        {
            var _examen = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            string msj = "";
            int count;
            if (_examen != null)
            {
                //buscamos la preentrevista
                var _pre = db.MED_PRE_ENTREVISTA.Where(x => x.numeroatencion == _examen.numeroatencion).ToList();
                if (_pre.Count > 0)
                {
                    msj += "- Existe registro de Pre-Entrevista<br/>";
                    if (_pre.Count > 1)
                    {
                        msj += "- Existe mas de 1 registro de Pre-Entrevista<br/>";
                        //Eliminamos las pre entrevistas si tiene mas de 1
                        count = 0;
                        foreach (var item in _pre)
                        {
                            if (count > 0)
                            {
                                db.MED_PRE_ENTREVISTA.Remove(item);
                                msj += "- <code>Se Elimino el registro de Pre-Entrevista N°" + item.numeroatencion + "</code><br/>";
                            }
                            count++;
                        }
                    }
                }
                else
                    msj += "-<code> No existe ningun registro de Pre-Entrevista</code><br/>";
                //verificamos si tiene solo una encuesta
                var _encus = db.Encuesta.Where(x => x.numeroexamen == _examen.codigo).ToList();
                if (_encus.Count > 0)
                {
                    msj += "- Existe registro de Encuesta<br/>";
                    if (_encus.Count > 1)
                    {
                        msj += "- Existe más de 1 registro de Encuesta<br/>";
                        //eliminamos las encuestas si tiene mas de 1
                        count = 0;
                        foreach (var item in _encus)
                        {
                            if (count > 0)
                            {
                                db.Encuesta.Remove(item);
                                msj += "- <code>Se Elimino la Encuesta N°" + item.idencuesta + " con el Examen N°" + item.numeroexamen + "</code><br/>";
                            }
                            count++;
                        }
                    }
                    //verificamos si tiene tipo de encuesta
                    foreach (var item in _encus)
                    {
                        if (item.tipo_encu != null)
                        {
                            msj += "- Tipo de Encuesta " + item.tipo_encu + " " + Enum.GetName(typeof(EncuestaType), item.tipo_encu) + "<br/>";
                            //buscamos la encuesta segun el tipo
                            bool isExist = false;
                            switch ((EncuestaType)item.tipo_encu)
                            {
                                #region Tipo Encuestas
                                case EncuestaType.Cerebro:
                                    var _historia = db.HISTORIA_CLINICA_CEREBRO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_historia.Count > 0)
                                    {
                                        isExist = true;
                                        if (_historia.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _historia)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_CEREBRO.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Cerebro<br/>";
                                        }
                                    }
                                    else
                                        msj += "- <code>No existe ningun Historia de Cerebro</code><br/>";
                                    break;
                                case EncuestaType.HombroIzquierdo:
                                case EncuestaType.HombroDerecho:
                                    var _historia1 = db.HISTORIA_CLINICA_HOMBRO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_historia1.Count > 0)
                                    {
                                        isExist = true;
                                        if (_historia1.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _historia1)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_HOMBRO.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Hombro<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Hombro<br/>";
                                    break;
                                case EncuestaType.RodillaIzquierda:
                                case EncuestaType.RodillaDerecha:
                                    var _rodillas = db.HISTORIA_CLINICA_RODILLA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_rodillas.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Rodilla<br/>";
                                        if (_rodillas.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _rodillas)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_RODILLA.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Rodilla<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Rodilla<br/>";
                                    break;
                                case EncuestaType.ColumnaCervical:
                                case EncuestaType.ColumnaCervicoDorsal:
                                    var _ColumnasA = db.HISTORIA_CLINICA_COLUMNA_A.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_ColumnasA.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Columnas A<br/>";
                                        if (_ColumnasA.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _ColumnasA)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_COLUMNA_A.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Columnas A<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Columnas A<br/>";
                                    break;
                                case EncuestaType.ColumnaDorsal:
                                case EncuestaType.ColumnaDorsoLumbar:
                                case EncuestaType.ColumnaLumboSacra:
                                    var _ColumnasB = db.HISTORIA_CLINICA_COLUMNA_B.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_ColumnasB.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Columnas B<br/>";
                                        if (_ColumnasB.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _ColumnasB)
                                            {
                                                if (count > 0)
                                                {

                                                    db.HISTORIA_CLINICA_COLUMNA_B.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Columnas B<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Columnas B<br/>";
                                    break;
                                case EncuestaType.ColumnaSacroCoxis:
                                    var _ColumnasC = db.HISTORIA_CLINICA_COLUMNA_C.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_ColumnasC.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Columnas C<br/>";
                                        if (_ColumnasC.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _ColumnasC)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_COLUMNA_C.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Columnas C<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Columnas C<br/>";
                                    break;
                                case EncuestaType.Caderaderecha:
                                case EncuestaType.Caderaizquierda:
                                    var _Caderas = db.HISTORIA_CLINICA_CADERA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Caderas.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Cadera<br/>";
                                        if (_Caderas.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Caderas)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_CADERA.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Cadera<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Cadera<br/>";
                                    break;
                                case EncuestaType.Cododerecha:
                                case EncuestaType.Codoizquierda:
                                    var _Codos = db.HISTORIA_CLINICA_CODO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Codos.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Codos<br/>";
                                        if (_Codos.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Codos)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_CODO.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Codos<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Codos<br/>";
                                    break;
                                case EncuestaType.TobilloIzquierdo:
                                case EncuestaType.PieIzquierdo:
                                case EncuestaType.DedosIzquierdo:
                                case EncuestaType.TobilloDerecho:
                                case EncuestaType.PieDerecho:
                                case EncuestaType.DedosDerecho:
                                    var _Pies = db.HISTORIA_CLINICA_PIE.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Pies.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Pie<br/>";
                                        if (_Pies.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Pies)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_PIE.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Pie<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Pie<br/>";
                                    break;
                                case EncuestaType.MeniqueDerecho:
                                case EncuestaType.AnularDerecho:
                                case EncuestaType.MedioDerecho:
                                case EncuestaType.IndiceDerecho:
                                case EncuestaType.PulgarDerecho:
                                case EncuestaType.ManoDerecha:
                                case EncuestaType.MunecaDerecha:
                                case EncuestaType.MeniqueIzquierdo:
                                case EncuestaType.AnularIzquierdo:
                                case EncuestaType.MedioIzquierdo:
                                case EncuestaType.IndiceIzquierdo:
                                case EncuestaType.PulgarIzquierdo:
                                case EncuestaType.ManoIzquierdo:
                                case EncuestaType.MunecaIzquierdo:
                                    var _Manos = db.HISTORIA_CLINICA_MANO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Manos.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Mano<br/>";
                                        if (_Manos.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Manos)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_MANO.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Mano<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Mano<br/>";
                                    break;
                                case EncuestaType.Abdomen:
                                    var _Abdomenes = db.HISTORIA_CLINICA_ABDOMEN.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Abdomenes.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Abdomen<br/>";
                                        if (_Abdomenes.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Abdomenes)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_ABDOMEN.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Abdomen<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Abdomen<br/>";
                                    break;
                                case EncuestaType.BrazoDerecho:
                                case EncuestaType.AntebrazoDerecho:
                                case EncuestaType.MusloDerecho:
                                case EncuestaType.PiernaDerecho:
                                case EncuestaType.BrazoIzquierdo:
                                case EncuestaType.AntebrazoIzquierdo:
                                case EncuestaType.MusloIzquierdo:
                                case EncuestaType.PiernaIzquierdo:
                                    var _Extremidades = db.HISTORIA_CLINICA_EXTREMIDAD.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Extremidades.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Extremidad<br/>";
                                        if (_Extremidades.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Extremidades)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_EXTREMIDAD.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Extremidad<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Extremidad<br/>";
                                    break;
                                case EncuestaType.MamaDerecha:
                                case EncuestaType.MamaIzquierda:
                                    var _Mamas = db.HISTORIA_CLINICA_MAMA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Mamas.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Mama<br/>";
                                        if (_Mamas.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Mamas)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_MAMA.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Mama<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Mama<br/>";
                                    break;
                                case EncuestaType.SupervisarEncuesta:
                                    break;
                                case EncuestaType.Oncologico:
                                    var _Oncos = db.HISTORIA_CLINICA_ONCOLOGICO.Where(x => x.numeroestudio == item.numeroexamen).ToList();
                                    if (_Oncos.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Oncologico<br/>";
                                        if (_Oncos.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Oncos)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_ONCOLOGICO.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Oncologico<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Oncologico<br/>";
                                    break;
                                case EncuestaType.Colonoscopia:
                                    var _Colonos = db.HISTORIA_CLINICA_TEM_COLONOSCOPIA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Colonos.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Colonoscopia<br/>";
                                        if (_Colonos.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Colonos)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_TEM_COLONOSCOPIA.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Colonoscopia<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Colonoscopia<br/>";
                                    break;
                                case EncuestaType.AngiotemCoronario:
                                    var _coronos = db.HISTORIA_CLINICA_TEM_ARTERIAS.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_coronos.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Coronarias<br/>";
                                        if (_coronos.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _coronos)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_TEM_ARTERIAS.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Coronarias<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Coronarias<br/>";
                                    break;
                                case EncuestaType.AngiotemAorta:
                                    var _Aortas = db.HISTORIA_CLINICA_TEM_AORTA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Aortas.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de Aorta<br/>";
                                        if (_Aortas.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Aortas)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_TEM_AORTA.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de Aorta<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de Aorta<br/>";
                                    break;
                                case EncuestaType.AngiotemCerebral:
                                    var _Cerebrales = db.HISTORIA_CLINICA_TEM_CEREBRO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Cerebrales.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de TEM CEREBRO<br/>";
                                        if (_Cerebrales.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Cerebrales)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_TEM_CEREBRO.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de TEM CEREBRO<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de TEM CEREBRO<br/>";
                                    break;
                                case EncuestaType.MusculoEsqueletico:
                                    var _Musculos = db.HISTORIA_CLINICA_TEM_MUSCULO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Musculos.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de TEM Musuculo<br/>";
                                        if (_Musculos.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Musculos)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_TEM_MUSCULO.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de TEM Musuculo<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de TEM Musuculo<br/>";
                                    break;
                                case EncuestaType.NeuroCerebro:
                                    var _Neuros = db.HISTORIA_CLINICA_TEM_NEUROCEREBRO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Neuros.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de TEM Neurocerebro<br/>";
                                        if (_Neuros.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Neuros)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_TEM_NEUROCEREBRO.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de TEM Neurocerebro<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de TEM Neurocerebro<br/>";
                                    break;
                                case EncuestaType.Generica:
                                    var _Genericas = db.HISTORIA_CLINICA_TEM_GENERICA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                    if (_Genericas.Count > 0)
                                    {
                                        isExist = true;
                                        msj += "- Existe un Historia de TEM Generica<br/>";
                                        if (_Genericas.Count > 1)
                                        {
                                            count = 0;
                                            foreach (var _his in _Genericas)
                                            {
                                                if (count > 0)
                                                {
                                                    db.HISTORIA_CLINICA_TEM_GENERICA.Remove(_his);
                                                }
                                                count++;
                                            }
                                            msj += "- Existe " + count + " registro de Historia de TEM Generica<br/>";
                                        }
                                    }
                                    else
                                        msj += "- No existe ningun Historia de TEM Generica<br/>";
                                    break;
                                case EncuestaType.ProtocoloTEM:
                                    break;
                                case EncuestaType.EncuestaVacia:
                                    break;
                                case EncuestaType.EncuestaAmpliacion:
                                    break;
                                default:
                                    break;
                                #endregion
                            }
                            if (isExist == false)
                            {
                                msj += "- No existe tipo de Encuesta<br/>";
                                #region Tipo Encuestas
                                // case EncuestaType.Cerebro:
                                var _historia = db.HISTORIA_CLINICA_CEREBRO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_historia.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.Cerebro;
                                    msj += "- Se cambio el tipo de encuesta a Cerebro<br/>";
                                    break;
                                }
                                // case EncuestaType.HombroIzquierdo:
                                // case EncuestaType.HombroDerecho:
                                if (db.HISTORIA_CLINICA_HOMBRO.Where(x => x.numeroexamen == item.numeroexamen).ToList().Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.HombroDerecho;
                                    msj += "- Se cambio el tipo de encuesta a HombroDerecho<br/>";
                                    break;
                                }
                                // case EncuestaType.RodillaIzquierda:
                                // case EncuestaType.RodillaDerecha:
                                var _rodillas = db.HISTORIA_CLINICA_RODILLA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_rodillas.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.RodillaDerecha;
                                    msj += "- Se cambio el tipo de encuesta a RodillaDerecha<br/>";
                                    break;
                                }
                                // case EncuestaType.ColumnaCervical:
                                // case EncuestaType.ColumnaCervicoDorsal:
                                var _ColumnasA = db.HISTORIA_CLINICA_COLUMNA_A.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_ColumnasA.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.ColumnaCervical;
                                    msj += "- Se cambio el tipo de encuesta a ColumnaCervical<br/>";
                                    break;
                                }
                                // case EncuestaType.ColumnaDorsal:
                                // case EncuestaType.ColumnaDorsoLumbar:
                                // case EncuestaType.ColumnaLumboSacra:
                                var _ColumnasB = db.HISTORIA_CLINICA_COLUMNA_B.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_ColumnasB.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.ColumnaDorsoLumbar;
                                    msj += "- Se cambio el tipo de encuesta a ColumnaDorsoLumbar<br/>";
                                    break;
                                }
                                // case EncuestaType.ColumnaSacroCoxis:
                                var _ColumnasC = db.HISTORIA_CLINICA_COLUMNA_C.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_ColumnasC.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.ColumnaSacroCoxis;
                                    msj += "- Se cambio el tipo de encuesta a ColumnaSacroCoxis<br/>";
                                    break;
                                }
                                // case EncuestaType.Caderaderecha:
                                // case EncuestaType.Caderaizquierda:
                                var _Caderas = db.HISTORIA_CLINICA_CADERA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Caderas.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.Caderaderecha;
                                    msj += "- Se cambio el tipo de encuesta a Caderaderecha<br/>";
                                    break;
                                }
                                // case EncuestaType.Cododerecha:
                                // case EncuestaType.Codoizquierda:
                                var _Codos = db.HISTORIA_CLINICA_CODO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Codos.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.Cododerecha;
                                    msj += "- Se cambio el tipo de encuesta a Cododerecha<br/>";
                                    break;
                                }
                                // case EncuestaType.TobilloIzquierdo:
                                // case EncuestaType.PieIzquierdo:
                                // case EncuestaType.DedosIzquierdo:
                                // case EncuestaType.TobilloDerecho:
                                // case EncuestaType.PieDerecho:
                                // case EncuestaType.DedosDerecho:
                                var _Pies = db.HISTORIA_CLINICA_PIE.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Pies.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.PieDerecho;
                                    msj += "- Se cambio el tipo de encuesta a PieDerecho<br/>";
                                    break;
                                }
                                // case EncuestaType.MeniqueDerecho:
                                // case EncuestaType.AnularDerecho:
                                // case EncuestaType.MedioDerecho:
                                // case EncuestaType.IndiceDerecho:
                                // case EncuestaType.PulgarDerecho:
                                // case EncuestaType.ManoDerecha:
                                // case EncuestaType.MunecaDerecha:
                                // case EncuestaType.MeniqueIzquierdo:
                                // case EncuestaType.AnularIzquierdo:
                                // case EncuestaType.MedioIzquierdo:
                                // case EncuestaType.IndiceIzquierdo:
                                // case EncuestaType.PulgarIzquierdo:
                                // case EncuestaType.ManoIzquierdo:
                                // case EncuestaType.MunecaIzquierdo:
                                var _Manos = db.HISTORIA_CLINICA_MANO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Manos.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.ManoDerecha;
                                    msj += "- Se cambio el tipo de encuesta a ManoDerecha<br/>";
                                    break;
                                }
                                // case EncuestaType.Abdomen:
                                var _Abdomenes = db.HISTORIA_CLINICA_ABDOMEN.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Abdomenes.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.Abdomen;
                                    msj += "- Se cambio el tipo de encuesta a Abdomen<br/>";
                                    break;
                                }
                                // case EncuestaType.BrazoDerecho:
                                // case EncuestaType.AntebrazoDerecho:
                                // case EncuestaType.MusloDerecho:
                                // case EncuestaType.PiernaDerecho:
                                // case EncuestaType.BrazoIzquierdo:
                                // case EncuestaType.AntebrazoIzquierdo:
                                // case EncuestaType.MusloIzquierdo:
                                // case EncuestaType.PiernaIzquierdo:
                                var _Extremidades = db.HISTORIA_CLINICA_EXTREMIDAD.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Extremidades.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.MusloDerecho;
                                    msj += "- Se cambio el tipo de encuesta a MusloDerecho<br/>";
                                    break;
                                }
                                // case EncuestaType.MamaDerecha:
                                // case EncuestaType.MamaIzquierda:
                                var _Mamas = db.HISTORIA_CLINICA_MAMA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Mamas.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.MamaDerecha;
                                    msj += "- Se cambio el tipo de encuesta a MamaDerecha<br/>";
                                    break;
                                }
                                // case EncuestaType.Oncologico:
                                var _Oncos = db.HISTORIA_CLINICA_ONCOLOGICO.Where(x => x.numeroestudio == item.numeroexamen).ToList();
                                if (_Oncos.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.Oncologico;
                                    msj += "- Se cambio el tipo de encuesta a Oncologico<br/>";
                                    break;
                                }
                                // case EncuestaType.Colonoscopia:
                                var _Colonos = db.HISTORIA_CLINICA_TEM_COLONOSCOPIA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Colonos.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.Colonoscopia;
                                    msj += "- Se cambio el tipo de encuesta a Colonoscopia<br/>";
                                    break;
                                }
                                // case EncuestaType.AngiotemCoronario:
                                var _coronos = db.HISTORIA_CLINICA_TEM_ARTERIAS.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_coronos.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.AngiotemCoronario;
                                    msj += "- Se cambio el tipo de encuesta a AngiotemCoronario<br/>";
                                    break;
                                }
                                // case EncuestaType.AngiotemAorta:
                                var _Aortas = db.HISTORIA_CLINICA_TEM_AORTA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Aortas.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.AngiotemAorta;
                                    msj += "- Se cambio el tipo de encuesta a AngiotemAorta<br/>";
                                    break;
                                }
                                // case EncuestaType.AngiotemCerebral:
                                var _Cerebrales = db.HISTORIA_CLINICA_TEM_CEREBRO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Cerebrales.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.AngiotemCerebral;
                                    msj += "- Se cambio el tipo de encuesta a AngiotemCerebral<br/>";
                                    break;
                                }
                                // case EncuestaType.MusculoEsqueletico:
                                var _Musculos = db.HISTORIA_CLINICA_TEM_MUSCULO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Musculos.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.MusculoEsqueletico;
                                    msj += "- Se cambio el tipo de encuesta a MusculoEsqueletico<br/>";
                                    break;
                                }
                                // case EncuestaType.NeuroCerebro:
                                var _Neuros = db.HISTORIA_CLINICA_TEM_NEUROCEREBRO.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Neuros.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.NeuroCerebro;
                                    msj += "- Se cambio el tipo de encuesta a NeuroCerebro<br/>";
                                    break;
                                }
                                // case EncuestaType.Generica:
                                var _Genericas = db.HISTORIA_CLINICA_TEM_GENERICA.Where(x => x.numeroexamen == item.numeroexamen).ToList();
                                if (_Genericas.Count > 0)
                                {
                                    item.tipo_encu = (int)EncuestaType.Generica;
                                    msj += "- Se cambio el tipo de encuesta a Generica<br/>";
                                    break;
                                }

                                #endregion
                            }
                        }
                        else
                            msj += "- No existe tipo de Encuesta<br/>";
                    }
                }
                else
                    msj += "- No existe registro de Encuesta<br/>";

                //verificamos si tiene solo una supervision
                var _supers = db.SupervisarEncuesta.Where(x => x.numeroexamen == _examen.codigo).ToList();
                if (_supers.Count > 0)
                {
                    msj += "- Existe registro de Supervisión<br/>";
                    if (_supers.Count > 1)
                    {
                        msj += "- Existe más de 1 registro de Supervisión<br/>";
                        //eliminamos las encuestas si tiene mas de 1
                        count = 0;
                        SupervisarEncuesta oldSuper = new SupervisarEncuesta();
                        foreach (var item in _supers)
                        {
                            if (count == 0)
                            {
                                oldSuper = item;
                            }
                            else
                            {



                                if (oldSuper.p1 != null)
                                    oldSuper.p1 = item.p1;
                                if (oldSuper.p1_1 != null)
                                    oldSuper.p1_1 = item.p1_1;

                                if (oldSuper.p2 != null)
                                    oldSuper.p2 = item.p2;
                                if (oldSuper.p2_1 != null)
                                    oldSuper.p2_1 = item.p2_1;

                                if (oldSuper.p3 != null)
                                    oldSuper.p3 = item.p3;
                                if (oldSuper.p3 != null)
                                    oldSuper.p3_1 = item.p3_1;

                                if (oldSuper.p4 != null)
                                    oldSuper.p4 = item.p4;
                                if (oldSuper.p4_1 != null)
                                    oldSuper.p4_1 = item.p4_1;

                                if (oldSuper.anotaciones == null || oldSuper.anotaciones == "")
                                    oldSuper.anotaciones = item.anotaciones;

                                if (oldSuper.usu_reg != null)
                                    oldSuper.usu_reg = item.usu_reg;
                                if (oldSuper.fecha_reg != null)
                                    oldSuper.fecha_reg = item.fecha_reg;

                                if (oldSuper.fecha_termino != null)
                                    oldSuper.fecha_termino = item.fecha_termino;

                                if (oldSuper.superviso_encuesta != null)
                                    oldSuper.superviso_encuesta = item.superviso_encuesta;

                                if (oldSuper.superviso_tecnologo != null)
                                    oldSuper.superviso_tecnologo = item.superviso_tecnologo;

                                if (oldSuper.estado != null)
                                    oldSuper.estado = item.estado;

                                if (oldSuper.superviso_imagenes != null)
                                    oldSuper.superviso_imagenes = item.superviso_imagenes;

                                if (oldSuper.isInEcnuesta != null)
                                    oldSuper.isInEcnuesta = item.isInEcnuesta;

                                if (oldSuper.isInImagenes != null)
                                    oldSuper.isInImagenes = item.isInImagenes;

                                if (oldSuper.p1_informante != null)
                                    oldSuper.p1_informante = item.p1_informante;

                                if (oldSuper.p2_informante != null)
                                    oldSuper.p2_informante = item.p2_informante;

                                if (oldSuper.p3_informante != null)
                                    oldSuper.p3_informante = item.p3_informante;

                                if (oldSuper.p4_informante != null)
                                    oldSuper.p4_informante = item.p4_informante;

                                if (oldSuper.p1_1_informante != null)
                                    oldSuper.p1_1_informante = item.p1_1_informante;

                                if (oldSuper.p2_1_informante != null)
                                    oldSuper.p2_1_informante = item.p2_1_informante;

                                if (oldSuper.p3_1_informante != null)
                                    oldSuper.p3_1_informante = item.p3_1_informante;

                                if (oldSuper.p4_1_informante != null)
                                    oldSuper.p4_1_informante = item.p4_1_informante;

                                if (oldSuper.fec_reg_inf != null)
                                    oldSuper.fec_reg_inf = item.fec_reg_inf;

                                if (oldSuper.usu_reg_inf != null)
                                    oldSuper.usu_reg_inf = item.usu_reg_inf;

                                if (oldSuper.p1_validador != null)
                                    oldSuper.p1_validador = item.p1_validador;

                                if (oldSuper.p2_validador != null)
                                    oldSuper.p2_validador = item.p2_validador;

                                if (oldSuper.p3_validador != null)
                                    oldSuper.p3_validador = item.p3_validador;

                                if (oldSuper.p4_ivalidador != null)
                                    oldSuper.p4_ivalidador = item.p4_ivalidador;

                                if (oldSuper.p1_1_validador != null)
                                    oldSuper.p1_1_validador = item.p1_1_validador;

                                if (oldSuper.p2_1_validador != null)
                                    oldSuper.p2_1_validador = item.p2_1_validador;

                                if (oldSuper.p3_1_validador != null)
                                    oldSuper.p3_1_validador = item.p3_1_validador;

                                if (oldSuper.p4_1_validador != null)
                                    oldSuper.p4_1_validador = item.p4_1_validador;

                                if (oldSuper.fec_reg_val != null)
                                    oldSuper.fec_reg_val = item.fec_reg_val;

                                if (oldSuper.usu_reg_val != null)
                                    oldSuper.usu_reg_val = item.usu_reg_val;

                                if (oldSuper.diagnostico_inf != null)
                                    oldSuper.diagnostico_inf = item.diagnostico_inf;

                                if (oldSuper.diagnostico_info != null)
                                    oldSuper.diagnostico_info = item.diagnostico_info;

                                if (oldSuper.diagnostico_val != null)
                                    oldSuper.diagnostico_val = item.diagnostico_val;

                                if (oldSuper.diagnostico_vali != null)
                                    oldSuper.diagnostico_vali = item.diagnostico_vali;

                                if (oldSuper.p5 != null)
                                    oldSuper.p5 = item.p5;

                                if (oldSuper.p5_1 != null)
                                    oldSuper.p5_1 = item.p5_1;

                                if (oldSuper.p5_informante != null)
                                    oldSuper.p5_informante = item.p5_informante;

                                if (oldSuper.p5_1_informante != null)
                                    oldSuper.p5_1_informante = item.p5_1_informante;

                                if (oldSuper.p5_validador != null)
                                    oldSuper.p5_validador = item.p5_validador;

                                if (oldSuper.p5_1_validador != null)
                                    oldSuper.p5_1_validador = item.p5_1_validador;

                                if (oldSuper.isSendEmailProcessSuperivor == false)
                                    oldSuper.isSendEmailProcessSuperivor = item.isSendEmailProcessSuperivor;

                                if (oldSuper.isSendEmailProcessInformante == false)
                                    oldSuper.isSendEmailProcessInformante = item.isSendEmailProcessInformante;

                                if (oldSuper.isSendEmailProcessValidador == false)
                                    oldSuper.isSendEmailProcessValidador = item.isSendEmailProcessValidador;

                                db.SupervisarEncuesta.Remove(item);
                                msj += "- Se Elimino el registro de Supervisión N°" + item.idsupervisar + "<br/>";
                            }

                            count++;
                        }
                    }
                }
                else
                    msj += "- <code>No existe registro de Supervisión</code><br/>";



            }

            db.SaveChanges();
            return msj;
        }


        public ActionResult LlamarPersonalMedico(string tipo, string ubicacion)
        {
            using (RepositorioJhonEntities db1 = new RepositorioJhonEntities())
            {
                try
                {
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    AlertasEncuestas ae = new AlertasEncuestas();
                    ae.fecha = DateTime.Now;
                    ae.usuario = user.UserName;
                    ae.ubicacion_solicitante = ubicacion;
                    ae.tipo_solicitud = tipo;
                    ae.isReproducido = false;
                    db1.AlertasEncuestas.Add(ae);
                    db1.SaveChanges();
                }
                catch (Exception) { }
            }
            return Json(true);
        }

    }

}

