using Newtonsoft.Json.Converters;
using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaResocentro.Controllers
{
    [Authorize]
    public class PacienteController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: Paciente
        public ActionResult CreatePaciente()
        {
            ViewBag.Tdocumento = new SelectList(new Variable().getTipoDocumento(), "codigo", "nombre");
            ViewBag.Sexo = new SelectList(new Variable().getSexo(), "codigo", "nombre");
            return View(new PACIENTE());
        }

        public ActionResult CreatePacienteAjax(string item)
        {
            var format = "dd/MM/yyyy"; // your datetime format
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
            PACIENTE paciente = Newtonsoft.Json.JsonConvert.DeserializeObject<PACIENTE>(item,dateTimeConverter);
            paciente.codigopaciente=db.PACIENTE.Max(x => x.codigopaciente) + 1;
            if (paciente.telefono != null)
                paciente.telefono = "";
            db.PACIENTE.Add(paciente);
            try
            {
                //db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, idpaciente = paciente.codigopaciente }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdatePaciente( int codigopaciente)
        {
            ViewBag.Tdocumento = new SelectList(new Variable().getTipoDocumento(), "codigo", "nombre");
            
            var pac = db.PACIENTE.Where(x => x.codigopaciente == codigopaciente).SingleOrDefault();
            ViewBag.Sexo = new SelectList(new Variable().getSexo(), "codigo", "nombre");
            return View(pac);
        }

        public ActionResult UpdatePacienteAjax(string item)
        {
            var format = "dd/MM/yyyy"; // your datetime format
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
            PACIENTE paciente = Newtonsoft.Json.JsonConvert.DeserializeObject<PACIENTE>(item,dateTimeConverter);
            var _pac = db.PACIENTE.Where(x => x.codigopaciente == paciente.codigopaciente).SingleOrDefault();
            if (_pac != null)
            {
                _pac.nacionalidad = paciente.nacionalidad;
                _pac.direccion = paciente.direccion;
                _pac.email = paciente.email;
                _pac.celular = paciente.celular;
                _pac.telefono = paciente.telefono;
                _pac.fechanace = paciente.fechanace;
                _pac.sexo = paciente.sexo;
                _pac.nombres = paciente.nombres;
                _pac.apellidos = paciente.apellidos;
                _pac.dni = paciente.dni;
                _pac.tipo_doc = paciente.tipo_doc;
            }else
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, idpaciente = paciente.codigopaciente }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchPaciente()
        {
            return View();
        }

        public ActionResult DetallePaciente(int codigopaciente)
        {
            ViewBag.Tdocumento = new SelectList(new Variable().getTipoDocumento(), "codigo", "nombre");

            var pac = db.PACIENTE.Where(x => x.codigopaciente == codigopaciente).SingleOrDefault();
            ViewBag.Sexo = new SelectList(new Variable().getSexo(), "codigo", "nombre");
            return View(pac);
        }

        public ActionResult ListaPaciente(string apellido)
        {
            return View(db.PACIENTE.Where(x=> x.apellidos.Contains(apellido)).AsParallel().ToList());
        }

        public ActionResult ValidarDocumento(string dni, string tdoc)
        {
            var _exist = db.PACIENTE.Where(x => x.dni == dni && x.tipo_doc == tdoc).FirstOrDefault();
            return Json(_exist == null,JsonRequestBehavior.AllowGet);
        }
    }
}