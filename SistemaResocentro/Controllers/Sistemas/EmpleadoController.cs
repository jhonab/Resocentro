using SistemaResocentro.Member;
using SistemaResocentro.Models;
using SistemaResocentro.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace SistemaResocentro.Controllers.Sistemas
{
    [Authorize]
    public class EmpleadoController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: Empleado
        public ActionResult Update(int tipo, string valor)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var emp = db.EMPLEADO.Where(x => x.dni == user.dni).SingleOrDefault();
            if (tipo == 1)
                emp.email = valor;
            else if (tipo == 2)
                emp.telefono = valor;
            else { }
            try
            {
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult List()
        {
            return View(db.EMPLEADO.Include("USUARIO").ToList());
        }
        public ActionResult Create()
        {
            ViewBag.EstadoCivil = new SelectList(new Variable().getEstadoCivil(), "codigo", "codigo");
            ViewBag.Sexo = new SelectList(new Variable().getSexo(), "codigo", "nombre");
            ViewBag.Cargo = new SelectList(db.CARGO.ToList().OrderBy(x => x.descripcion), "codigocargo", "descripcion");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmpleadoViewModel item)
        {
            try
            {

                item.emp.estadoactual = "LABORANDO";
                item.emp.modalidad = "PLANILLA";
                item.emp.direccion = "Lima";
                item.emp.celular = "";
                item.emp.telefono = "";
                item.emp.cant_hijo = 0;
                item.emp.dni_corregido = item.emp.dni;
                db.EMPLEADO.Add(item.emp);

                item.usu.dni = item.emp.dni;
                item.usu.estadousuario = item.emp.estadoactual;
                item.usu.bloqueado = false;
                item.usu.contrasena = item.usu.codigousuario;
                db.USUARIO.Add(item.usu);
                db.SaveChanges();
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.EstadoCivil = new SelectList(new Variable().getEstadoCivil(), "codigo", "codigo");
                ViewBag.Sexo = new SelectList(new Variable().getSexo(), "codigo", "nombre");
                ViewBag.Cargo = new SelectList(db.CARGO.ToList().OrderBy(x => x.descripcion), "codigocargo", "descripcion");
                return View();
            }

        }
        public ActionResult Detalle(string dni)
        {
            EmpleadoViewModel emple = new EmpleadoViewModel();
            emple.emp = db.EMPLEADO.Where(x => x.dni == dni).SingleOrDefault();
            emple.usu = db.USUARIO.Where(x => x.dni == dni).SingleOrDefault();
            return View(emple);
        }
        public ActionResult Editar(string dni)
        {
            EmpleadoViewModel emple = new EmpleadoViewModel();
            emple.emp = db.EMPLEADO.Where(x => x.dni == dni).SingleOrDefault();
            emple.usu = db.USUARIO.Where(x => x.dni == dni).SingleOrDefault();
            ViewBag.EstadoCivil = new SelectList(new Variable().getEstadoCivil(), "codigo", "codigo");
            ViewBag.Sexo = new SelectList(new Variable().getSexo(), "codigo", "nombre");
            ViewBag.Cargo = new SelectList(db.CARGO.ToList().OrderBy(x => x.descripcion), "codigocargo", "descripcion");
            return View(emple);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(EmpleadoViewModel item)
        {
            try
            {
                var _emp = db.EMPLEADO.Where(x => x.dni == item.emp.dni).SingleOrDefault();
                var _usu = db.USUARIO.Where(x => x.dni == item.emp.dni).SingleOrDefault();
                _emp.nombres = item.emp.nombres;
                _emp.apellidos = item.emp.apellidos;
                _emp.fechanacimiento = item.emp.fechanacimiento;
                _emp.sexo = item.emp.sexo;
                _emp.estadocivil = item.emp.estadocivil;
                _emp.fechaingreso = item.emp.fechaingreso;
                _emp.codigocargo = item.emp.codigocargo;
                _emp.email = item.emp.email;
                _emp.CMP2 = item.emp.CMP2;
                _emp.pathfoto = item.emp.pathfoto;
                _emp.email_personal = item.emp.email_personal;
                _emp.RNE = item.emp.RNE;
                _emp.dni_corregido = item.emp.dni_corregido;
                _emp.isActivo = item.emp.isActivo;
                _usu.ShortName = item.usu.ShortName;

                db.SaveChanges();
                return RedirectToAction("List");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.EstadoCivil = new SelectList(new Variable().getEstadoCivil(), "codigo", "codigo");
                ViewBag.Sexo = new SelectList(new Variable().getSexo(), "codigo", "nombre");
                ViewBag.Cargo = new SelectList(db.CARGO.ToList().OrderBy(x => x.descripcion), "codigocargo", "descripcion");
                return View();
            }
        }
        public ActionResult validarSiglas(string siglas)
        {
            var d = db.USUARIO.Where(x => x.siglas == siglas).ToList();
            return Json(d.Count <= 0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult validarClave(string clave)
        {
            return Json(db.USUARIO.Where(x => x.codigousuario == clave).ToList().Count <= 0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult validarDNI(string dni)
        {
            return Json(db.USUARIO.Where(x => x.dni == dni).ToList().Count <= 0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sucursalesusuario(string id, int tipo)
        {
            ViewBag.Sucursal = new SelectList(db.SUCURSAL.Where(x => x.codigounidad == 1 || x.codigounidad == 2).Select(x => new { codigosucursal = x.codigounidad * 100 + x.codigosucursal, descripcion = (x.codigounidad == 1 ? "Resocentro - " : "Emetac - ") + x.descripcion }).OrderBy(x=> x.codigosucursal).ToList(), "codigosucursal", "descripcion");

            USUARIO usuario;
            if (tipo == 1)
                usuario = db.USUARIO.Where(x => x.dni == id).SingleOrDefault();
            else
                usuario = db.USUARIO.Where(x => x.codigousuario == id).SingleOrDefault();
            if (usuario != null)
            {
                ViewBag.Usuario = usuario.codigousuario;
                var lista = db.SUCURSALXUSUARIO.Where(x => x.codigousuario == usuario.codigousuario ).ToList();
                return View(lista);
            }
            return RedirectToAction("List");
        }

        public ActionResult AgregarSucursalusuario(string id, int unidad,int sucursal)
        {
            var _obj = db.SUCURSALXUSUARIO.Where(x => x.codigousuario == id && x.codigounidad == 1 && x.codigosucursal == sucursal).ToList();
            if (_obj.Count == 0)
            {
                SUCURSALXUSUARIO item = new SUCURSALXUSUARIO();
                item.codigousuario = id;
                item.codigounidad = unidad;
                item.codigosucursal = sucursal;
                item.estado = "1";
                db.SUCURSALXUSUARIO.Add(item);
                db.SaveChanges();
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EliminarSucursalusuario(string id,int unidad,int sucursal)
        {
            var _obj = db.SUCURSALXUSUARIO.SingleOrDefault(x => x.codigousuario == id && x.codigounidad == unidad && x.codigosucursal == sucursal);
            if (_obj != null)
            {
                db.SUCURSALXUSUARIO.Remove(_obj);
                db.SaveChanges();
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadImg(IEnumerable<HttpPostedFileBase> files)
        {
            Variable u = new Variable();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var name = Guid.NewGuid().ToString();
            string _ruta = u.getUrlpathImgColaborador + name + ".png";
            var r = new List<UploadFilesResult>();
            foreach (string file in Request.Files)
            {

                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;
                hpf.SaveAs(_ruta);
                r.Add(new UploadFilesResult()
                {
                    Name = name + ".png",
                    Length = hpf.ContentLength,
                    Type = hpf.ContentType
                });
            }
            // Returns json
            return Content("{\"name\":\"" + r[0].Name + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");
        }

        public ActionResult deleteImgBuddy(string file)
        {
            Variable u = new Variable();
            System.IO.File.Delete(u.getUrlpathImgColaborador + file);
            return Json(true);
        }


        public ActionResult ListBoletaPago()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            List<DownloadableFile> vm = new List<DownloadableFile>();
            try
            {
                string filePath = @"\\serverweb\Boleta de Pagos\" + user.dni;

                //map the virtual path to a "local" path since GetFiles() can't use URI paths
                DirectoryInfo dir = new DirectoryInfo(filePath);

                //Get all files (but not any subdirectories) in the folder specified above
                FileInfo[] files = dir.GetFiles();

                //iterate through each file, get its name and set its path, and add to my VM
                foreach (FileInfo file in files)
                {
                    DownloadableFile newFile = new DownloadableFile();
                    newFile.FileName = Path.GetFileNameWithoutExtension(file.FullName);     //remove the file extension for the name
                    newFile.Path = "http://extranet.resocentro.com:5050/BOLETASPAGOS/" + user.dni + "/" + file.Name;  
                      //set path to virtual directory + file name
                    newFile.pathdelete = user.dni + "/" + file.Name;  
                    vm.Add(newFile);                                       //add each file to the right list in the Viewmodel
                }
            }
            catch (Exception ex)
            {
                ex = ex;
            }
            //identify the virtual path
           

            return View(vm);
        }
        public ActionResult ConfirmacionLectura(string filename,string path)
        {
             CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var usuario= user.ProviderUserKey.ToString();
            var confirmacion=db.ConfirmacionLecturaPago.SingleOrDefault(x=> x.codigousuario==usuario && x.archivo==filename);
            if (confirmacion == null)
            {

                ConfirmacionLecturaPago item = new ConfirmacionLecturaPago();
                item.archivo = filename;
                item.fecha = Variable.getDatetime();
                item.usuario = user.UserName;
                item.codigousuario = user.ProviderUserKey.ToString();
                db.ConfirmacionLecturaPago.Add(item);
            db.SaveChanges();
            }
           
           
            return Json(true);
        }


        
    }

    public class DisplayViewModel
    {
        public List<DownloadableFile> FileList { get; set; }
    }

    public class DownloadableFile
    {
        public string FileName { get; set; }
        public string Path { get; set; }

        public string pathdelete { get; set; }
    }
}