using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Encuesta.Models;
using Encuesta.Member;

using System.Web.Security;
using Newtonsoft;
using log4net;
using Encuesta.Util;

namespace Encuesta.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public ActionResult Index()
        {
             return View();
        }

       
    }
}
