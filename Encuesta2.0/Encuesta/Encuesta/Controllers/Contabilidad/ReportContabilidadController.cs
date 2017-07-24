using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Encuesta.Controllers.Contabilidad
{
    public class ReportContabilidadController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        //
        // GET: /ReportContabilidad/

        public ActionResult Pagos()
        {
            return View();
        }

        public ActionResult ListaPagos( DateTime i, DateTime f)
        {
            Pagos_Mes pa = new Pagos_Mes();
            pa.encuesta  = db.spu_getReporteEncuestadores(i.ToShortDateString(), f.ToShortDateString()).ToList();
            pa.supervision = db.spu_getReporteSupervisores(i.ToShortDateString(), f.ToShortDateString()).ToList();
            return View(pa);
        }

    }
}

public class Pagos_Mes {
    public List<spu_getReporteEncuestadores_Result> encuesta { get; set; }
    public List<spu_getReporteSupervisores_Result> supervision { get; set; }
}
