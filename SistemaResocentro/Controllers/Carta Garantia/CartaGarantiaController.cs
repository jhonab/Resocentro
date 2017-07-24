using iTextSharp.text;
using iTextSharp.text.pdf;
using SistemaResocentro.Member;
using SistemaResocentro.Models;
using SistemaResocentro.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaResocentro.Controllers
{
    [Authorize(Roles = "1,2,3,4,9")]
    public class CartaGarantiaController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: CartaGarantia
        public ActionResult ListaProforma()
        {
            var lista = (from pro in db.PROFORMA
                         join pa in db.PACIENTE on pro.codigopaciente equals pa.codigopaciente
                         join cs in db.COMPANIASEGURO on pro.codigocompaniaseguro equals cs.codigocompaniaseguro
                         join ch in db.CLINICAHOSPITAL on pro.codigoclinica equals ch.codigoclinica
                         join me in db.MEDICOEXTERNO on pro.cmp equals me.cmp
                         join us in db.USUARIO on pro.codigousuario equals us.codigousuario
                         join es in db.ESPECIALIDAD on me.codigoespecialidad equals es.codigoespecialidad
                         where
                             pro.estado == "INICIADA"
                         select new Lista_ProformaViewModel
                         {
                             numero_proforma = pro.numerodeproforma,
                             fechaEmision = pro.fechaemision,
                             titular = pro.titular,
                             poliza = pro.poliza,
                             contratante = pro.contratante,
                             tiempo_enf = pro.tiempoenfermedad,
                             sedacion = pro.sedacion,
                             comentarios = pro.observacion,
                             idadjunto = pro.codigodocescaneado,
                             tramitador = us.ShortName,
                             pac = pa.apellidos + " " + pa.nombres,
                             paciente = pa,
                             companiaseguro = cs,
                             clinica = ch,
                             medico = me,
                             especialidad = es,
                         }).AsParallel().ToList();
            return View(lista);
        }
        public ActionResult RechazarCarta(int numero_proforma)
        {
            db.PROFORMA.Where(x => x.numerodeproforma == numero_proforma).SingleOrDefault().estado = "RECHAZADA";
            db.SaveChanges();
            return RedirectToAction("ListaProforma");
        }
        //GET:create Carta x proforma
        public ActionResult CartaProforma(int numero_proforma)
        {
            var _profo = db.PROFORMA.Where(x => x.numerodeproforma == numero_proforma).SingleOrDefault();
            if (_profo != null)
            {
                ViewBag.TipoAseguradora = new SelectList(new Variable().getTipoAfiliacion(), "codigo", "nombre");
                ViewBag.EstadoCarta = new SelectList(new Variable().getEstadoCarta(), "nombre", "nombre");
                ViewBag.Aseguradora = new SelectList(db.COMPANIASEGURO.Select(x => new { codigocompaniaseguro = x.codigocompaniaseguro, descripcion = x.codigocompaniaseguro + " - " + x.descripcion }).AsParallel().ToList(), "codigocompaniaseguro", "descripcion");
                ViewBag.Clinica = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).AsParallel().ToList(), "codigoclinica", "razonsocial");
                ViewBag.Vacio = new SelectList(new Variable().getVacio(), "codigo", "nombre");

                ViewBag.Clase = new SelectList(db.CLASE.ToList(), "codigoclase", "nombreclase");
                ViewBag.Medico = db.MEDICOEXTERNO.Where(x => x.cmp == _profo.cmp).Select(x => x.apellidos + " " + x.nombres).AsParallel().SingleOrDefault();
                //cargamos data
                CartaGarantiaViewModel item = new CartaGarantiaViewModel();
                item.carta = new CARTAGARANTIA();
                item.carta.codigocartagarantia = DateTime.Now.ToString("ddMMyyhhmm") + _profo.codigopaciente;
                item.carta.titular = _profo.titular;
                item.carta.contratante = _profo.contratante;
                item.carta.seguimiento = _profo.observacion;
                item.carta.poliza = _profo.poliza;
                item.carta.codigopaciente = _profo.codigopaciente;
                item.carta.PACIENTE = db.PACIENTE.Where(x => x.codigopaciente == _profo.codigopaciente).SingleOrDefault();
                item.carta.codigocompaniaseguro = _profo.codigocompaniaseguro;
                item.carta.cmp = _profo.cmp;
                item.carta.codigoclinica = _profo.codigoclinica;
                item.carta.codigoclinica2 = _profo.codigoclinica;
                item.num_proforma = _profo.numerodeproforma;
                //Cargamos los estudios
                string _uni = "", _moda = ""; int _unid = 0;
                List<Estudios_proforma> list = new List<Estudios_proforma>();
                foreach (var detalle in db.DETALLEPROFORMA.Where(x => x.numerodeproforma == _profo.numerodeproforma).ToList())
                {
                    Estudios_proforma d = new Estudios_proforma();
                    d.idestudio = detalle.codigoestudio;
                    d.idclase = detalle.codigoclase;
                    d.precio = detalle.monto;
                    d.nombre = db.ESTUDIO.Where(x => x.codigoestudio == detalle.codigoestudio).SingleOrDefault().nombreestudio;
                    list.Add(d);
                    _uni = d.idestudio.Substring(0, 3);
                    _moda = d.idestudio.Substring(4, 1);
                }

                item.list_estudios = list;
                item.estudios = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                item.idAdjunto = _profo.codigodocescaneado;
                item.Adjunto = _profo.codigodocescaneado != "" ? db.DOCESCANEADO.Where(x => x.codigodocadjunto == _profo.codigodocescaneado).SingleOrDefault().nombrearchivo : "";

                //set Unidad Negocio y Modalidad
                ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre", _uni);
                if (_uni != "")
                {
                    if (int.TryParse(_uni.Substring(0, 1), out _unid))
                        ViewBag.Modalidad = new SelectList(db.MODALIDAD.Where(x => x.codigounidad == _unid).OrderBy(x => x.codigomodalidad).AsParallel().ToList(),
                                       "codigomodalidad",
                                       "nombre", _moda);
                    else
                        ViewBag.Modalidad = new SelectList(new Variable().getVacio(), "codigo", "nombre");
                }
                else
                    ViewBag.Modalidad = new SelectList(new Variable().getVacio(), "codigo", "nombre");


                return View(item);
            }
            else
                return View(new CartaGarantiaViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CartaProforma(CartaGarantiaViewModel item)
        {
            if (item.estudios != null && item.carta != null)
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                item.carta.fechatramite = DateTime.Now;
                var _aseguradora = db.COMPANIASEGURO.Where(x => x.codigocompaniaseguro == item.carta.codigocompaniaseguro).SingleOrDefault();
                item.carta.ruc = _aseguradora.ruc;
                item.list_estudios = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Estudios_proforma>>(item.estudios);
                item.carta.monto = float.Parse(item.list_estudios.Sum(x => x.precio).ToString());
                item.carta.fechaprobacion = DateTime.Now;
                item.carta.isRevisada = false;
                item.carta.codigousuario = user.ProviderUserKey.ToString();
                item.carta.numero_proforma = item.num_proforma;
                db.CARTAGARANTIA.Add(item.carta);

                db.PROFORMA.Where(x => x.numerodeproforma == item.num_proforma).SingleOrDefault().estado = "TRAMITADA";
                db.SaveChanges();
                foreach (var detalle in item.list_estudios)
                {
                    ESTUDIO_CARTAGAR det = new ESTUDIO_CARTAGAR();
                    det.codigoestudio = detalle.idestudio;
                    det.codigocartagarantia = item.carta.codigocartagarantia;
                    det.cmp = item.carta.cmp;
                    det.codigocompaniaseguro = item.carta.codigocompaniaseguro;
                    det.ruc = item.carta.ruc;
                    det.codigopaciente = item.carta.codigopaciente;
                    det.codigoclase = detalle.idclase;
                    db.ESTUDIO_CARTAGAR.Add(det);
                    db.SaveChanges();
                }

                if (item.carta.codigodocadjunto == "")
                    return RedirectToAction("AdjuntarArchivoProforma", new { numeroproforma = item.carta.codigocartagarantia, tipo = 2 });
                return RedirectToAction("ListaProforma");
            }
            else
            {
                ViewBag.TipoAseguradora = new SelectList(new Variable().getTipoAfiliacion(), "codigo", "nombre");
                ViewBag.EstadoCarta = new SelectList(new Variable().getEstadoCarta(), "nombre", "nombre");
                ViewBag.Aseguradora = new SelectList(db.COMPANIASEGURO.Select(x => new { codigocompaniaseguro = x.codigocompaniaseguro, descripcion = x.codigocompaniaseguro + " - " + x.descripcion }).AsParallel().ToList(), "codigocompaniaseguro", "descripcion");
                ViewBag.Clinica = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).AsParallel().ToList(), "codigoclinica", "razonsocial");
                ViewBag.Vacio = new SelectList(new Variable().getVacio(), "codigo", "nombre");

                ViewBag.Medico = db.MEDICOEXTERNO.Where(x => x.cmp == item.carta.cmp).Select(x => x.apellidos + " " + x.nombres).AsParallel().SingleOrDefault();
                ViewBag.Clase = new SelectList(db.CLASE.ToList(), "codigoclase", "nombreclase");

                item.carta.PACIENTE = db.PACIENTE.Where(x => x.codigopaciente == item.carta.codigopaciente).SingleOrDefault();

                //Cargamos los estudios
                string _uni = "", _moda = ""; int _unid = 0;
                List<Estudios_proforma> list = new List<Estudios_proforma>();
                foreach (var detalle in db.DETALLEPROFORMA.Where(x => x.numerodeproforma == item.num_proforma).ToList())
                {
                    Estudios_proforma d = new Estudios_proforma();
                    d.idestudio = detalle.codigoestudio;
                    d.idclase = detalle.codigoclase;
                    d.precio = detalle.monto;
                    d.nombre = db.ESTUDIO.Where(x => x.codigoestudio == detalle.codigoestudio).SingleOrDefault().nombreestudio;
                    list.Add(d);
                    _uni = d.idestudio.Substring(0, 3);
                    _moda = d.idestudio.Substring(4, 1);
                }

                item.list_estudios = list;
                item.estudios = Newtonsoft.Json.JsonConvert.SerializeObject(list);

                //set Unidad Negocio y Modalidad
                ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre", _uni);
                if (_uni != "")
                {
                    if (int.TryParse(_uni.Substring(0, 1), out _unid))
                        ViewBag.Modalidad = new SelectList(db.MODALIDAD.Where(x => x.codigounidad == _unid).OrderBy(x => x.codigomodalidad).AsParallel().ToList(),
                                       "codigomodalidad",
                                       "nombre", _moda);
                    else
                        ViewBag.Modalidad = new SelectList(new Variable().getVacio(), "codigo", "nombre");
                }
                else
                    ViewBag.Modalidad = new SelectList(new Variable().getVacio(), "codigo", "nombre");
                return View(item);
            }
        }
        public ActionResult CreateProforma()
        {
            ViewBag.Aseguradora = new SelectList(db.COMPANIASEGURO.Select(x => new { codigocompaniaseguro = x.codigocompaniaseguro, descripcion = x.codigocompaniaseguro + " - " + x.descripcion }).AsParallel().ToList(), "codigocompaniaseguro", "descripcion");
            ViewBag.Clinica = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).AsParallel().ToList(), "codigoclinica", "razonsocial");

            ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre");

            ViewBag.Clase = new SelectList(db.CLASE.ToList(), "codigoclase", "nombreclase");
            ProformaViewModel pro = new ProformaViewModel();
            pro.list_estudio = new List<Estudios_proforma>();
            return View(pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProforma(ProformaViewModel item)
        {

            if (item.estudios != null && item.proforma != null)
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

                item.list_estudio = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Estudios_proforma>>(item.estudios);
                var _aseguradora = db.COMPANIASEGURO.Where(x => x.codigocompaniaseguro == item.proforma.codigocompaniaseguro).SingleOrDefault();
                item.proforma.ruc = _aseguradora.ruc;
                if (item.proforma.tiempoenfermedad != "")
                    item.proforma.tiempoenfermedad += " dias";
                item.proforma.estado = "INICIADA";
                item.proforma.fechaemision = DateTime.Now;
                item.proforma.codigousuario = user.ProviderUserKey.ToString();
                item.proforma.codigodocescaneado = "";
                item.proforma.montototal = float.Parse(item.list_estudio.Sum(x => x.precio).ToString());

                db.PROFORMA.Add(item.proforma);
                db.SaveChanges();
                foreach (var detalle in item.list_estudio)
                {
                    DETALLEPROFORMA det = new DETALLEPROFORMA();
                    det.monto = float.Parse(detalle.precio.ToString());
                    det.codigoestudio = detalle.idestudio;
                    det.codigoclase = detalle.idclase;
                    det.numerodeproforma = item.proforma.numerodeproforma;
                    db.DETALLEPROFORMA.Add(det);
                    db.SaveChanges();
                }

                return RedirectToAction("AdjuntarArchivoProforma", new { numeroproforma = item.proforma.numerodeproforma, tipo = 1 });
            }
            else
            {
                item.list_estudio = new List<Estudios_proforma>();

                ViewBag.Aseguradora = new SelectList(db.COMPANIASEGURO.Select(x => new { codigocompaniaseguro = x.codigocompaniaseguro, descripcion = x.codigocompaniaseguro + " - " + x.descripcion }).AsParallel().ToList(), "codigocompaniaseguro", "descripcion");
                ViewBag.Clinica = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).AsParallel().ToList(), "codigoclinica", "razonsocial");

                ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre");

                ViewBag.Clase = new SelectList(db.CLASE.ToList(), "codigoclase", "nombreclase");
                ModelState.AddModelError("", "Ocurrio un error vuelva a intenterlo. ");
                return View();
            }

        }
        public ActionResult AdjuntarArchivoProforma(int numeroproforma, int tipo)
        {
            ViewBag.Id = numeroproforma;
            ViewBag.Tipo = tipo;
            return View();
        }
        public ActionResult ImprimirProforma(int numeroproforma)
        {
            ProformaViewModel item = new ProformaViewModel();
            item.proforma = db.PROFORMA.Where(x => x.numerodeproforma == numeroproforma).SingleOrDefault();
            var pac = db.PACIENTE.Where(x => x.codigopaciente == item.proforma.codigopaciente).SingleOrDefault();
            var ase = db.COMPANIASEGURO.Where(x => x.codigocompaniaseguro == item.proforma.codigocompaniaseguro).SingleOrDefault();
            var med = db.MEDICOEXTERNO.Where(x => x.cmp == item.proforma.cmp).SingleOrDefault();
            var cli = db.CLINICAHOSPITAL.Where(x => x.codigoclinica == item.proforma.codigoclinica).SingleOrDefault();
            item.paciente = pac.apellidos + ", " + pac.nombres;
            item.aseguradora = ase.descripcion;
            item.medico = med.apellidos + ", " + med.nombres;
            item.clinica = cli.razonsocial;
            List<Estudios_proforma> lista = new List<Estudios_proforma>();
            foreach (var est in db.DETALLEPROFORMA.Where(x => x.numerodeproforma == numeroproforma).ToList())
            {
                var estudio = db.ESTUDIO.Where(x => x.codigoestudio == est.codigoestudio).SingleOrDefault();
                Estudios_proforma _est = new Estudios_proforma();
                _est.nombre = estudio.nombreestudio;
                _est.precio = est.monto;
                lista.Add(_est);
            }
            item.list_estudio = lista;
            return View(item);
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
                if (tipo == "1")//PROFORMA
                {
                    var idproforma = int.Parse(numero);
                    var _proforma = db.PROFORMA.Where(x => x.numerodeproforma == idproforma).SingleOrDefault();
                    var _paciente = db.PACIENTE.Where(x => x.codigopaciente == _proforma.codigopaciente).SingleOrDefault();
                    DOCESCANEADO doc = new DOCESCANEADO();
                    doc.codigodocadjunto = (DateTime.Now.ToString("ddMMyyyyHHmm") + "-" + _proforma.codigopaciente.ToString());
                    doc.nombrearchivo = _paciente.apellidos + " " + _paciente.nombres + ".pdf";
                    doc.cuerpoarchivo = buffer;
                    doc.fecharegistro = DateTime.Now;
                    doc.codigousuario = user.ProviderUserKey.ToString();
                    _proforma.codigodocescaneado = doc.codigodocadjunto;
                    db.DOCESCANEADO.Add(doc);
                    db.SaveChanges();
                }
                if (tipo == "2")
                {
                    var idcarta = numero;
                    var _carta = db.CARTAGARANTIA.Where(x => x.codigocartagarantia == idcarta).SingleOrDefault();
                    var _paciente = db.PACIENTE.Where(x => x.codigopaciente == _carta.codigopaciente).SingleOrDefault();
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
        public ActionResult UpdaterArchivos(string idDoc)
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

                var _documento = db.DOCESCANEADO.Where(x => x.codigodocadjunto == idDoc).SingleOrDefault();
                _documento.cuerpoarchivo = buffer;
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
        public ActionResult getModalidad(int idunidad)
        {
            return Json(new SelectList(db.MODALIDAD.Where(x => x.codigounidad == idunidad).OrderBy(x => x.codigomodalidad).AsParallel().ToList(),
                                       "codigomodalidad",
                                       "nombre"),
                                       JsonRequestBehavior.AllowGet);
        }
        public ActionResult DescargarAdjunto(string id, string tipo)
        {
            if (tipo == "1")
            {
                var archivo = db.DOCESCANEADO.Where(x => x.codigodocadjunto == id).SingleOrDefault();
                if (archivo.isFisico)
                    return Redirect("http://192.168.0.5:5002/" + archivo.codigodocadjunto + "/" + archivo.nombrearchivo.Replace(" ", "%20") + "");
                else
                    return File(archivo.cuerpoarchivo, "application/pdf", archivo.nombrearchivo);

            }
            else
            {
                return View();
            }
        }
        public ActionResult getEstudio(string idunidad, string idmodalidad, int idclase, int aseguradora)
        {
            var list = (from e in db.EQUIPO_ESTUDIO
                        where
                        (e.bloqueado == false &&
                        e.ESTUDIO.codigoclase == idclase &&
                        e.codigoestudio.Substring(5, 1) == "0" &&
                        e.ESTUDIO.codigoestudio.Substring(4, 1) == idmodalidad &&
                        e.ESTUDIO.codigoestudio.Substring(0, 3) == idunidad) &&
                        e.ESTUDIO.codigoestudio.Substring(7, 2) != "99"
                        orderby e.ESTUDIO.nombreestudio
                        group e by new
                        {
                            e.ESTUDIO.codigoestudio,
                            e.ESTUDIO.nombreestudio,
                            e.ESTUDIO.codigoclase
                        } into estudios
                        select new Estudios_proforma
                        {
                            idestudio = estudios.Key.codigoestudio,
                            idclase = estudios.Key.codigoclase,
                            nombre = estudios.Key.nombreestudio
                        }).AsParallel().ToList();
            foreach (var item in list)
            {
                item.precio = (from x in db.ESTUDIO_COMPANIA
                               where x.codigoestudio == item.idestudio &&
                               x.codigocompaniaseguro == aseguradora
                               select x.preciobruto).SingleOrDefault();
            }
            return View(list);
        }
        public ActionResult VerificarAseguradora(int idaseguradora)
        {
            return Json(new
            {
                //Producto
                producto = db.SitedProducto
                .Where(x => x.codigocompaniaseguro == idaseguradora)
                .AsParallel()
                .ToList()
                .Count() > 0,

                list_producto = new SelectList(db.SitedProducto.Where(x => x.codigocompaniaseguro == idaseguradora && x.IsEnabled == true)
                    .Select(x => new { codigo = x.SitedProductoId, value = x.SitedCodigoProducto + " - " + x.descripcion }).OrderBy(x => x.value).AsParallel().ToList(),
                    "codigo",
                    "value"),

                //Beneficio
                list_beneficio = idaseguradora != 37 ? new SelectList(db.Sunasa_Cobertura.Where(x => x.codigocompaniaseguro == 0 && x.IsActive == true)
                    .Select(x => new { codigo = x.Sunasa_CoberturaId, value = x.Nombre }).OrderBy(x => x.value).AsParallel().ToList(),
                    "codigo",
                    "value") : new SelectList(new Variable().getVacio(), "", "")
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getBeneficio(int idaseguradora, int idproducto)
        {
            return Json(new SelectList((from x in db.Sunasa_Cobertura
                                        where x.codigocompaniaseguro == idaseguradora
                                               && x.SitedProductoId == idproducto
                                               && x.IsActive == true
                                        select x).AsQueryable(),
                                     "Sunasa_CoberturaId",
                                     "Nombre"),
                                     JsonRequestBehavior.AllowGet);


        }
        public ActionResult validarCodigoCarta(string codigo)
        {
            return Json((from ca in db.CARTAGARANTIA
                         where ca.codigocartagarantia == codigo

                         select ca).AsParallel().ToList().Count <= 0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateCarta()
        {
            ViewBag.TipoAseguradora = new SelectList(new Variable().getTipoAfiliacion(), "codigo", "nombre");
            ViewBag.EstadoCarta = new SelectList(new Variable().getEstadoCarta(), "nombre", "nombre");
            ViewBag.Aseguradora = new SelectList(db.COMPANIASEGURO.Select(x => new { codigocompaniaseguro = x.codigocompaniaseguro, descripcion = x.codigocompaniaseguro + " - " + x.descripcion }).AsParallel().ToList(), "codigocompaniaseguro", "descripcion");
            ViewBag.Clinica = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).AsParallel().ToList(), "codigoclinica", "razonsocial");
            ViewBag.Vacio = new SelectList(new Variable().getVacio(), "codigo", "nombre");


            ViewBag.Clase = new SelectList(db.CLASE.ToList(), "codigoclase", "nombreclase");
            ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre");
            CartaGarantiaViewModel pro = new CartaGarantiaViewModel();
            pro.list_estudios = new List<Estudios_proforma>();
            return View(pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCarta(CartaGarantiaViewModel item)
        {
            if (item.estudios != null && item.carta != null)
            {
                item.carta.fechatramite = DateTime.Now;
                var _aseguradora = db.COMPANIASEGURO.Where(x => x.codigocompaniaseguro == item.carta.codigocompaniaseguro).SingleOrDefault();
                item.carta.ruc = _aseguradora.ruc;
                item.list_estudios = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Estudios_proforma>>(item.estudios);
                item.carta.monto = float.Parse(item.list_estudios.Sum(x => x.precio).ToString());
                item.carta.fechaprobacion = DateTime.Now;
                item.carta.isRevisada = false;
                db.CARTAGARANTIA.Add(item.carta);

                db.SaveChanges();
                foreach (var detalle in item.list_estudios)
                {
                    ESTUDIO_CARTAGAR det = new ESTUDIO_CARTAGAR();
                    det.codigoestudio = detalle.idestudio;
                    det.codigocartagarantia = item.carta.codigocartagarantia;
                    det.cmp = item.carta.cmp;
                    det.codigocompaniaseguro = item.carta.codigocompaniaseguro;
                    det.ruc = item.carta.ruc;
                    det.codigopaciente = item.carta.codigopaciente;
                    det.codigoclase = detalle.idclase;
                    db.ESTUDIO_CARTAGAR.Add(det);
                    db.SaveChanges();
                }

                if (item.carta.codigodocadjunto == null)
                    return RedirectToAction("AdjuntarArchivoProforma", new { numeroproforma = item.carta.codigocartagarantia, tipo = 2 });
                return RedirectToAction("ListaProforma");
            }
            else
            {
                ViewBag.TipoAseguradora = new SelectList(new Variable().getTipoAfiliacion(), "codigo", "nombre");
                ViewBag.EstadoCarta = new SelectList(new Variable().getEstadoCarta(), "nombre", "nombre");
                ViewBag.Aseguradora = new SelectList(db.COMPANIASEGURO.Select(x => new { codigocompaniaseguro = x.codigocompaniaseguro, descripcion = x.codigocompaniaseguro + " - " + x.descripcion }).AsParallel().ToList(), "codigocompaniaseguro", "descripcion");
                ViewBag.Clinica = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).AsParallel().ToList(), "codigoclinica", "razonsocial");
                ViewBag.Vacio = new SelectList(new Variable().getVacio(), "codigo", "nombre");


                ViewBag.Clase = new SelectList(db.CLASE.ToList(), "codigoclase", "nombreclase");
                ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre");
                CartaGarantiaViewModel pro = new CartaGarantiaViewModel();
                pro.list_estudios = new List<Estudios_proforma>();
                return View(pro);
            }
        }
        public ActionResult getListCarta(int idpaciente)
        {
            return View(db.CARTAGARANTIA.Where(x => x.codigopaciente == idpaciente).ToList());
        }

        public ActionResult VisorCartas()
        {
            return View();
        }
        public ActionResult VisorxTipo(int tipo, DateTime fecha, DateTime fini, string paciente, DateTime ffin)
        {
            List<visordecartas_Result> _list = new List<visordecartas_Result>();
            if (tipo == 1)
            {
                _list = db.visordecartas(tipo, 1, fecha.Day, fecha.Month, fecha.Year, "", "", "").AsParallel().ToList();
            }
            if (tipo == 2)
            {
                _list = db.visordecartas(tipo, 1, 0, 0, 0, fini.ToString("dd/MM/yyyy") + " 00:01", "", ffin.ToString("dd/MM/yyyy") + " 23:59").AsParallel().ToList();
            }
            List<List_VisorCarta> lista = new List<List_VisorCarta>();
            foreach (var item in _list)
            {
                List_VisorCarta vc = new List_VisorCarta();
                vc.tramite = item.Tramite;
                vc.idPaciente = item.ID;
                vc.paciente = item.Paciente;
                vc.estudio = item.Estudio;
                vc.telefono = item.Telefono;
                vc.aseguradora = item.Aseguradora;
                vc.cobertura = item.C__Cob_;
                vc.estado = item.Estado;
                vc.idcarta = item.Codigo;
                vc.usu_reg = item.Usuario;
                vc.fec_update = item.fec_update;
                vc.usu_update = item.user_update;
                vc.usu_cita = item.user_cita;
                vc.fec_aprueba = item.FEC__APROB_;
                vc.obs = item.Observacion;
                lista.Add(vc);
            }
            return View(lista);
        }
        public ActionResult ActualizarCarta()
        {
            return View();
        }
        public ActionResult UpdateCarta(string idcarta, int idpaciente, bool? isRevisada)
        {
            var carta = db.CARTAGARANTIA.Where(x => x.codigocartagarantia == idcarta & x.codigopaciente == idpaciente).SingleOrDefault();
            ViewBag.TipoAseguradora = new SelectList(new Variable().getTipoAfiliacion(), "codigo", "nombre");
            ViewBag.EstadoCarta = new SelectList(new Variable().getEstadoCarta(), "nombre", "nombre");
            ViewBag.Aseguradora = new SelectList(db.COMPANIASEGURO.Select(x => new { codigocompaniaseguro = x.codigocompaniaseguro, descripcion = x.codigocompaniaseguro + " - " + x.descripcion }).AsParallel().ToList(), "codigocompaniaseguro", "descripcion");
            ViewBag.Clinica = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).AsParallel().ToList(), "codigoclinica", "razonsocial");
            ViewBag.Vacio = new SelectList(new Variable().getVacio(), "codigo", "nombre");
            ViewBag.Medico = db.MEDICOEXTERNO.Where(x => x.cmp == carta.cmp).Select(x => x.apellidos + " " + x.nombres).AsParallel().SingleOrDefault();
            ViewBag.CIE = db.CIE.Where(x => x.codigo == carta.cie).Select(x => x.descripcion).AsParallel().SingleOrDefault();

            ViewBag.Beneficio = db.Sunasa_Cobertura.Where(x => x.Sunasa_CoberturaId == carta.Sunasa_CoberturaId)
                    .Select(x => x.Nombre).AsParallel().SingleOrDefault();
            int idproducto = 0;
            if (int.TryParse(carta.SitedCodigoProducto, out idproducto))
            {
                ViewBag.Producto = new SelectList(db.SitedProducto.Where(x => x.codigocompaniaseguro == carta.codigocompaniaseguro && x.IsEnabled == true)
                    .Select(x => new { codigo = x.SitedProductoId, value = x.SitedCodigoProducto + " - " + x.descripcion }).OrderBy(x => x.value).AsParallel().ToList(),
                    "codigo",
                    "value");
            }
            else
            {
                ViewBag.Producto = new SelectList(new Variable().getVacio(),
                    "codigo",
                    "nombre");
            }

            ViewBag.Clase = new SelectList(db.CLASE.ToList(), "codigoclase", "nombreclase");
            ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre");
            CartaGarantiaViewModel pro = new CartaGarantiaViewModel();
            pro.isRevisada = isRevisada;
            pro.carta = new CARTAGARANTIA();

            //PACIENTE
            pro.carta.codigopaciente = carta.codigopaciente;
            pro.carta.TipoAfiliacion = carta.TipoAfiliacion;
            pro.carta.PACIENTE = carta.PACIENTE;
            //COMPANIA
            pro.carta.codigocompaniaseguro = carta.codigocompaniaseguro;
            pro.carta.ruc = carta.ruc;

            //CARTAGARANTIA
            pro.carta.titular = carta.titular;
            pro.carta.poliza = carta.poliza;
            pro.carta.contratante = carta.contratante;
            pro.carta.numerocarnetseguro = carta.numerocarnetseguro;
            pro.carta.codigocartagarantia = carta.codigocartagarantia;
            pro.carta.codigocartagarantia = carta.codigocartagarantia;
            pro.carta.codigocartagarantia2 = carta.codigocartagarantia2;
            pro.carta.estadocarta = carta.estadocarta;
            pro.carta.cobertura = carta.cobertura;
            pro.carta.seguimiento = carta.seguimiento;
            pro.carta.SitedCodigoProducto = carta.SitedCodigoProducto;
            pro.carta.Sunasa_CoberturaId = carta.Sunasa_CoberturaId;
            pro.carta.Sunasa_CoberturaId = carta.Sunasa_CoberturaId;
            pro.carta.cie = carta.cie;
            pro.carta.codigoclinica = carta.codigoclinica;
            pro.carta.codigoclinica2 = carta.codigoclinica2;
            pro.carta.cmp = carta.cmp;
            pro.carta.codigodocadjunto = carta.codigodocadjunto;
            pro.carta.isRevisada = carta.isRevisada;

            string _uni = "", _moda = ""; int _unid = 0;
            List<Estudios_proforma> list = new List<Estudios_proforma>();
            foreach (var _est in db.ESTUDIO_CARTAGAR.Where(x => x.codigocartagarantia == idcarta && x.codigopaciente == idpaciente).ToList())
            {
                Estudios_proforma d = new Estudios_proforma();
                d.idestudio = _est.codigoestudio;
                d.idclase = _est.codigoclase;
                d.precio = 0;
                if(_est.cobertura_det!=null)
                d.cob = Convert.ToDouble(_est.cobertura_det);
                d.nombre = db.ESTUDIO.Where(x => x.codigoestudio == _est.codigoestudio).SingleOrDefault().nombreestudio;
                list.Add(d);
                _uni = d.idestudio.Substring(0, 3);
                _moda = d.idestudio.Substring(4, 1);

            }

            pro.list_estudios = list;
            pro.estudios = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            pro.idAdjunto = carta.codigodocadjunto;
            pro.Adjunto = carta.codigodocadjunto != "" ? db.DOCESCANEADO.Where(x => x.codigodocadjunto == carta.codigodocadjunto).SingleOrDefault().nombrearchivo : "";

            //set Unidad Negocio y Modalidad
            ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre", _uni);
            if (_uni != "")
            {
                if (int.TryParse(_uni.Substring(0, 1), out _unid))

                    ViewBag.Modalidad = new SelectList(db.MODALIDAD.Where(x => x.codigounidad == _unid).OrderBy(x => x.codigomodalidad).AsParallel().ToList(),
                                   "codigomodalidad",
                                   "nombre", _moda);
                else
                    ViewBag.Modalidad = new SelectList(new Variable().getVacio(), "codigo", "nombre");
            }
            else
                ViewBag.Modalidad = new SelectList(new Variable().getVacio(), "codigo", "nombre");
            return View(pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCarta(CartaGarantiaViewModel item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            if (item.estudios != null && item.carta != null)
            {
                var _cg = (from ca in db.CARTAGARANTIA
                           where ca.codigocartagarantia == item.carta.codigocartagarantia
                           && ca.codigopaciente == item.carta.codigopaciente
                           select ca).SingleOrDefault();
                _cg.seguimiento = item.carta.seguimiento + "";
                _cg.estadocarta = item.carta.estadocarta;
                _cg.contratante = item.carta.contratante;
                _cg.titular = item.carta.titular;
                _cg.poliza = item.carta.poliza;
                _cg.cmp = item.carta.cmp;
                _cg.codigopaciente = item.carta.codigopaciente;
                _cg.cobertura = item.carta.cobertura;
                _cg.monto = item.carta.monto;
                if (_cg.estadocarta == "APROBADA")
                    _cg.fechaprobacion = DateTime.Now;
                _cg.codigousuario = user.ProviderUserKey.ToString();
                _cg.numerocarnetseguro = item.carta.numerocarnetseguro;
                _cg.codigodocadjunto = item.carta.codigodocadjunto;
                _cg.cie = item.carta.cie;
                _cg.codigocartagarantia2 = item.carta.codigocartagarantia2;
                _cg.beneficio = item.carta.beneficio;
                _cg.Sunasa_CoberturaId = item.carta.Sunasa_CoberturaId;
                _cg.SitedCodigoProducto = item.carta.SitedCodigoProducto;
                _cg.codigoclinica = item.carta.codigoclinica;
                _cg.codigoclinica2 = item.carta.codigoclinica2;
                _cg.TipoAfiliacion = item.carta.TipoAfiliacion;
                _cg.user_update = user.ProviderUserKey.ToString();
                _cg.fec_update = DateTime.Now;
                _cg.isRevisada = item.isRevisada;
                db.SaveChanges();


                foreach (var dt in db.ESTUDIO_CARTAGAR.Where(x => x.codigocartagarantia == item.carta.codigocartagarantia && x.codigopaciente == item.carta.codigopaciente).ToList())
                {
                    db.ESTUDIO_CARTAGAR.Remove(dt);
                    db.SaveChanges();
                }
                item.list_estudios = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Estudios_proforma>>(item.estudios);
                foreach (var detalle in item.list_estudios)
                {
                    ESTUDIO_CARTAGAR det = new ESTUDIO_CARTAGAR();
                    det.codigoestudio = detalle.idestudio;
                    det.codigocartagarantia = item.carta.codigocartagarantia;
                    det.cmp = item.carta.cmp;
                    det.codigocompaniaseguro = item.carta.codigocompaniaseguro;
                    det.ruc = _cg.ruc;
                    det.codigopaciente = item.carta.codigopaciente;
                    det.codigoclase = detalle.idclase;
                    db.ESTUDIO_CARTAGAR.Add(det);
                    db.SaveChanges();
                }

                if (item.carta.codigodocadjunto == "")
                    return RedirectToAction("AdjuntarArchivoProforma", new { numeroproforma = item.carta.codigocartagarantia, tipo = 2 });

                return RedirectToAction("ActualizarCarta", "CartaGarantia");

            }
            else
            {
                ViewBag.TipoAseguradora = new SelectList(new Variable().getTipoAfiliacion(), "codigo", "nombre");
                ViewBag.EstadoCarta = new SelectList(new Variable().getEstadoCarta(), "n<ombre", "nombre");
                ViewBag.Aseguradora = new SelectList(db.COMPANIASEGURO.Select(x => new { codigocompaniaseguro = x.codigocompaniaseguro, descripcion = x.codigocompaniaseguro + " - " + x.descripcion }).AsParallel().ToList(), "codigocompaniaseguro", "descripcion");
                ViewBag.Clinica = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).AsParallel().ToList(), "codigoclinica", "razonsocial");
                ViewBag.Vacio = new SelectList(new Variable().getVacio(), "codigo", "nombre");

                ViewBag.Medico = db.MEDICOEXTERNO.Where(x => x.cmp == item.carta.cmp).Select(x => x.apellidos + " " + x.nombres).AsParallel().SingleOrDefault();
                ViewBag.CIE = db.CIE.Where(x => x.codigo == item.carta.cie).Select(x => x.descripcion).AsParallel().SingleOrDefault();

                ViewBag.Beneficio = db.Sunasa_Cobertura.Where(x => x.Sunasa_CoberturaId == item.carta.Sunasa_CoberturaId)
                        .Select(x => x.Nombre).AsParallel().SingleOrDefault();
                ViewBag.Clase = new SelectList(db.CLASE.ToList(), "codigoclase", "nombreclase");

                item.carta.PACIENTE = db.PACIENTE.Where(x => x.codigopaciente == item.carta.codigopaciente).SingleOrDefault();

                //Cargamos los estudios
                string _uni = "", _moda = ""; int _unid = 0;
                List<Estudios_proforma> list = new List<Estudios_proforma>();
                foreach (var detalle in db.DETALLEPROFORMA.Where(x => x.numerodeproforma == item.num_proforma).ToList())
                {
                    Estudios_proforma d = new Estudios_proforma();
                    d.idestudio = detalle.codigoestudio;
                    d.idclase = detalle.codigoclase;
                    d.precio = detalle.monto;
                    d.nombre = db.ESTUDIO.Where(x => x.codigoestudio == detalle.codigoestudio).SingleOrDefault().nombreestudio;
                    list.Add(d);
                    _uni = d.idestudio.Substring(0, 3);
                    _moda = d.idestudio.Substring(4, 1);
                }

                item.list_estudios = list;
                item.estudios = Newtonsoft.Json.JsonConvert.SerializeObject(list);

                //set Unidad Negocio y Modalidad
                ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre", _uni);
                if (_uni != "")
                {
                    if (int.TryParse(_uni.Substring(0, 1), out _unid))
                        ViewBag.Modalidad = new SelectList(db.MODALIDAD.Where(x => x.codigounidad == _unid).OrderBy(x => x.codigomodalidad).AsParallel().ToList(),
                                       "codigomodalidad",
                                       "nombre", _moda);
                    else
                        ViewBag.Modalidad = new SelectList(new Variable().getVacio(), "codigo", "nombre");
                }
                else
                    ViewBag.Modalidad = new SelectList(new Variable().getVacio(), "codigo", "nombre");
                return View(item);
            }
        }
        public ActionResult RevisarCarta(string estado, DateTime fecha)
        {
            ViewBag.EstadoCarta = new SelectList(new Variable().getEstadoCarta(), "nombre", "nombre", estado);
            IQueryable<Carta_list_Revisar> lista = (from ca in db.CARTAGARANTIA
                                                    join ec in db.EXAMENXCITA on new { codigocarta = ca.codigocartagarantia, paciente = ca.codigopaciente } equals new { codigocarta = ec.CITA.codigocartagarantia, paciente = ec.CITA.codigopaciente } into ec_join
                                                    from ec in
                                                        (from f in ec_join where f.estadoestudio != "X" select f).DefaultIfEmpty()
                                                    join est in db.ESTUDIO_CARTAGAR on new { codigocarta = ca.codigocartagarantia, paciente = ca.codigopaciente } equals new { codigocarta = est.codigocartagarantia, paciente = est.codigopaciente } into est_join
                                                    from est in est_join.DefaultIfEmpty()
                                                    where ca.isRevisada == false
                                                && est.codigoestudio.Substring(0, 1) == "1"

                                                    group ca by new
                                                    {
                                                        ca.codigocartagarantia,
                                                        ca.fechatramite,
                                                        ca.estadocarta,
                                                        ca.COMPANIASEGURO.descripcion,
                                                        ca.cobertura,
                                                        ca.codigocartagarantia2,
                                                        ca.codigodocadjunto,
                                                        ca.codigopaciente,
                                                        paciente = ca.PACIENTE.apellidos + " " + ca.PACIENTE.nombres,
                                                        reserva = ec.CITA.fechareserva,
                                                        tramitador = ca.USUARIO1.ShortName,
                                                        ca.fec_update,
                                                        update = ca.USUARIO2.ShortName,
                                                        ca.user_revisa,
                                                        ca.fec_revisa
                                                    } into car
                                                    select new Carta_list_Revisar
                                                    {
                                                        id_carta = car.Key.codigocartagarantia,
                                                        fechaTramite = car.Key.fechatramite,
                                                        estado = car.Key.estadocarta,
                                                        aseguradora = car.Key.descripcion,
                                                        cobertura = car.Key.cobertura,
                                                        num_carta = car.Key.codigocartagarantia2,
                                                        idadjunto = car.Key.codigodocadjunto,
                                                        idpaciente = car.Key.codigopaciente,
                                                        paciente = car.Key.paciente,
                                                        iscitado = car.Key.reserva == null ? false : true,
                                                        isAdjunto = car.Key.codigodocadjunto == "" ? false : true,
                                                        user_tramita = car.Key.tramitador,
                                                        fec_update = car.Key.fec_update,
                                                        user_update = car.Key.update,
                                                        fec_cita = car.Key.reserva,
                                                        user_revisa = car.Key.user_revisa,
                                                        fec_revisa = car.Key.fec_revisa
                                                    });
            List<Carta_list_Revisar> lista_result = new List<Carta_list_Revisar>();
            if (estado == "CITADA")
                lista_result = lista.Where(x => x.estado == estado && (x.fec_cita.Value.Year == fecha.Year && x.fec_cita.Value.Month == fecha.Month && x.fec_cita.Value.Day == fecha.Day)).ToList();
            else
                lista_result = lista.Where(x => x.estado == estado && (x.fechaTramite.Year == fecha.Year && x.fechaTramite.Month == fecha.Month && x.fechaTramite.Day == fecha.Day)).ToList();

            return View(lista_result);
        }

         [Authorize(Roles = "9")]
        public ActionResult Herramientas()
        {
            return View();
        }

        public ActionResult CambiarCartaxCita(int cita, string carta, int paciente)
        {
            var _carta = db.CARTAGARANTIA.Where(x => x.codigocartagarantia == carta && x.codigopaciente == paciente).SingleOrDefault();
            var _cita = db.CITA.Where(x => x.numerocita == cita && x.codigopaciente == paciente).SingleOrDefault();
            if (_carta != null && _cita != null)
            {
                using (db)
                {
                    db.Database.Connection.Open();
                    var command = db.Database.Connection.CreateCommand();
                    command.CommandText = "dbo.cambiarCarta_Cita";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter parameter1 = new SqlParameter();
                    parameter1.ParameterName = "@ncita";
                    parameter1.SqlDbType = SqlDbType.Int;
                    parameter1.Direction = ParameterDirection.Input;
                    parameter1.Value = cita;
                    command.Parameters.Add(parameter1);
                    SqlParameter parameter2 = new SqlParameter();
                    parameter2.ParameterName = "@codigocarta";
                    parameter2.SqlDbType = SqlDbType.VarChar;
                    parameter2.Direction = ParameterDirection.Input;
                    parameter2.Value = carta;
                    command.Parameters.Add(parameter2);
                    SqlParameter parameter3 = new SqlParameter();
                    parameter3.ParameterName = "@paciente";
                    parameter3.SqlDbType = SqlDbType.VarChar;
                    parameter3.Direction = ParameterDirection.Input;
                    parameter3.Value = paciente;
                    command.Parameters.Add(parameter3);
                    var test = (command.ExecuteScalar());
                    db.Database.Connection.Close();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }
    }

}