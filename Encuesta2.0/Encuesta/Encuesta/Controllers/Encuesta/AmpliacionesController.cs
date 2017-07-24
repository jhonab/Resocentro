using Encuesta.Member;
using Encuesta.Models;
using Encuesta.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Encuesta
{
    [Authorize(Roles = "1")]
    public class AmpliacionesController : Controller
    {
        //
        // GET: /Ampliaciones/
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        public ActionResult ListaEspera()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _fecha = DateTime.Now;
            var lista = (from ec in db.EXAMENXCITA
                         where
                         ec.CITA.fechareserva.Year == _fecha.Year
                         && ec.CITA.fechareserva.Month == _fecha.Month
                         && ec.CITA.fechareserva.Day == _fecha.Day
                         && (user.sucursales).Contains(ec.codigoestudio.Substring(1 - 1, 3)) //sucursales asignadas
                         && ec.ESTUDIO.nombreestudio.Contains("Ampliaci")
                         && (new string[]{"C","K","A"}).Contains(ec.estadoestudio)
                         select new AmpliacionViewModel
                         {
                             cita=ec,
                             examen = db.EXAMENXATENCION.Where(x => x.codigopaciente == ec.codigopaciente && x.codigoestudio == ec.codigoestudio.Substring(0, 5) + "0" + ec.codigoestudio.Substring(6) ).FirstOrDefault()
                         }
                ).ToList();
            return View(lista);
        }

    }
}
