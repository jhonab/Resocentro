using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Encuesta.Controllers.Encuesta
{
    [Authorize(Roles = "1,2,5")]
    public class CancelarEncuestaController : Controller
    {
        //
        // GET: /CancelarEncuesta/
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        public ActionResult Cancelar()
        {
            return View();
        }

        public ActionResult GetEncuesta(int examen)
        {
            string msj = "";
            bool rsl = true;
            var _encu = (from ea in db.EXAMENXATENCION
                         join p in db.PACIENTE on ea.codigopaciente equals p.codigopaciente
                         join en in db.Encuesta on ea.codigo equals en.numeroexamen into en_join
                         from en in en_join.DefaultIfEmpty()
                         join us_e in db.USUARIO on en.usu_reg_encu equals us_e.codigousuario into ue_e_join
                         from us_e in ue_e_join.DefaultIfEmpty()
                         join us_s in db.USUARIO on en.usu_reg_super equals us_s.codigousuario into us_s_join
                         from us_s in us_s_join.DefaultIfEmpty()
                         join us_t in db.USUARIO on en.usu_reg_tecno equals us_t.codigousuario into us_t_join
                         from us_t in us_t_join.DefaultIfEmpty()
                         where ea.codigo == examen 
                         && ea.estadoestudio=="A"
                         select new
                         {
                             nexamen = ea.codigo,
                             fecha = ea.ATENCION.fechayhora,
                             paciente = p.apellidos + " " + p.nombres,
                             estudio = ea.ESTUDIO.nombreestudio,
                             encuestador = us_e.ShortName,
                             tecnologo = us_t.ShortName,
                             supervisor = us_s.ShortName
                         }).ToList();
            if (_encu.Count > 0)
            {
                string data = "";
                foreach (var item in _encu)
                {
                    data +=
                   "<tr>" +
                       "<td>" + item.nexamen + "</td>" +
                       "<td>" + item.fecha.ToShortDateString() + "</td>" +
                       "<td>" + item.paciente + "</td>" +
                       "<td>" + item.estudio + "</td>" +
                       "<td>" + item.encuestador + "</td>" +
                       "<td>" + item.tecnologo + "</td>" +
                       "<td>" + item.supervisor + "</td>" +
                       "<td><button onclick='verEncuesta(" + item.nexamen + ")'  class='btn btn-outline btn-info' ><i class='fa fa-file-text-o fa-2x'></i></button><br/><button onclick='ModalCancelarExamen(" + item.nexamen + ")'  class='btn btn-outline btn-danger' ><i class='fa fa-times-circle-o fa-2x'></i> </button></td>" +
                   "</tr>";
                }
                msj =
                    "<table class='table table-striped table-bordered table-hover'>" +
                        "<thead>" +
                            "<tr>" +
                                "<th>N° Examen</th>" +
                                "<th>Fecha</th>" +
                                "<th>Paciente</th>" +
                                "<th>Estudio</th>" +
                                "<th>Encuestador</th>" +
                                "<th>Tecnologo</th>" +
                                "<th>Supervisor</th>" +
                                "<th>Encuesta</th>" +
                            "</tr>" +
                        "</thead>" +
                        "<tbody>" +
                                 data +
                        "</tbody>";
            }
            else
            {
                rsl = false;
                msj = "";
            }
            return Json(new { result = rsl, msj = msj }, JsonRequestBehavior.DenyGet);
        }

        
    }
}
