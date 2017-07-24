using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SistemaResocentro.Controllers.Medica
{
    [Authorize(Roles = "14-c")]
    public class TecnologoController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: Tecnologo
        public ActionResult EnviarIntegrador()
        {
            List<cboequipo> lista = new List<cboequipo>();
            foreach (var item in db.UNIDADNEGOCIO.Where(x => x.codigounidad != 1).ToList())
            {
                cboequipo ce = new cboequipo();
                ce.empresa = item.nombre;
                ce.equipos = db.EQUIPO.Where(x => x.codigounidad2 == item.codigounidad && x.estado == "1" && x.isIntegrator == true).Select(x => new lstequipo { idequipo = x.AETitleConsulta, nombre = x.nombreequipo }).ToList();
                if (ce.equipos.Count > 0)
                    lista.Add(ce);
            }

            ViewBag.Equipo = lista;
            return View();
        }
        public ActionResult getExamen(int examen)
        {
            return Json(db.EXAMENXATENCION.Where(x => x.codigo == examen && !x.codigoestudio.StartsWith("1")).Select(x => new { nombre = x.ATENCION.PACIENTE.apellidos + ", " + x.ATENCION.PACIENTE.nombres, estudio = x.ESTUDIO.nombreestudio }).ToArray(), JsonRequestBehavior.AllowGet);

        }
        public ActionResult sendIntegrador(int examen, string equipo)
        {
            try
            {
                if (db.INTEGRACION.SingleOrDefault(x => x.numero_estudio == examen) == null)
                {
                    db.integrador(examen, equipo);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

    }
}

public class cboequipo
{
    public string empresa { get; set; }
    public List<lstequipo> equipos { get; set; }
}
public class lstequipo
{
    public string idequipo { get; set; }
    public string nombre { get; set; }
}