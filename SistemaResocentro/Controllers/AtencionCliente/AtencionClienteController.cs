using SistemaResocentro.Member;
using SistemaResocentro.Models;
using SistemaResocentro.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaResocentro.Controllers
{
    [Authorize(Roles = "6,7,8,10,15,37")]
    public class AtencionClienteController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        private ResocentroWebEntities db1 = new ResocentroWebEntities();
        // GET: AtencionCliente
        public ActionResult GenerarCorrelativo()
        {
            return View();
        }

        #region Motivos
        //
        // GET: /Motivo/

        public ActionResult ListaMotivos()
        {
            return View(db.MotivoCodigo.ToList());
        }

        //
        // GET: /Motivo/Details/5

        public ActionResult DetalleMotivo(int id = 0)
        {
            MotivoCodigo motivocodigo = db.MotivoCodigo.Find(id);
            if (motivocodigo == null)
            {
                return HttpNotFound();
            }
            return View(motivocodigo);
        }

        //
        // GET: /Motivo/Create

        public ActionResult RegistrarMotivo()
        {
            return View();
        }

        //
        // POST: /Motivo/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarMotivo(MotivoCodigo motivocodigo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            if (ModelState.IsValid)
            {
                motivocodigo.userRegister = user.ProviderUserKey.ToString();
                motivocodigo.fecha = DateTime.Now;
                db.MotivoCodigo.Add(motivocodigo);
                db.SaveChanges();
                return RedirectToAction("ListaMotivos");
            }

            return View(motivocodigo);
        }

        //
        // GET: /Motivo/Edit/5

        public ActionResult EditarMotivo(int id = 0)
        {
            MotivoCodigo motivocodigo = db.MotivoCodigo.Find(id);
            if (motivocodigo == null)
            {
                return HttpNotFound();
            }
            return View(motivocodigo);
        }

        //
        // POST: /Motivo/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarMotivo(MotivoCodigo motivocodigo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            if (ModelState.IsValid)
            {
                motivocodigo.user_lastUpdate = user.ProviderUserKey.ToString();
                motivocodigo.lastUpdate = DateTime.Now;
                db.Entry(motivocodigo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListaMotivos");
            }
            return View(motivocodigo);
        }

        //
        // GET: /Motivo/Delete/5

        public ActionResult EliminarMotivo(int id = 0)
        {
            MotivoCodigo motivocodigo = db.MotivoCodigo.Find(id);
            if (motivocodigo == null)
            {
                return HttpNotFound();
            }
            return View(motivocodigo);
        }

        //
        // POST: /Motivo/Delete/5

        [HttpPost, ActionName("EliminarMotivo")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarMotivoConfirmed(int id)
        {
            MotivoCodigo motivocodigo = db.MotivoCodigo.Find(id);
            db.MotivoCodigo.Remove(motivocodigo);
            db.SaveChanges();
            return RedirectToAction("ListaMotivos");
        }


        #endregion
        #region Historial Motivos
        public ActionResult HistorialMotivos()
        {
            var historialcorrelativo = db.HistorialCorrelativo.Include(h => h.MotivoCodigo).Include(h => h.USUARIO).OrderByDescending(x => x.idHistorial);
            return View(historialcorrelativo.ToList());
        }


        #endregion
        #region Generar Codigo
        public ActionResult ListaGenerado()
        {
            var correlativomotivo = db.CorrelativoMotivo.Include(c => c.MotivoCodigo);
            return View(correlativomotivo.ToList());
        }

        //
        // GET: /Generador/Details/5

        public ActionResult DetalleGenerado(int id = 0)
        {
            CorrelativoMotivo correlativomotivo = db.CorrelativoMotivo.Find(id);
            if (correlativomotivo == null)
            {
                return HttpNotFound();
            }
            return View(correlativomotivo);
        }

        //
        // GET: /Generador/Create

        public ActionResult RegistrarGenerado()
        {
            ViewBag.idmotivo = new SelectList(db.MotivoCodigo, "idMotivo", "descripcion");
            return View();
        }

        //
        // POST: /Generador/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarGenerado(CorrelativoMotivo correlativomotivo)
        {
            if (ModelState.IsValid)
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                correlativomotivo.userRegister = user.ProviderUserKey.ToString();
                db.CorrelativoMotivo.Add(correlativomotivo);
                db.SaveChanges();
                return RedirectToAction("ListaGenerado");
            }

            ViewBag.idmotivo = new SelectList(db.MotivoCodigo, "idMotivo", "descripcion", correlativomotivo.idmotivo);
            return View(correlativomotivo);
        }

        //
        // GET: /Generador/Edit/5

        public ActionResult EditarGenerado(int id = 0)
        {
            CorrelativoMotivo correlativomotivo = db.CorrelativoMotivo.Find(id);
            correlativomotivo.correlativo += 1;
            correlativomotivo.MotivoCodigo.userRegister = "";
            correlativomotivo.lastGenerado = correlativomotivo.MotivoCodigo.nomenclatura + (correlativomotivo.correlativo).ToString("D10");
            if (correlativomotivo == null)
            {
                return HttpNotFound();
            }

            return View(correlativomotivo);
        }

        //
        // POST: /Generador/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarGenerado(CorrelativoMotivo correlativomotivo)
        {
            if (ModelState.IsValid)
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                var item = (from x in db.CorrelativoMotivo where x.idmotivo == correlativomotivo.idmotivo select x).SingleOrDefault();

                item.correlativo = correlativomotivo.correlativo;
                item.lastUpdate = DateTime.Now;
                //                item.userRegister = correlativomotivo.userRegister;
                item.lastGenerado = correlativomotivo.lastGenerado;
                /*Agregando a Historial*/
                HistorialCorrelativo his = new HistorialCorrelativo();
                his.idmotivo = correlativomotivo.idmotivo;
                his.correlativo = correlativomotivo.lastGenerado;
                his.usu_Generador = user.ProviderUserKey.ToString();
                his.solicitante = correlativomotivo.MotivoCodigo.userRegister;
                his.motivo = correlativomotivo.MotivoCodigo.userRegister;
                his.fecha = DateTime.Now;
                db.HistorialCorrelativo.Add(his);
                db.SaveChanges();
                return RedirectToAction("ListaGenerado");
            }

            return View(correlativomotivo);
        }

        #endregion
        #region Pagina Web
        public ActionResult Correos()
        {
            return View();
        }
        public ActionResult ListConsulta()
        {
            return View(db1.Consulta.Where(x => x.IsEliminado == false).ToList());
        }
        public ActionResult DeleteConsulta(int id)
        {
            var consulta = db1.Consulta.Where(x => x.ConsultaId == id).SingleOrDefault();
            if (consulta != null)
                consulta.IsEliminado = true;
            db1.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConsultaisAtendido(int id)
        {
            var consulta = db1.Consulta.Where(x => x.ConsultaId == id).SingleOrDefault();
            if (consulta != null)
                consulta.isAtendido = !consulta.isAtendido;
            db1.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult responerConsulta(int id, string msj)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var consulta = db1.Consulta.Where(x => x.ConsultaId == id).SingleOrDefault();
            Historia his = new Historia();
            his.ConsultId = id;
            his.usuario = "Informes Resocentro (" + user.UserName.ToString() + ")";
            his.fecha = DateTime.Now;
            his.mensaje = msj;
            db1.Historia.Add(his);
            db1.SaveChanges();
            #region msj
            string titulo = @"<TABLE style='BORDER-COLLAPSE: collapse; BORDER-SPACING: 0'>
 <TBODY>
  <TR >
   <TD  style='WIDTH: 80%;'>
    <TABLE class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'>
     <TBODY>
      <TR>
       <TD class='padded'  style='WORD-WRAP: break-word; PADDING-LEFT: 0px; FONT-SIZE: 12px; FONT-FAMILY: Tahoma,sans-serif; VERTICAL-ALIGN: top;TEXT-ALIGN: left;'>
        <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a): " +
                                             consulta.Nombre.Substring(0, 1).ToLower() + consulta.Nombre.Substring(1)
                              + @"</h1>
       </TD>
      </TR>
     </TBODY>
    </TABLE>
   </TD>
   <TD  style='WIDTH: 20%; VERTICAL-ALIGN: top; PADDING-BOTTOM: 32px;'>
    <TABLE class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'>
     <TBODY>
      <TR>
       <TD style='WORD-WRAP: break-word;FONT-FAMILY: Tahoma,sans-serif; VERTICAL-ALIGN: top;TEXT-ALIGN: right;'>
         <a href='http://www.resocentro.com/Paciente/Consulta?id=" + his.ConsultId + @"' style='TEXT-DECORATION: none; FONT-FAMILY: Tahoma,sans-serif;float: right;font-size: 15px;'>N° Consulta: " + his.ConsultId + @"</a>
       </TD>
      </TR>
     </TBODY>
    </TABLE>
   </TD>
  </TR>
 </TBODY>
</TABLE>", cuerpo = @"<p style='Margin-top: 0;color: #565656;font-family: Tahoma,sans-serif;font-size: 15px;line-height: 25px;Margin-bottom: 24px'>
Hemos respondido a su consulta, por favor ingrese a nuestra web en la sección <a href='http://www.resocentro.com/Paciente/Contactenos'>Contáctenos</a> con el número de consulta (" + his.ConsultId + @") o acceda mediante el siguiente <a href='http://www.resocentro.com/Paciente/Consulta?id=" + his.ConsultId + @"'>Enlace</a>.
</p>
", img = "http://extranet.resocentro.com:5050/PaginaWeb/correo/Contactenostop.jpg";
            #endregion
            //sendCorreo("Confirmación de Delivery", "jhon.alvarez@resocentro.com", "jefferson.oviedo@resocentro.com", mensaje);
            //mensaje = string.Format(mensaje, item.nombres.Substring(0, 1).ToUpper() + item.nombres.Substring(1), item.email);
            if (new Variable().sendCorreo("Respuesta a su Consulta", consulta.Email, "", new Variable().getCuerpoEmail(img, titulo, cuerpo), ""))
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getConsulta(int id)
        {
            HistorialConsulta his = new HistorialConsulta();
            his.pregunta = db1.Consulta.Where(x => x.ConsultaId == id).SingleOrDefault();
            his.pregunta.isLeido = true;
            his.historia = db1.Historia.Where(x => x.ConsultId == id).OrderByDescending(x => x.fecha).ToList();
            db1.SaveChanges();
            return View(his);
        }
        #endregion
        #region Seguimiento de Produccion
        public ActionResult ListaSeguimiento()
        {
            return View();
        }

        public ActionResult ListaSegAsync(string empresa, string inicio, string fin, string estado)
        {
            var lista = new Script_Sql().SeguimientoProduccion(empresa, estado, inicio, fin);
            List<ListaSeguimientoViewModels> lista_final = new List<ListaSeguimientoViewModels>();

            foreach (var item in lista)
            {
                Errores_SegViewModel errores = new Errores_SegViewModel();
                errores.obs = item.observacion;
                errores.examen = item.nexamen;
                bool add = false;
                var log = db.Log_Seguimiento_Produccion.SingleOrDefault(x => x.examen == item.nexamen);
                if (log == null)
                    log = new Log_Seguimiento_Produccion();

                //if (log != null)
                //{
                //    item.error.medico = log.medico ? "" : item.error.medico;
                //    item.error.dni = log.dni ? "" : item.error.dni;
                //    item.error.clinica = log.clinica ? "" : item.error.clinica;
                //    item.error.carta = log.cobertura ? "" : item.error.carta;
                //    item.error.edad = log.edad ? "" : item.error.edad;
                //    item.error.talla = log.talla ? "" : item.error.talla;
                //    item.error.peso = log.peso ? "" : item.error.peso;
                //    item.error.documento = log.documento ? "" : item.error.documento;
                //    item.error.adjunto = log.adjunto_admision ? "" : item.error.adjunto;
                //    item.error.adjuntocarta = log.adjunto_carta ? "" : item.error.adjuntocarta;
                //}
                // clinica
                if (item.clinica.Contains("Particular"))
                {
                    if (log.clinica == false)
                    {
                        add = true;
                        errores.clinica = "Verificar Clínica Procedencia(" + item.clinica + ")";
                    }
                }
                // medico
                if (item.medico.Contains("Desconocido"))
                    if (log.medico == false)
                    {
                        add = true;
                        errores.medico = "Verificar Médico";
                    }
                // DNI
                if (item.tipo_documento.Contains("DNI"))
                    if (log.dni == false)
                    {
                        if (item.ndocumento.Length != 8)
                        {
                            add = true;
                            errores.dni = "DNI Incorrecto";
                        }
                    }

                //pagos
                if (item.estado == "P" || item.estado == "I" || item.estado == "V")
                {
                    if (item.tipo_paciente == "Particular")
                    {
                        if (item.documentos == "")
                        {
                            add = true;
                            errores.documento = "No se cobro al Paciente";
                        }
                    }
                    else if (item.tipo_paciente == "Asegurado")
                    {
                        if (item.documentos == "")
                        {
                            add = true;
                            errores.documento = "No se cobro al Paciente";
                        }
                        else if (item.cobertura < 100)
                        {
                            if (item.estado == "I" || item.estado == "V")
                            {
                                var doc = item.documentos.Split('#');
                                if (doc.Count() < 3)
                                {
                                    add = true;
                                    errores.documento = "Falta generar un documento";
                                }
                            }
                        }
                    }
                }

                // cobertura
                if (item.tipo_paciente.Contains("Asegurado") && item.cobertura == 0)
                    if (log.cobertura == false)
                    {
                        add = true;
                        errores.carta = "Falta modificar la cobertura (" + item.codigocarta + ")";
                    }

                //ADjunto
                if (!item.estudio.Contains("Ampliaci"))
                {
                    if (!item.carta)
                        if (!item.adjunto)
                            if (log.adjunto_admision == false)
                            {
                                add = true;
                                errores.adjunto = "No tiene ningun adjunto Admisión";
                            }
                }
                //ADjunto Carta
                if (item.tipo_paciente == "Asegurado")
                    if (!item.carta)
                        if (log.adjunto_carta == false)
                        {
                            add = true;
                            errores.adjuntocarta = "No tiene ningun adjunto Carta";
                        }


                //Edad
                var _edad = new Variable().calcularedad(item.nacimiento);
                if (item.edad != _edad)
                    if (log.edad == false)
                    {
                        add = true;
                        errores.edad = "No corresponde la edad (" + _edad.ToString() + ")";
                    }
                //Talla
                if (item.edad < 5)
                {
                    if (item.talla < 1)
                        if (log.talla == false)
                        {
                            add = true;
                            errores.talla = "No corresponde la edad con la talla";
                        }
                }
                else if (item.edad < 10)
                {
                    if (item.talla < 1.20)
                        if (log.talla == false)
                        {
                            add = true;
                            errores.talla = "No corresponde la edad con la talla";
                        }
                }
                else if (item.edad > 10)
                {
                    if (item.talla < 1.50)
                        if (log.talla == false)
                        {
                            add = true;
                            errores.talla = "No corresponde la edad con la talla";
                        }
                }

                //Peso
                if (item.talla < 1)
                {
                    if (item.peso < 10)
                        if (log.peso == false)
                        {
                            add = true;
                            errores.peso = "No corresponde la talla con el peso";
                        }
                }
                else if (item.talla < 1.30)
                {
                    if (item.peso < 20)
                        if (log.peso == false)
                        {
                            add = true;
                            errores.peso = "No corresponde la talla con el peso";
                        }
                }
                else if (item.talla > 1.50)
                {
                    if (item.peso < 40)
                        if (log.peso == false)
                        {
                            add = true;
                            errores.peso = "No corresponde la talla con el peso";
                        }
                }
                item.error = errores;
                if (add)
                {
                    lista_final.Add(item);

                }
            }

            var result = (from x in lista_final
                          select new[]{
                              Newtonsoft.Json.JsonConvert.SerializeObject(x.error),
                              x.adjunto.ToString(),
                              x.carta.ToString(),
                              x.fecha.ToString("dd/MM/yy HH:mm "),
                              x.nexamen.ToString(),
                              x.estado.ToString(),
                              x.sucursal.ToString(),
                              x.tipo_paciente.ToString(),
                              x.tipo_documento.ToString(),
                              x.ndocumento.ToString(),
                              x.paciente.ToString(),
                              x.isProtocolo.ToString(),
                              x.sexo.ToString(),
                              x.nacimiento.ToString("dd/MM/yyyy"),
                              x.edad.ToString(),
                              x.peso.ToString(),
                              x.talla.ToString(),
                              x.aseguradora.ToString(),
                              x.clinica.ToString(),
                              x.medico.ToString(),
                              x.estudio.ToString(),
                              x.cobertura.ToString(),
                              x.documentos.ToString(),
                            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAdjuntos(int examen, int tipo)
        {
            string fileName;
            if (tipo == 1)
            {
                var _a = (from ea in db.EXAMENXATENCION
                          join esc in db.ESCANADMISION on ea.numeroatencion equals esc.numerodeatencion into esc_join
                          from esc in esc_join.DefaultIfEmpty()
                          where ea.codigo == examen
                          select esc).SingleOrDefault();
                if (_a != null)
                {
                    byte[] archivo = _a.cuerpoarchivo;
                    fileName = _a.nombrearchivo;

                    return File(archivo, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            else
            {
                var _a = (from ea in db.EXAMENXATENCION
                          join ca in db.CARTAGARANTIA on ea.ATENCION.CITA.codigocartagarantia equals ca.codigocartagarantia into carta_join
                          from ca in carta_join.DefaultIfEmpty()
                          join dca in db.DOCESCANEADO on ca.codigodocadjunto equals dca.codigodocadjunto into dcarta_join
                          from dca in dcarta_join.DefaultIfEmpty()
                          where ea.codigo == examen
                          select dca).SingleOrDefault();
                if (_a != null)
                {
                    byte[] archivo = _a.cuerpoarchivo;
                    fileName = _a.nombrearchivo;

                    return File(archivo, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            return View();

        }

        public ActionResult GetMedicos()
        {
            return Json(new SelectList(db.MEDICOEXTERNO.Where(x => x.isactivo == true).Select(x => new { cmp = x.cmp, nombre = x.cmp + " - " + x.apellidos + " " + x.nombres }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre"));
        }
        public ActionResult GetClinicas()
        {
            return Json(new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).Select(x => new { codigo = x.codigoclinica, nombre = x.razonsocial }).OrderBy(x => x.nombre).ToList(), "codigo", "nombre"));
        }

        public ActionResult UpdateSeguimiento(string cadena)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            using (Log_AplicativoEntities db1 = new Log_AplicativoEntities())
            {
                Log_sistResocentro log;
                Errores_SegViewModel error = Newtonsoft.Json.JsonConvert.DeserializeObject<Errores_SegViewModel>(cadena);
                var _exam = db.EXAMENXATENCION.Where(x => x.codigo == error.examen).SingleOrDefault();
                var _cita = db.CITA.Where(x => x.numerocita == _exam.numerocita).SingleOrDefault();
                if (!error.isCorrectpeso)
                {
                    if (error.medico != null && error.medico != "")
                    {
                        log = new Log_sistResocentro();
                        log.examen = _exam.codigo;
                        log.fecha = DateTime.Now;
                        log.tabla = "CITA";
                        log.campo = "cmp";
                        log.oldvalue = _cita.cmp;
                        log.newvalue = error.medico;
                        log.usuario = user.ProviderUserKey.ToString();
                        log.modulo = "Seguimiento Produccion";
                        db1.Log_sistResocentro.Add(log);
                        db1.SaveChanges();
                        _cita.cmp = error.medico;
                        _exam.ATENCION.cmp = error.medico;
                        db.SaveChanges();
                    }
                }
                else
                {
                    log = new Log_sistResocentro();
                    log.examen = _exam.codigo;
                    log.fecha = DateTime.Now;
                    log.tabla = "CITA";
                    log.campo = "cmp";
                    log.oldvalue = _cita.cmp;
                    log.newvalue = "es correcto";
                    log.usuario = user.ProviderUserKey.ToString();
                    log.modulo = "Seguimiento Produccion";
                    db1.Log_sistResocentro.Add(log);
                    db1.SaveChanges();
                }

                if (!error.isCorrectdni)
                {
                    if (error.dni != "")
                    {
                        log = new Log_sistResocentro();
                        log.examen = _exam.codigo;
                        log.fecha = DateTime.Now;
                        log.tabla = "PACIENTE";
                        log.campo = "dni";
                        log.oldvalue = _cita.PACIENTE.dni;
                        log.newvalue = error.dni;
                        log.usuario = user.ProviderUserKey.ToString();
                        log.modulo = "Seguimiento Produccion";
                        db1.Log_sistResocentro.Add(log);
                        db1.SaveChanges();
                        _cita.PACIENTE.dni = error.dni;
                        db.SaveChanges();
                    }
                }
                else
                {
                    log = new Log_sistResocentro();
                    log.examen = _exam.codigo;
                    log.fecha = DateTime.Now;
                    log.tabla = "PACIENTE";
                    log.campo = "dni";
                    log.oldvalue = _cita.PACIENTE.dni;
                    log.newvalue = "es correcto";
                    log.usuario = user.ProviderUserKey.ToString();
                    log.modulo = "Seguimiento Produccion";
                    db1.Log_sistResocentro.Add(log);
                    db1.SaveChanges();
                }
                if (!error.isCorrectclinica)
                {
                    if (error.clinica != null && error.clinica != "")
                    {
                        var idcli = int.Parse(error.clinica);
                        var _clinica = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == idcli);
                        log = new Log_sistResocentro();
                        log.examen = _exam.codigo;
                        log.fecha = DateTime.Now;
                        log.tabla = "CITA";
                        log.campo = "codigoclinica";
                        log.oldvalue = _cita.codigoclinica.ToString();
                        log.newvalue = _clinica.codigoclinica.ToString();
                        log.usuario = user.ProviderUserKey.ToString();
                        log.modulo = "Seguimiento Produccion";
                        db1.Log_sistResocentro.Add(log);
                        db1.SaveChanges();
                        _cita.codigoclinica = _clinica.codigoclinica;
                        _cita.codigozona = _clinica.codigozona;
                        db.SaveChanges();
                    }
                }
                else
                {
                    log = new Log_sistResocentro();
                    log.examen = _exam.codigo;
                    log.fecha = DateTime.Now;
                    log.tabla = "CITA";
                    log.campo = "codigoclinica";
                    log.oldvalue = _cita.codigoclinica.ToString();
                    log.newvalue = "es correcto";
                    log.usuario = user.ProviderUserKey.ToString();
                    log.modulo = "Seguimiento Produccion";
                    db1.Log_sistResocentro.Add(log);
                    db1.SaveChanges();
                }
                if (!error.isCorrectcarta)
                {
                    if (error.carta != "")
                    {
                        log = new Log_sistResocentro();
                        log.examen = _exam.codigo;
                        log.fecha = DateTime.Now;
                        log.tabla = "CARTAGARANTIA";
                        log.campo = "cobertura";
                        if (_cita.CARTAGARANTIA != null)
                            log.oldvalue = _cita.CARTAGARANTIA.cobertura.ToString();
                        else
                            log.oldvalue = "*";
                        log.newvalue = error.carta;
                        log.usuario = user.ProviderUserKey.ToString();
                        log.modulo = "Seguimiento Produccion";
                        db1.Log_sistResocentro.Add(log);
                        db1.SaveChanges();
                        _cita.CARTAGARANTIA.cobertura = float.Parse(error.carta);
                        db.SaveChanges();
                    }
                }
                else
                {
                    log = new Log_sistResocentro();
                    log.examen = _exam.codigo;
                    log.fecha = DateTime.Now;
                    log.tabla = "CARTAGARANTIA";
                    log.campo = "cobertura";
                    if (_cita.CARTAGARANTIA != null)
                        log.oldvalue = _cita.CARTAGARANTIA.cobertura.ToString();
                    else
                        log.oldvalue = "*";
                    log.newvalue = "es correcto";
                    log.usuario = user.ProviderUserKey.ToString();
                    log.modulo = "Seguimiento Produccion";
                    db1.Log_sistResocentro.Add(log);
                    db1.SaveChanges();
                }
                if (!error.isCorrectedad)
                {
                    if (error.edad != "")
                    {
                        log = new Log_sistResocentro();
                        log.examen = _exam.codigo;
                        log.fecha = DateTime.Now;
                        log.tabla = "ATENCION";
                        log.campo = "edad";
                        log.oldvalue = _exam.ATENCION.edad.ToString();
                        log.newvalue = error.edad;
                        log.usuario = user.ProviderUserKey.ToString();
                        log.modulo = "Seguimiento Produccion";
                        db1.Log_sistResocentro.Add(log);
                        db1.SaveChanges();
                        _exam.ATENCION.edad = int.Parse(error.edad);
                        db.SaveChanges();
                    }
                }
                else
                {
                    log = new Log_sistResocentro();
                    log.examen = _exam.codigo;
                    log.fecha = DateTime.Now;
                    log.tabla = "ATENCION";
                    log.campo = "edad";
                    log.oldvalue = _exam.ATENCION.edad.ToString();
                    log.newvalue = "es correcta";
                    log.usuario = user.ProviderUserKey.ToString();
                    log.modulo = "Seguimiento Produccion";
                    db1.Log_sistResocentro.Add(log);
                    db1.SaveChanges();

                }
                if (!error.isCorrecttalla)
                {
                    if (error.talla != "")
                    {
                        log = new Log_sistResocentro();
                        log.examen = _exam.codigo;
                        log.fecha = DateTime.Now;
                        log.tabla = "ATENCION";
                        log.campo = "talla";
                        log.oldvalue = _exam.ATENCION.talla.ToString();
                        log.newvalue = error.talla;
                        log.usuario = user.ProviderUserKey.ToString();
                        log.modulo = "Seguimiento Produccion";
                        db1.Log_sistResocentro.Add(log);
                        db1.SaveChanges();
                        _exam.ATENCION.talla = float.Parse(error.talla);
                        db.SaveChanges();
                    }
                }
                else
                {
                    log = new Log_sistResocentro();
                    log.examen = _exam.codigo;
                    log.fecha = DateTime.Now;
                    log.tabla = "ATENCION";
                    log.campo = "talla";
                    log.oldvalue = _exam.ATENCION.talla.ToString();
                    log.newvalue = "es correcto";
                    log.usuario = user.ProviderUserKey.ToString();
                    log.modulo = "Seguimiento Produccion";
                    db1.Log_sistResocentro.Add(log);
                    db1.SaveChanges();
                }
                if (!error.isCorrectpeso)
                {
                    if (error.peso != "")
                    {
                        log = new Log_sistResocentro();
                        log.examen = _exam.codigo;
                        log.fecha = DateTime.Now;
                        log.tabla = "ATENCION";
                        log.campo = "peso";
                        log.oldvalue = _exam.ATENCION.peso.ToString();
                        log.newvalue = error.peso;
                        log.usuario = user.ProviderUserKey.ToString();
                        log.modulo = "Seguimiento Produccion";
                        db1.Log_sistResocentro.Add(log);
                        db1.SaveChanges();
                        _exam.ATENCION.peso = int.Parse(error.peso);
                        db.SaveChanges();

                    }
                }
                else
                {
                    log = new Log_sistResocentro();
                    log.examen = _exam.codigo;
                    log.fecha = DateTime.Now;
                    log.tabla = "ATENCION";
                    log.campo = "peso";
                    log.oldvalue = _exam.ATENCION.peso.ToString();
                    log.newvalue = "es correcto";
                    log.usuario = user.ProviderUserKey.ToString();
                    log.modulo = "Seguimiento Produccion";
                    db1.Log_sistResocentro.Add(log);
                    db1.SaveChanges();
                }
                var _logSeg = db.Log_Seguimiento_Produccion.SingleOrDefault(x => x.examen == error.examen);
                if (_logSeg == null)
                {
                    Log_Seguimiento_Produccion log_seg = new Log_Seguimiento_Produccion();
                    log_seg.examen = error.examen;
                    log_seg.medico = error.isCorrectmedico;
                    log_seg.dni = error.isCorrectdni;
                    log_seg.clinica = error.isCorrectclinica;
                    log_seg.cobertura = error.isCorrectcarta;
                    log_seg.edad = error.isCorrectedad;
                    log_seg.talla = error.isCorrecttalla;
                    log_seg.peso = error.isCorrectpeso;
                    log_seg.documento = false;
                    log_seg.adjunto_admision = error.isCorrectadjunto;
                    log_seg.adjunto_carta = error.isCorrectadjuntocarta;
                    db.Log_Seguimiento_Produccion.Add(log_seg);
                    db.SaveChanges();
                }
                else
                {
                    _logSeg.medico = error.isCorrectmedico;
                    _logSeg.dni = error.isCorrectdni;
                    _logSeg.clinica = error.isCorrectclinica;
                    _logSeg.cobertura = error.isCorrectcarta;
                    _logSeg.edad = error.isCorrectedad;
                    _logSeg.talla = error.isCorrecttalla;
                    _logSeg.peso = error.isCorrectpeso;
                    _logSeg.documento = false;
                    _logSeg.adjunto_admision = error.isCorrectadjunto;
                    _logSeg.adjunto_carta = error.isCorrectadjuntocarta;
                    db.SaveChanges();
                }

            }
            return Json(true);
        }
        public ActionResult AdjuntarArchivos(string numero, string tipo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var r = new List<UploadFilesResult>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;

                if (hpf.ContentLength == 0)
                    continue;

                int length = hpf.ContentLength;
                byte[] buffer = new byte[length];
                hpf.InputStream.Read(buffer, 0, length);
                var nexamen = int.Parse(numero);
                var _examen = db.EXAMENXATENCION.Where(x => x.codigo == nexamen).SingleOrDefault();
                var _paciente = _examen.ATENCION.PACIENTE;
                if (tipo == "1")//admision
                {
                    ESCANADMISION doc = new ESCANADMISION();
                    doc.numerodeatencion = _examen.numeroatencion;
                    doc.nombrearchivo = _paciente.apellidos + " " + _paciente.nombres + ".pdf";
                    doc.cuerpoarchivo = buffer;
                    doc.fecharegistro = DateTime.Now;
                    doc.codigousuario = user.ProviderUserKey.ToString();
                    db.ESCANADMISION.Add(doc);
                    db.SaveChanges();
                }
                if (tipo == "2")//carta
                {

                    var _cita = db.CITA.Where(x => x.numerocita == _examen.numerocita).SingleOrDefault();
                    var _carta = db.CARTAGARANTIA.Where(x => x.codigocartagarantia == _cita.CARTAGARANTIA.codigocartagarantia).SingleOrDefault();
                    DOCESCANEADO doc = new DOCESCANEADO();
                    doc.codigodocadjunto = (DateTime.Now.ToString("ddMMyyyyHHmm") + "-" + _carta.codigopaciente.ToString());
                    doc.nombrearchivo = _paciente.apellidos + " " + _paciente.nombres + ".pdf";
                    doc.cuerpoarchivo = buffer;
                    doc.fecharegistro = DateTime.Now;
                    doc.codigousuario = user.ProviderUserKey.ToString();
                    _carta.codigodocadjunto = doc.codigodocadjunto;
                    db.DOCESCANEADO.Add(doc);
                    db.SaveChanges();
                }
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
        #endregion
        #region Reporte Atenciones
        public ActionResult ReporteAtencion()
        {
            return View();
        }

        public ActionResult ReporteAtencionAsync(string sede, string inicio, string fin)
        {
            var lista = new Script_Sql().ReporteAtenciones(sede, inicio, fin);
            var result = (from x in lista
                          select new[]{
                               x.fecha.ToString("dd/MM/yy HH:mm"),
                               x.apellidos.ToString(),
                               x.nombres.ToString(),
                               x.examen.ToString(),
                               x.estudio.ToString(),
                               x.estado.ToString(),
                               x.aseguradora.ToString(),
                               x.sedacion?"Si":"No",
                               x.contraste?"Si":"No",
                               x.documentos.ToString()
                          });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

      
        #endregion
        #region Entre Turno
        public ActionResult AprobarEntreTurno()
        {
            string queryString = "select se.IdSolicitud,se.hora_reg hora,case when m.tipo=0 then 'Medico' else 'Administrativo' end tipo_sol,m.tipo,s.ShortName solicitante,r.ShortName tramitador,m.descripcion motivo,se.comentario_usu comentarios,e.ShortDesc equipo from Solicitud_Entre_Turno se inner join USUARIO r on se.usu_reg=r.codigousuario inner join USUARIO s on se.solicitante=s.codigousuario inner join Motivo_Entre_Turno m on se.codigomotivo=m.idMotivo inner join EQUIPO e on se.codigoequipo_reg=e.codigoequipo  where se.isaprobado is null and convert(date,se.fecha_reg)=convert(date,GETDATE())";
            //tipo de motivo 0 medico y 1 administrativo
            List<SolicitudesEntreTurno> lista = new List<SolicitudesEntreTurno>();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        lista.Add(new SolicitudesEntreTurno
                        {
                            idSolicitud = Convert.ToInt32(reader["IdSolicitud"]),
                            hora = Convert.ToDateTime(reader["hora"]),
                            tipo = Convert.ToInt32(reader["tipo"]),
                            tipo_solicitud = (reader["tipo_sol"]).ToString(),
                            solicitante = reader["solicitante"].ToString(),
                            tramitador = reader["tramitador"].ToString(),
                            motivo = (reader["motivo"]).ToString(),
                            comentarios = (reader["comentarios"]).ToString(),
                            equipo = (reader["equipo"]).ToString(),
                        });
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }

            return View(lista);
        }
        public ActionResult DetalleEntreTurno(string id)
        {
            string queryString = "select se.IdSolicitud,se.hora_reg hora,m.tipo,s.ShortName solicitante,r.ShortName tramitador,m.descripcion motivo,se.comentario_usu comentarios,e.ShortDesc equipo,se.codigoequipo_reg from Solicitud_Entre_Turno se inner join USUARIO r on se.usu_reg=r.codigousuario inner join USUARIO s on se.solicitante=s.codigousuario inner join Motivo_Entre_Turno m on se.codigomotivo=m.idMotivo inner join EQUIPO e on se.codigoequipo_reg=e.codigoequipo where se.IdSolicitud=" + id;
            //tipo de motivo 0 medico y 1 administrativo
            SolicitudesEntreTurno item = new SolicitudesEntreTurno();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        item.idSolicitud = Convert.ToInt32(reader["IdSolicitud"]);
                        item.hora = Convert.ToDateTime(reader["hora"]);
                        item.tipo = Convert.ToInt32(reader["tipo"]);
                        item.solicitante = reader["solicitante"].ToString();
                        item.tramitador = reader["tramitador"].ToString();
                        item.motivo = (reader["motivo"]).ToString();
                        item.comentarios = (reader["comentarios"]).ToString();
                        item.equipo = (reader["equipo"]).ToString();
                        item.equipomedico = Convert.ToInt32(reader["codigoequipo_reg"].ToString());
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            ViewBag.Equipo = new SelectList((from eq in db.EQUIPO
                                             where eq.estado == "1" &&
                                             (user.sucursales_int).Contains(eq.codigounidad2 * 100 + eq.codigosucursal2) //sucursales asignadas

                                             select eq).AsQueryable(), "codigoequipo", "nombreequipo");
            return View(item);
        }
        public ActionResult getTurnos(string equipo, string hour)
        {
            List<Turno_Horario> listaCarta = new List<Turno_Horario>();
            string queryString = @"SELECT top(10) h.codigohorario AS Id,CONVERT(CHAR(5),h.hora,108)AS Hora,h.bloquear AS E,ISNULL(p.apellidos,'') paciente,ISNULL(es.nombreestudio,'') estudio,h.turnomedico AS Turno FROM HORARIO h left join PACIENTE p on h.codigopaciente=p.codigopaciente left join ESTUDIO es on h.codigoestudio=es.codigoestudio  WHERE h.codigoequipo='{0}' AND DAY(h.fecha)='{1}' AND MONTH(h.fecha)='{2}' AND YEAR(h.fecha)='{3}'  and h.bloquear<>1 and convert(time,h.hora)>convert(time,'{4}:00') ORDER BY h.hora;";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, equipo, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, hour), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            listaCarta.Add(new Turno_Horario
                            {
                                codigohorario = Convert.ToInt32((reader["id"]).ToString()),
                                hora = Convert.ToDateTime((reader["hora"]).ToString()),
                                isBloqueado = Convert.ToBoolean((reader["e"]).ToString()),
                                paciente = ((reader["paciente"]).ToString()),
                                estudio = ((reader["estudio"]).ToString()),
                                turno = ((reader["turno"]).ToString())
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                string formato = @"
                <tr>
                    <td> {0}</td>
                    <td> {1}</td>
                </tr>";
                string result = "";
                foreach (var item in listaCarta)
                {
                    result += string.Format(formato, item.hora.ToString("hh:mmtt"), item.estudio);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult setSolicitudEntreTurno(int id, bool e, int eq, int h, int m, string c)
        {
            string queryString = "select top(1)* from HORARIO where codigoequipo=" + eq + " and CONVERT(date,fecha)=convert(date,'" + DateTime.Now.ToShortDateString() + "') and CONVERT(time,hora)<CONVERT(time,'" + h + ":" + m + "') order by codigohorario desc";
            var turno = ""; var unidad = 0;
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        turno = ((reader["turnomedico"]).ToString());
                        unidad = Convert.ToInt32((reader["codigounidad"]).ToString());
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            bool result = false;
            var solicitud = db.Solicitud_Entre_Turno.SingleOrDefault(x => x.IdSolicitud == id);
            if (solicitud != null)
            {
                solicitud.isaprobado = e;
                solicitud.comentario_med = c;
                solicitud.fec_aprueba = DateTime.Now;
                solicitud.usu_med = user.ProviderUserKey.ToString();
                if (e)
                {
                    var f = DateTime.Now;
                    solicitud.codigoequipo_med = eq;
                    solicitud.hora_med = new DateTime(f.Year, f.Month, f.Day, h, m, 0);
                    //creacion de entreturno


                    HORARIO horario = new HORARIO();
                    horario.atendido = false;
                    horario.confirmado = false;
                    horario.hora = solicitud.hora_med.Value;
                    horario.fecha = solicitud.hora_med.Value;
                    horario.turnomedico = turno;
                    horario.bloquear = false;
                    horario.codigounidad = unidad;
                    horario.codigoequipo = eq;
                    horario.IsEntre = true;
                    db.HORARIO.Add(horario);

                }
                db.SaveChanges();
                var usuario = db.USUARIO.Where(x => x.codigousuario == solicitud.usu_reg).SingleOrDefault();
                if (usuario != null)
                {
                    if (usuario.EMPLEADO != null)
                        if (usuario.EMPLEADO.email != null)
                        {
                            string titulo = @"<h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a):" + usuario.ShortName + "</h1>", cuerpo = @" 
                                   <p>La Solicitud de Entre Turno  <b>" + (e ? "fue APROBADA" : "NO fue APROBADA") + "</b> por " + user.UserName + " para la hora: " + solicitud.hora_med.Value.ToShortTimeString() + ". </p>";
                            new Variable().sendCorreo("Respuesta Solicitud Entre Turno", usuario.EMPLEADO.email, "", new Variable().getCuerpoEmail("http://extranet.resocentro.com:5050/PaginaWeb/correo/Contactenostop.jpg", titulo, cuerpo), "");
                        }
                }

                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Asignacion de Sedaciones
        public ActionResult AsignarAnestesiologo()
        {
            ViewBag.Sedadores = new SelectList(db.Com_Medico.Where(x => x.isActivo == true).Select(x => new { id = x.idCom_Medico, value = x.medico + "   <" + x.email + ">" }).ToList(), "id", "value");
            return View();
        }
        public ActionResult ListaAsignacion(string f)
        {
            var lista = new Script_Sql().getSedaciones(f);
            return View(lista);
        }

        public ActionResult EnviarSedaciones(string[] citas, string f, int dr, string com)
        {
            var sedador = db.Com_Medico.SingleOrDefault(x => x.idCom_Medico == dr);
            var lista = new Script_Sql().getSedaciones(f);
            lista = lista.Where(x => citas.Contains(x.numerocita)).ToList();
            #region mesaje
            string titulo = @"<TABLE class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'>
     <TBODY>
      <TR>
       <TD class='padded'  style='WORD-WRAP: break-word; PADDING-LEFT: 0px; FONT-SIZE: 12px; FONT-FAMILY: Tahoma,sans-serif; VERTICAL-ALIGN: top;TEXT-ALIGN: left;'>    
                              <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'> Estimado(a) " + sedador.medico + @"</h1>
       </TD>
      </TR>
     </TBODY>
    </TABLE>
   </TD>
   <TD  style='WIDTH: 20%; VERTICAL-ALIGN: top; PADDING-BOTTOM: 32px;'>
    <TABLE class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'>
     <TBODY>
      <TR>
       <TD style='WORD-WRAP: break-word;FONT-FAMILY: Tahoma,sans-serif; VERTICAL-ALIGN: top;TEXT-ALIGN: right;'>
         <a href='#' style='TEXT-DECORATION: none; FONT-FAMILY: Tahoma,sans-serif;float: right;font-size: 15px;padding-right: 32px;'></a>
       </TD>
      </TR>
     </TBODY>
    </TABLE>"
,

                cuerpo = @"<p style='Margin-top: 0;color: #565656;font-family: Tahoma,sans-serif;font-size: 14px;line-height: 25px;Margin-bottom: 24px'>Se envía la lista de pacientes programados para el día " + f + @" <br/> <b>Estudio(s):</b>", img = "http://extranet.resocentro.com:5050/PaginaWeb/correo/Contactenostop.jpg";

            string detalle = "", tabla = @"

<TABLE  class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'><THEAD>
<TR>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Hora</TH>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Paciente</TH>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Estudio</TH>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Edad</TH>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Equipo</TH>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Anestesiólogo</TH></TR></THEAD>
<TBODY>
{0}
</TBODY></TABLE>";
            foreach (var item in lista.OrderBy(x => x.fecha).ToList())
            {
                detalle += "<TR><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px'>" + item.fecha.ToString("dd/MM/yy hh:mm tt") + "</TD><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px'>" + item.paciente + "</TD><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px'>" + item.edad + "</TD><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px'>" + item.estudio + "</TD><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px'>" + item.equipo + "</TD><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px'>" + sedador.medico + "</TD><TD></TR>";
                int ncita = int.Parse(item.numerocita);
                var x_cita = db.EXAMENXCITA.SingleOrDefault(x => x.codigoexamencita == ncita);
                if (x_cita != null)
                {
                    x_cita.sedador = sedador.codigousuario;
                    db.SaveChanges();
                }
            }
            tabla = string.Format(tabla, detalle);
            if (com != "")
            {
                tabla += "<b>Comentarios Adicionales:</b> " + com + "<br/>";
            }
            tabla += "</p>";
            #endregion
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            new Variable().sendCorreo("Relacion de Pacientes con sedacion del dia " + f, sedador.email, "jorge.herrera@resocentro.com;paola.porturas@resocentro.com;milagritoscerdan@resocentro.com;" + user.Email, new Variable().getCuerpoEmail(img, titulo, cuerpo + "" + tabla), "");
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
public class HistorialConsulta
{
    public Consulta pregunta { get; set; }
    public List<Historia> historia { get; set; }
}