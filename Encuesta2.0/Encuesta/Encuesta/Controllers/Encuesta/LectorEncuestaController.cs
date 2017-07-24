using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Encuesta;
using Encuesta.ViewModels.Encuesta.Tipos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Encuesta
{
    [Authorize(Roles = "18")]
    public class LectorEncuestaController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        //
        // GET: /LectorEncuesta/
        public ActionResult LectorEncuesta(int examen, string lector, bool? isVisible)
        {
            EXAMENXATENCION _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            #region Verificacion de ampliaciones
            if (_exam.codigoestudio.Substring(5, 1) == "1")//si es ampliacion cambiamos el numero de examen por el de origen si tiene
            {
                int? amplicodigo = _exam.CodigoDeEstudioOrigen;
                if (amplicodigo != null)
                {
                    //asignamos el codigo del examen de origen y cargamos los datos del nuevo examen
                    examen = amplicodigo.Value;
                    _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
                }
            }

            #endregion
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                if (isVisible == null)
                    isVisible = true;
                try
                {


                    #region Redireccionar Encuesta
                    EncuestaType tipo_encu = (EncuestaType)_encu.tipo_encu;
                    switch (tipo_encu)
                    {
                        case EncuestaType.Cerebro:
                            return RedirectToAction("LectorCerebro", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.HombroIzquierdo:
                        case EncuestaType.HombroDerecho:
                            return RedirectToAction("LectorHombro", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.RodillaIzquierda:
                        case EncuestaType.RodillaDerecha:
                            return RedirectToAction("LectorRodilla", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.ColumnaCervical:
                        case EncuestaType.ColumnaCervicoDorsal:
                            return RedirectToAction("LectorColumnaA", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.ColumnaDorsal:
                        case EncuestaType.ColumnaDorsoLumbar:
                        case EncuestaType.ColumnaLumboSacra:
                            return RedirectToAction("LectorColumnaB", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.ColumnaSacroCoxis:
                            return RedirectToAction("LectorColumnac", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.Caderaderecha:
                        case EncuestaType.Caderaizquierda:
                            return RedirectToAction("LectorCadera", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.Cododerecha:
                        case EncuestaType.Codoizquierda:
                            return RedirectToAction("LectorCodo", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.TobilloIzquierdo:
                        case EncuestaType.PieIzquierdo:
                        case EncuestaType.DedosIzquierdo:
                        case EncuestaType.TobilloDerecho:
                        case EncuestaType.PieDerecho:
                        case EncuestaType.DedosDerecho:
                            return RedirectToAction("LectorPie", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

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
                            return RedirectToAction("LectorMano", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });


                        case EncuestaType.Abdomen:
                            return RedirectToAction("LectorAbdomen", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.BrazoDerecho:
                        case EncuestaType.AntebrazoDerecho:
                        case EncuestaType.MusloDerecho:
                        case EncuestaType.PiernaDerecho:
                        case EncuestaType.BrazoIzquierdo:
                        case EncuestaType.AntebrazoIzquierdo:
                        case EncuestaType.MusloIzquierdo:
                        case EncuestaType.PiernaIzquierdo:
                            return RedirectToAction("LectorExtremidad", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.MamaDerecha:
                        case EncuestaType.MamaIzquierda:
                            return RedirectToAction("LectorMama", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.Oncologico:
                            return RedirectToAction("LectorOncologio", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.Colonoscopia:
                            return RedirectToAction("LectorColonoscopia", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.AngiotemCoronario:
                            return RedirectToAction("LectorAngioCoronario", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.AngiotemAorta:
                            return RedirectToAction("LectorAngioAorta", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.AngiotemCerebral:
                            return RedirectToAction("LectorTEMCerebro", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });
                        //return RedirectToAction("EncuestaAngioCerebral", new { examen = _exam.codigo, tipo = _encu.tipo_encu,lector=lector });

                        case EncuestaType.MusculoEsqueletico:
                            return RedirectToAction("LectorMusculoEsqueletico", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.NeuroCerebro:
                            return RedirectToAction("LectorNeuroCerebro", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.Generica:
                            return RedirectToAction("LectorGenericaTEM", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.EncuestaGenericaEME:
                            return RedirectToAction("LectorGenericaTEMEMETAC", "LectorEncuesta", new { Area = "Medica", examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });

                        case EncuestaType.EncuestaVacia:
                        default:
                            return RedirectToAction("EncuestaVacia", new { examen = _exam.codigo, tipo = _encu.tipo_encu, lector = lector, isVisible = isVisible });
                    }

                    #endregion
                }
                catch (Exception)
                {

                    var herramienta = new Herramientas.HerramientasController();
                    herramienta.SolucionarProblemasEncuestaInternal(examen);
                    return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen });
                }
            }
            else
            {
                return RedirectToAction("ListaEspera", "RealizarEncuesta");
            }
        }

        public EncuestaDetalleViewModel getEncuestaDetalle(Models.Encuesta enc)
        {
            try
            {
                var encuestador = db.USUARIO.Where(x => x.codigousuario == enc.usu_reg_encu).SingleOrDefault();
                var tecnoologo = db.USUARIO.Where(x => x.codigousuario == enc.usu_reg_tecno).SingleOrDefault();
                var supervisor = db.USUARIO.Where(x => x.codigousuario == enc.usu_reg_super).SingleOrDefault();
                var _exam = db.EXAMENXATENCION.Where(x => x.codigo == enc.numeroexamen).SingleOrDefault();
                var _honorario = db.HONORARIOTECNOLOGO.Where(x => x.codigohonorariotecnologo == _exam.codigo).SingleOrDefault();
                var equipo = db.EQUIPO.Where(x => x.codigoequipo == _exam.equipoAsignado).SingleOrDefault();
                
                EncuestaDetalleViewModel item = new EncuestaDetalleViewModel();
                item.encuestador = encuestador == null ? "" : encuestador.ShortName;
                item.tecnologo = tecnoologo == null ? "" : tecnoologo.ShortName;
                item.supervisor = supervisor == null ? "" : supervisor.ShortName;
                item.equipo = equipo == null ? "" : equipo.ShortDesc;
                item.nexamen = _exam.codigo;
                item.calificacion = db.SupervisarEncuesta.Where(x => x.numeroexamen == _exam.codigo).SingleOrDefault();
                item.isampliacion = _exam.AmpliCodigo != null;
                if (_honorario != null)
                    item.comentariosTecnologo = _honorario.comentarios;
                if (item.isampliacion)
                {
                    var itemx = db.ExamenAmpliacion.Where(x => x.codigo == _exam.codigo && x.EstadoAmpliacion == "R").Take(1).SingleOrDefault();
                    var _encuAmpliacion = db.Encuesta.Where(x => x.numeroexamen == itemx.AmpliCodigo).SingleOrDefault();
                    if (itemx != null && _encuAmpliacion!=null)
                    {
                        item.AmpliMotivo = itemx.AmpliMotivo;                        
                        var encuestadorAmpli = db.USUARIO.Where(x => x.codigousuario == _encuAmpliacion.usu_reg_encu).SingleOrDefault();
                        var tecnoAmpli = db.USUARIO.Where(x => x.codigousuario == _encuAmpliacion.usu_reg_tecno).SingleOrDefault();
                        var superAmpli = db.USUARIO.Where(x => x.codigousuario == _encuAmpliacion.usu_reg_super).SingleOrDefault();
                        var _examAmpli = db.EXAMENXATENCION.Where(x => x.codigo == _encuAmpliacion.numeroexamen).SingleOrDefault();
                        var equipoAmpli = db.EQUIPO.Where(x => x.codigoequipo == _examAmpli.equipoAsignado).SingleOrDefault();
                        var _honorarioAmpli = db.HONORARIOTECNOLOGO.Where(x => x.codigohonorariotecnologo == _examAmpli.codigo).SingleOrDefault();
                        item.nexamenAmpli = _examAmpli.codigo;
                        item.calificacion_Ampli = db.SupervisarEncuesta.Where(x => x.numeroexamen == _examAmpli.codigo).SingleOrDefault();
                        item.encuestadorAmpli = encuestadorAmpli == null ? "" : encuestadorAmpli.ShortName;
                        item.tecnoAmpli = tecnoAmpli == null ? "" : tecnoAmpli.ShortName;
                        item.superAmpli = superAmpli == null ? "" : superAmpli.ShortName;
                        item.equipoAmpli = equipoAmpli == null ? "" : equipoAmpli.ShortDesc;
                        if (_honorarioAmpli != null)
                            item.comentariosTecnologoAmpli = _honorarioAmpli.comentarios;
                    }

                }
                item.tecnicas = ListarTecnicas(_exam.numerocita,_exam.codigoestudio.Substring(3, 2));
                item.contraste = db.EXAMENXCITA.Where(x => x.numerocita == _exam.numerocita &&
                    x.codigoestudio.Substring(6, 1) == "9" &&
                    x.codigoestudio.Substring(7, 2) != "99" &&
                    x.ESTUDIO.nombreestudio.Contains("contraste")
                    ).Select(x=> x.codigoexamencita).ToList().Count() > 0;
                return item;
            }
            catch (Exception ex)
            {
                ex = ex;
                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(enc.numeroexamen);
                return getEncuestaDetalle(enc);
            }



        }

        #region RESOCENTRO

        /******************************************************************************************
       *      RM
      *******************************************************************************************/

        #region CEREBRO
        public ActionResult LectorCerebro(int examen, int tipo, bool isVisible,string lector="")
        {

            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaCerebroViewModel item = new EncuestaCerebroViewModel();
            //EncuestaExamenViewModel encuesta = new EncuestaExamenViewModel();
            //Cargar datos de la pre-encuesta
            #region Pre-Entevista
            /*
            var med_pre = db.MED_PRE_ENTREVISTA.Where(x => x.numeroatencion == _exam.numeroatencion).SingleOrDefault();
            encuesta.usu_reg = med_pre.usu_reg;
            encuesta.numeroatencion = med_pre.numeroatencion;
            encuesta.p1 = med_pre.CheckBox1;
            encuesta.p2 = med_pre.CheckBox2;
            encuesta.CheckBox3 = med_pre.CheckBox3;
            encuesta.CheckBox4 = med_pre.CheckBox4;
            encuesta.CheckBox5 = med_pre.CheckBox5;
            encuesta.CheckBox6 = med_pre.CheckBox6;
            encuesta.CheckBox7 = med_pre.CheckBox7;
            encuesta.CheckBox8 = med_pre.CheckBox8;
            encuesta.CheckBox9 = med_pre.CheckBox9;
            encuesta.CheckBox10 = med_pre.CheckBox10;
            encuesta.CheckBox11 = med_pre.CheckBox11;
            encuesta.CheckBox12 = med_pre.CheckBox12;
            encuesta.CheckBox13 = med_pre.CheckBox13;
            encuesta.CheckBox14 = med_pre.CheckBox14;
            encuesta.CheckBox15 = med_pre.CheckBox15;
            encuesta.CheckBox16 = med_pre.CheckBox16;
            encuesta.CheckBox17 = med_pre.CheckBox17;
            encuesta.CheckBox18 = med_pre.CheckBox18;
            encuesta.CheckBox19 = med_pre.CheckBox19;
            encuesta.CheckBox20 = med_pre.CheckBox20;
            encuesta.CheckBox21 = med_pre.CheckBox21;
            encuesta.txtOtros = med_pre.txtOtros;
            encuesta.CheckBox30 = med_pre.CheckBox30;
            encuesta.CheckBox31 = med_pre.CheckBox31;
            encuesta.txtNumeroConsentimiento = med_pre.txtNumeroConsentimiento;
            encuesta.isAutorizado = med_pre.isAutorizado;
            if (med_pre.tipoAutorizacion != null)
                encuesta.tipoAutorizacion = med_pre.tipoAutorizacion.Value;
            encuesta.detalleAutorizacion = med_pre.detalleAutorizacion;
            encuesta.fec_reg_ini = med_pre.fec_reg_ini;
            encuesta.fec_reg_fin = med_pre.fec_reg_fin;
            */
            #endregion
            //cargar datos principales de la encuesta 
            #region Encuesta
            /*encuesta.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
            encuesta.numeroatencion = _exam.numeroatencion;
            encuesta.fec_reg_ini = DateTime.Now;
            encuesta.CheckBox31 = _exam.ATENCION.CITA.sedacion;
            encuesta.encuesta = new Models.Encuesta();
            encuesta.num_examen = _exam.codigo;
            encuesta.sexo = _paci.sexo;
            encuesta._examen = _exam;
            encuesta._paciente = _paci;*/
            #endregion
            //cargar datos de la encuesta cerebro
            var _cerebro = db.HISTORIA_CLINICA_CEREBRO.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_cerebro != null)
            {
                item = GetEntity(_cerebro);
                item._examen = _exam;
                item._paciente = _paci;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;

                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }

        public EncuestaCerebroViewModel GetEntity(HISTORIA_CLINICA_CEREBRO item)
        {
            var entidad = new EncuestaCerebroViewModel();
            entidad.numeroexamen = item.numeroexamen;

            entidad.p1 = item.p1;
            if (entidad.p1.Value)
            {
                entidad.p1_1 = item.p1a[0].ToString() == "1";
                entidad.p1_2 = item.p1a[1].ToString() == "1";
                entidad.p1_3 = item.p1a[2].ToString() == "1";
                if (entidad.p1_3)
                    entidad.p1_4 = item.p1a31;
            }
            entidad.p2 = item.p2;
            if (entidad.p2.Value)
            {
                entidad.p2_1 = item.p2a.Value;
            }
            entidad.p3 = item.p3;
            if (entidad.p3.Value)
            {
                entidad.p3_1 = item.p3a.Value;
                if (entidad.p3_1.Value)
                {
                    //1
                    entidad.p3_1_1 = item.p3a1.Value;
                    //2
                    entidad.p3_1_2 = item.p3a2.Value;
                    //3
                    entidad.p3_1_3 = item.p3a3[0].ToString() == "1";
                    entidad.p3_1_4 = item.p3a3[1].ToString() == "1";
                    entidad.p3_1_5 = item.p3a3[2].ToString() == "1";
                }
            }
            entidad.p4 = item.p4;
            if (entidad.p4.Value)
            {
                entidad.p4_1 = item.p4a;
            }
            entidad.p5 = item.p5;
            if (entidad.p5.Value)
            {
                entidad.p5_1 = item.p5a[0].ToString() == "1";
                entidad.p5_2 = item.p5a[1].ToString() == "1";
                entidad.p5_3 = item.p5a[2].ToString() == "1";
                entidad.p5_4 = item.p5a[3].ToString() == "1";
                entidad.p5_4_1 = item.p5a41;
            }
            entidad.p6 = item.p6;
            if (entidad.p6.Value)
            {
                entidad.p6_1 = item.p6a[0].ToString() == "1";
                entidad.p6_2 = item.p6a[2].ToString() == "1";
                entidad.p6_3 = item.p6a[4].ToString() == "1";

                entidad.p6_4 = item.p6a[1].ToString() == "1";
                entidad.p6_5 = item.p6a[3].ToString() == "1";
                entidad.p6_6 = item.p6a[5].ToString() == "1";

                entidad.p6_7 = item.p6a[6].ToString() == "1";
                entidad.p6_7_1 = item.p6a71;
            }
            entidad.p7 = item.p7;
            string[] s = item.p8.Split('%');
            entidad.p8 = int.Parse(s[0]);
            entidad.p8_1 = int.Parse(s[1]);
            entidad.p9 = item.p9;
            entidad.p9_1 = item.p9a1;
            entidad.p10 = item.p10;
            if (entidad.p10.Value)
            {
                entidad.p10_1 = item.p10a71;
            }
            entidad.p11 = item.p11;
            if (entidad.p11.Value)
            {
                entidad.p11_1 = item.p11a;
                string[] s11 = item.p11b.Split('%');
                entidad.p11_2 = int.Parse(s[0]);
                entidad.p11_3 = int.Parse(s[1]);
                entidad.p11_4 = item.p11c;
            }
            entidad.p12 = item.p12;
            if (entidad.p12.Value)
            {
                //Resonancia
                entidad.p12_1 = item.p12aa.Value;
                if (entidad.p12_1)
                {
                    entidad.p12_1_1 = int.Parse(item.p12a);
                    if (entidad.p12_1_1 == 9)
                        entidad.p12_1_2 = int.Parse(item.p12a41);
                    string[] s12b = item.p12b.Split('%');
                    entidad.p12_1_3 = int.Parse(s12b[0]);
                    entidad.p12_1_4 = int.Parse(s12b[1]);
                    entidad.p12_1_5 = item.p12c;
                }
                entidad.p12_2 = item.p12bb.Value;
                if (entidad.p12_2)
                {
                    entidad.p12_2_1 = int.Parse(item.p12d);
                    if (entidad.p12_2_1 == 9)
                        entidad.p12_2_2 = int.Parse(item.p12d41);
                    string[] s12b = item.p12e.Split('%');
                    entidad.p12_2_3 = int.Parse(s12b[0]);
                    entidad.p12_2_4 = int.Parse(s12b[1]);
                    entidad.p12_2_5 = item.p12f;
                }
            }
            entidad.p13A_1 = item.p13Acheck;
            if (entidad.p13A_1 == false)
                entidad.p13A = item.p13A;
            entidad.p13B_1 = item.p13Bcheck;
            if (entidad.p13B_1 == false)
                entidad.p13B = item.p13B;

            entidad.p14 = item.p14;

            entidad.p15_1 = item.p15[0].ToString() == "1";
            entidad.p15_2 = item.p15[1].ToString() == "1";
            entidad.p15_3 = item.p15[2].ToString() == "1";
            entidad.p15_4 = item.p15[3].ToString() == "1";
            entidad.p15_5 = item.p15[4].ToString() == "1";
            entidad.p15_6 = item.p15[5].ToString() == "1";
            entidad.p15_7 = item.p15[6].ToString() == "1";
            entidad.p15_8 = item.p15[7].ToString() == "1";
            entidad.p15_9 = item.p15[8].ToString() == "1";
            entidad.p15_10 = item.p15[9].ToString() == "1";
            entidad.p15_11 = item.p15[10].ToString() == "1";
            entidad.p15_12 = item.p15[11].ToString() == "1";
            entidad.p15_13 = item.p15[12].ToString() == "1";
            entidad.p15_12_1 = item.p15a121;
            entidad.p15_13_1 = item.p15a131;

            entidad.p16_1 = item.p16[0].ToString() == "1";
            entidad.p16_2 = item.p16[1].ToString() == "1";
            entidad.p16_3 = item.p16[2].ToString() == "1";
            entidad.p16_4 = item.p16[3].ToString() == "1";
            entidad.p16_5 = item.p16[4].ToString() == "1";
            entidad.p16_6 = item.p16[5].ToString() == "1";
            entidad.p16_7 = item.p16[6].ToString() == "1";
            entidad.p16_8 = item.p16[7].ToString() == "1";
            entidad.p16_9 = item.p16[8].ToString() == "1";
            entidad.p16_10 = item.p16[9].ToString() == "1";
            entidad.p16_10_1 = item.p16a101;
            entidad.p17 = int.Parse(item.p17);
            if (entidad.p17 == 1)
                entidad.p17_1 = item.p17_1.Value;

            return entidad;
        }
        #endregion

        #region HOMBRO
        public ActionResult LectorHombro(int examen, int tipo, bool isVisible,string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaHombroViewModel item = new EncuestaHombroViewModel();
            var _hombro = db.HISTORIA_CLINICA_HOMBRO.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_hombro != null)
            {
                #region Cargar Hombro

                item.p1 = _hombro.p1.Value;
                if (item.p1.Value)
                {
                    item.p1_1 = _hombro.p1_1.Value;
                    item.p1_2 = _hombro.p1_2.Value;
                    item.p1_3 = _hombro.p1_3.Value;
                    item.p1_4 = _hombro.p1_4.Value;
                    item.p1_5 = _hombro.p1_5.Value;
                    item.p1_6 = _hombro.p1_6.Value;
                    item.p1_7 = _hombro.p1_7.Value;
                    item.p1_7_1 = _hombro.p1_7_1;
                }

                item.p2 = _hombro.p2;
                if (item.p2.Value)
                {
                    item.p2_1 = _hombro.p2_1;
                    item.p2_21 = _hombro.p2_21;
                    item.p2_22 = _hombro.p2_22;
                }
                item.p3 = _hombro.p3;
                item.p4_1 = _hombro.p4_1;
                item.p4_2 = _hombro.p4_2.Value;
                if (_hombro.p4_3 != null)
                {
                    item.p4_3 = _hombro.p4_3.Value;
                    item.p4_3_1 = _hombro.p4_3_1;
                }

                item.p5 = _hombro.p5.Value;
                if (item.p5.Value)
                {
                    item.p5_1 = _hombro.p5_1;
                }
                item.p6 = _hombro.p6.Value;
                if (item.p6.Value)
                {
                    item.p6_1 = _hombro.p6_1;
                }
                item.p7 = _hombro.p7;
                item.p8 = _hombro.p8.Value;
                if (item.p8.Value)
                {
                    item.p8_1 = _hombro.p8_1;
                    item.p8_21 = _hombro.p8_21;
                    item.p8_22 = _hombro.p8_22.Value;
                    item.p8_3 = _hombro.p8_3;
                    item.p8_4 = _hombro.p8_4;
                }
                item.p9 = _hombro.p9.Value;
                if (item.p9.Value)
                {
                    item.p9_1 = _hombro.p9_1;
                    item.p9_21 = _hombro.p9_21;
                    item.p9_22 = _hombro.p9_22.Value;
                    item.p9_3 = _hombro.p9_3;
                }

                item.p10 = _hombro.p10.Value;
                if (item.p10.Value)
                {
                    item.p10_1 = _hombro.p10_1.Value;
                    if (item.p10_1 == 9)
                    {
                        item.p10_11 = _hombro.p10_11;
                    }
                    item.p10_21 = _hombro.p10_21;
                    item.p10_22 = _hombro.p10_22.Value;
                    item.p10_3 = _hombro.p10_3;
                }
                item.p11 = _hombro.p11.Value;
                if (item.p11.Value)
                {
                    item.p11_1 = _hombro.p11_1.Value;
                    if (item.p11_1 == 6)
                    {
                        item.p11_11 = _hombro.p11_11;
                    }
                    item.p11_2 = _hombro.p11_2.Value;
                }

                item.p12Acheck = _hombro.p12Acheck;
                if (item.p12Acheck == false)
                    item.p12A = _hombro.p12A;
                item.p12Bcheck = _hombro.p12Bcheck;
                if (item.p12Bcheck == false)
                    item.p12B = _hombro.p12B;

                item.p13 = _hombro.p13;
                item.p14 = _hombro.p14;
                if (item.p14 == 2)
                {
                    item.p14_1 = _hombro.p14_1.Value;
                }
                item.p15 = _hombro.p15;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region EXTREMIDAD
        public ActionResult LectorExtremidad(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaExtremidadesViewModel item = new EncuestaExtremidadesViewModel();
            var _itemdb = db.HISTORIA_CLINICA_EXTREMIDAD.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Extremidad

                item.numeroexamen = _itemdb.numeroexamen;
                item.p1 = _itemdb.p1.Value;
                if (item.p1.Value)
                {
                    item.p1_0 = _itemdb.p1_0.Value;
                    item.p1_2 = _itemdb.p1_2.Value;
                    item.p1_3 = _itemdb.p1_3.Value;
                    item.p1_4 = _itemdb.p1_4.Value;
                    item.p1_5 = _itemdb.p1_5.Value;
                    item.p1_1 = _itemdb.p1_1;
                }

                item.p2 = _itemdb.p2.Value;
                if (item.p2.Value)
                {
                    item.p2_1 = _itemdb.p2_1.Value;
                    item.p2_2 = _itemdb.p2_2.Value;
                    item.p2_3 = _itemdb.p2_3.Value;
                    item.p2_4 = _itemdb.p2_4.Value;
                    item.p2_5 = _itemdb.p2_5.Value;
                    item.p2_5_1 = _itemdb.p2_5_1;
                }

                item.p3 = _itemdb.p3.Value;
                if (item.p3.Value)
                {
                    item.p3_1 = _itemdb.p3_1;
                }

                item.p4_1 = _itemdb.p4_1;
                item.p4_2 = _itemdb.p4_2.Value;
                if (item.p4_1 == "")
                    item.p4_3 = _itemdb.p4_3.Value;
                item.p4_3_1 = _itemdb.p4_3_1;

                item.p5 = _itemdb.p5;
                if (item.p5.Value)
                {
                    item.p5_1 = _itemdb.p5_1;
                }

                item.p7 = _itemdb.p7;
                item.p9 = _itemdb.p9;
                //item.p9 = txt_p9_a_1.Text;

                item.p10 = _itemdb.p10;
                if (item.p10.Value)
                {
                    item.p10_1 = _itemdb.p10_1;
                    item.p10_21 = _itemdb.p10_21;
                    item.p10_22 = _itemdb.p10_22.Value;
                    item.p10_3 = _itemdb.p10_3;
                    item.p10_4 = _itemdb.p10_4;
                }
                item.p11 = _itemdb.p11;
                if (item.p11.Value)
                {
                    item.p11_1 = _itemdb.p11_1.Value;
                    if (item.p11_1 == 6)
                    {
                        item.p11_11 = _itemdb.p11_11;
                    }
                    item.p11_21 = _itemdb.p11_21;
                    item.p11_22 = _itemdb.p11_22.Value;
                    item.p11_3 = _itemdb.p11_3;
                }
                item.p12 = _itemdb.p12;
                if (item.p12.Value)
                {
                    item.p12_1 = _itemdb.p12_1.Value;
                    if (item.p12_1 == 6)
                    {
                        item.p12_2 = _itemdb.p12_2;
                    }
                    item.p12_3 = _itemdb.p12_3.Value;
                }

                item.p13A_2 = _itemdb.p13A_2;
                if (item.p13A_2 == false)
                    item.p13A_1 = _itemdb.p13A_1;

                item.p13B_2 = _itemdb.p13B_2;
                if (item.p13B_2 == false)
                    item.p13B_1 = _itemdb.p13B_1;

                item.p14_1 = _itemdb.p14_1;
                item.p15 = _itemdb.p15;
                if (item.p15 == "2")
                {
                    item.p15_1 = _itemdb.p15_1.Value;
                }
                item.p16 = _itemdb.p16;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region CODO
        public ActionResult LectorCodo(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaCodoViewModel item = new EncuestaCodoViewModel();
            var _itemdb = db.HISTORIA_CLINICA_CODO.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Codo
                item.p1 = _itemdb.p1;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2.Value;
                    item.p1_3 = _itemdb.p1_3.Value;
                    item.p1_4 = _itemdb.p1_4.Value;
                    item.p1_8 = _itemdb.p1_8.Value;
                    if (item.p1_8)
                    {
                        item.p1_8_1 = _itemdb.p1_8_1;
                    }
                }
                item.p1_5 = _itemdb.p1_5.Value;
                if (item.p1_5)
                {
                    item.p1_51 = _itemdb.p1_51.Value;
                }
                item.p1_6 = _itemdb.p1_6.Value;
                item.p1_7 = _itemdb.p1_7.Value;
                if (item.p1_7)
                {
                    item.p1_71 = _itemdb.p1_71;
                }

                item.p2_1 = _itemdb.p2_1 == null ? "" : _itemdb.p2_1;
                item.p2_2 = _itemdb.p2_2.Value;
                if (!(_itemdb.p2_3 == false && _itemdb.p2_3_1 == null))
                {
                    item.p2_3 = _itemdb.p2_3;
                    item.p2_3_1 = _itemdb.p2_3_1;
                }

                item.p3 = _itemdb.p3;
                if (item.p3 != "3")
                {
                    item.p3_1 = _itemdb.p3_1;
                }


                item.p4 = _itemdb.p4;
                if (item.p4 == "1")
                {
                    item.p4_1 = _itemdb.p4_1;
                }
                item.p5 = _itemdb.p5;
                item.p6 = _itemdb.p6;
                //item.p7 = txt_p7_a_1.Text.Trim();


                item.p8 = _itemdb.p8;
                if (item.p8.Value)
                {
                    item.p8_1 = _itemdb.p8_1;
                    item.p8_21 = _itemdb.p8_21;
                    item.p8_22 = _itemdb.p8_22.Value;
                    item.p8_3 = _itemdb.p8_3;
                    item.p8_4 = _itemdb.p8_4;
                }

                item.p9 = _itemdb.p9;
                if (item.p9.Value)
                {
                    item.p9_1 = _itemdb.p9_1;
                    item.p9_21 = _itemdb.p9_21.Value;
                    item.p9_22 = _itemdb.p9_22.Value;
                }

                item.p10 = _itemdb.p10;
                if (item.p10.Value)
                {
                    item.p10_1 = _itemdb.p10_1.Value;
                    if (item.p10_1 == 6)
                    {
                        item.p10_11 = _itemdb.p10_11;
                    }
                    item.p10_21 = _itemdb.p10_21;
                    item.p10_22 = _itemdb.p10_22.Value;
                    item.p10_3 = _itemdb.p10_3;
                }

                item.p11 = _itemdb.p11;
                if (item.p11.Value)
                {
                    item.p11_11 = _itemdb.p11_11.Value;
                    if (item.p11_11 == 6)
                    {
                        item.p11_12 = _itemdb.p11_12;
                    }
                    item.p11_2 = _itemdb.p11_2.Value;
                }

                item.p12A_2 = _itemdb.p12A_2;
                if (item.p12A_2 == false)
                    item.p12A_1 = _itemdb.p12A_1;

                item.p12B_2 = _itemdb.p12B_2;
                if (item.p12B_2 == false)
                    item.p12B_1 = _itemdb.p12B_1;

                item.p13 = _itemdb.p13;
                item.p14 = _itemdb.p14;
                if (item.p14 == "2")
                    item.p14_1 = _itemdb.p14_1.Value;

                item.p15 = _itemdb.p15;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region MAMA
        public ActionResult LectorMama(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaMamaViewModel item = new EncuestaMamaViewModel();
            var _itemdb = db.HISTORIA_CLINICA_MAMA.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Mama
                item.p1 = _itemdb.p1;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2.Value;
                    item.p1_3 = _itemdb.p1_3.Value;
                    item.p1_4 = _itemdb.p1_4.Value;
                    item.p1_5 = _itemdb.p1_5.Value;
                    item.p1_6 = _itemdb.p1_6.Value;
                    //izquierda
                    item.p1_1_1 = _itemdb.p1_1_1.Value;
                    item.p1_2_2 = _itemdb.p1_2_2.Value;
                    item.p1_3_1 = _itemdb.p1_3_1.Value;
                    item.p1_4_1 = _itemdb.p1_4_1.Value;
                    item.p1_5_1 = _itemdb.p1_5_1.Value;
                    item.p1_6_1 = _itemdb.p1_6_1.Value;

                    item.p1_0 = _itemdb.p1_0;
                }
                //2
                item.p2 = _itemdb.p2.Value;
                item.p2_1 = _itemdb.p2_1.Value;
                // 3
                item.p3 = _itemdb.p3.Value;
                if (item.p3.Value)
                    item.p3_1 = _itemdb.p3_1;

                // 4
                item.p4 = _itemdb.p4.Value;
                if (item.p4.Value)
                {

                    item.p4_1 = _itemdb.p4_1;
                    // B 
                    item.p4_2 = _itemdb.p4_2;
                    item.p4_2_1 = _itemdb.p4_2_1.Value;
                    // C
                    item.p4_3 = _itemdb.p4_3.Value;
                    // D
                    item.p4_4 = _itemdb.p4_4;
                }

                //5
                item.p5 = _itemdb.p5;

                //6
                item.p6 = _itemdb.p6;

                //7
                item.p7 = _itemdb.p7;

                //8
                item.p8 = _itemdb.p8;

                //9
                item.p9 = _itemdb.p9.Value;
                if (item.p9.Value)
                {
                    item.p9_1 = _itemdb.p9_1;
                    item.p9_2 = _itemdb.p9_2.Value;
                }
                //10

                item.ind_p1 = _itemdb.ind_p1;

                item.ind_p1_1 = _itemdb.ind_p1_1.Value;

                item.p10 = _itemdb.p10;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region ABDOMEN
        public ActionResult LectorAbdomen(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaAbdomenViewModel item = new EncuestaAbdomenViewModel();
            var _itemdb = db.HISTORIA_CLINICA_ABDOMEN.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Abdomen
                item.p1 = _itemdb.p1;
                if (item.p1.Value)
                {
                    /*Donde*/
                    item.p1_12 = _itemdb.p1_12.Value;
                    item.p1_13 = _itemdb.p1_13.Value;
                    item.p1_14 = _itemdb.p1_14.Value;
                    item.p1_15 = _itemdb.p1_15.Value;
                    item.p1_16 = _itemdb.p1_16.Value;
                    item.p1_16_1 = _itemdb.p1_16_1;

                    /*Caracteristica*/
                    /*Colicos*/
                    item.p1_2 = _itemdb.p1_2.Value;
                    item.p1_2_1 = _itemdb.p1_2_1;
                    /*Constante*/
                    item.p1_17 = _itemdb.p1_17.Value;
                    /*Irradiado*/
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_1_1 = _itemdb.p1_1_1;
                    /*Empeora con comidas*/
                    item.p1_3 = _itemdb.p1_3.Value;
                    item.p1_3_1 = _itemdb.p1_3_1;
                    /*Otros*/
                    item.p1_18 = _itemdb.p1_18.Value;
                    item.p1_18_1 = _itemdb.p1_18_1;
                }

                item.p1_4 = _itemdb.p1_4.Value;
                /*Baha de Peso*/
                item.p1_5 = _itemdb.p1_5.Value;
                item.p1_5_1 = _itemdb.p1_5_1;//cuanto?
                item.p1_5_2 = _itemdb.p1_5_2;//en cuanto?
                item.p1_6 = _itemdb.p1_6.Value;
                item.p1_7 = _itemdb.p1_7.Value;
                item.p1_8 = _itemdb.p1_8.Value;
                item.p1_9 = _itemdb.p1_9.Value;
                item.p1_10 = _itemdb.p1_10.Value;
                item.p1_11 = _itemdb.p1_11.Value;
                item.p1_11_1 = _itemdb.p1_11_1;

                item.p2_1 = _itemdb.p2_1 == null ? "" : _itemdb.p2_1;
                item.p2_2 = _itemdb.p2_2.Value;
                if (!(_itemdb.p2_3 == false && (_itemdb.p2_3_1 == null || _itemdb.p2_3_1 == "")))
                {
                    item.p2_3 = _itemdb.p2_3;
                    item.p2_3_1 = _itemdb.p2_3_1;
                }


                item.p3_1 = _itemdb.p3_1;
                item.p4 = _itemdb.p4;
                if (item.p4 == "1")
                {
                    item.p4_1 = _itemdb.p4_1;
                }
                item.p5 = _itemdb.p5;
                item.p6 = _itemdb.p6;
                item.p7 = _itemdb.p7;


                item.p8 = _itemdb.p8;
                if (item.p8.Value)
                {
                    item.p8_1 = _itemdb.p8_1;
                    item.p8_4 = _itemdb.p8_4;
                }

                item.p9 = _itemdb.p9;
                if (item.p9.Value)
                {
                    item.p9_1 = _itemdb.p9_1;
                    item.p9_21 = _itemdb.p9_21;
                }

                item.p10 = _itemdb.p10;
                if (item.p10.Value)
                {
                    item.p10_1 = _itemdb.p10_1.Value;
                    if (item.p10_1 == 6)
                    {
                        item.p10_11 = _itemdb.p10_11;
                    }
                    item.p10_21 = _itemdb.p10_21;
                    item.p10_22 = _itemdb.p10_22.Value;
                    item.p10_3 = _itemdb.p10_3;
                }

                item.p11A_2 = _itemdb.p11A_2;
                if (item.p11A_2 == false)
                    item.p11A_1 = _itemdb.p11A_1;

                item.p11B_2 = _itemdb.p11B_2;
                if (item.p11B_2 == false)
                    item.p11B_1 = _itemdb.p11B_1;

                item.p12 = _itemdb.p12;
                item.p13 = _itemdb.p13;
                if (item.p13 == "2")
                    item.p14_1 = _itemdb.p14_1.Value;

                item.p15 = _itemdb.p15;
                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region CADERA
        public ActionResult LectorCadera(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaCaderaViewModel item = new EncuestaCaderaViewModel();
            var _itemdb = db.HISTORIA_CLINICA_CADERA.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Cadera
                item.p1 = _itemdb.p1;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2.Value;
                    item.p1_3 = _itemdb.p1_3.Value;
                    item.p1_7 = _itemdb.p1_7.Value;
                    item.p1_71 = _itemdb.p1_71;

                }
                item.p1_4 = _itemdb.p1_4.Value;
                item.p1_5 = _itemdb.p1_5.Value;
                item.p1_6 = _itemdb.p1_6.Value;
                if (item.p1_6)
                {
                    item.p1_61 = _itemdb.p1_61;
                }
                item.p2_1 = _itemdb.p2_1 == null ? "" : _itemdb.p2_1;
                item.p2_2 = _itemdb.p2_2.Value;
                if (!(_itemdb.p2_3 == false && (_itemdb.p2_3_1 == null || _itemdb.p2_3_1 == "")))
                {
                    item.p2_3 = _itemdb.p2_3;
                    item.p2_3_1 = _itemdb.p2_3_1;
                }


                item.p3 = _itemdb.p3;
                if (item.p3 != "3")
                {
                    item.p3_1 = _itemdb.p3_1;
                }

                item.p4 = _itemdb.p4;
                if (item.p4 == "1")
                {
                    item.p4_1 = _itemdb.p4_1;
                }
                item.p5 = _itemdb.p5;
                item.p6 = _itemdb.p6;
                item.p7 = _itemdb.p7;


                item.p8 = _itemdb.p8;
                if (item.p8.Value)
                {
                    item.p8_1 = _itemdb.p8_1;
                    item.p8_21 = _itemdb.p8_21;
                    item.p8_22 = _itemdb.p8_22.Value;
                    item.p8_3 = _itemdb.p8_3;
                    item.p8_4 = _itemdb.p8_4;
                }

                item.p9 = _itemdb.p9;
                if (item.p9.Value)
                {
                    item.p9_1 = _itemdb.p9_1;
                    item.p9_21 = _itemdb.p9_21.Value;
                    item.p9_22 = _itemdb.p9_22.Value;
                }

                item.p10 = _itemdb.p10;
                if (item.p10.Value)
                {
                    item.p10_1 = _itemdb.p10_1.Value;
                    if (item.p10_1 == 6)
                    {
                        item.p10_11 = _itemdb.p10_11;
                    }
                    item.p10_21 = _itemdb.p10_21;
                    item.p10_22 = _itemdb.p10_22.Value;
                    item.p10_3 = _itemdb.p10_3;
                }

                item.p11 = _itemdb.p11;
                if (item.p11.Value)
                {
                    item.p11_11 = _itemdb.p11_11.Value;
                    if (item.p11_11 == 6)
                    {
                        item.p11_12 = _itemdb.p11_12;
                    }
                    item.p11_2 = _itemdb.p11_2.Value;
                }

                item.p12A_2 = _itemdb.p12A_2;
                if (item.p12A_2 == false)
                    item.p12A_1 = _itemdb.p12A_1;

                item.p12B_2 = _itemdb.p12B_2;
                if (item.p12B_2 == false)
                    item.p12B_1 = _itemdb.p12B_1;

                item.p13 = _itemdb.p13;
                item.p14 = _itemdb.p14;
                if (item.p14 == "2")
                    item.p14_1 = _itemdb.p14_1.Value;

                item.p15 = _itemdb.p15;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region MANO
        public ActionResult LectorMano(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaManoViewModel item = new EncuestaManoViewModel();
            var _itemdb = db.HISTORIA_CLINICA_MANO.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Mano
                item.p1 = _itemdb.p1;
                if (item.p1.Value)
                {
                    item.p1_3 = _itemdb.p1_3.Value;
                    item.p1_4 = _itemdb.p1_4.Value;
                    item.p1_5 = _itemdb.p1_5.Value;
                    item.p1_6 = _itemdb.p1_6.Value;
                    item.p1_7 = _itemdb.p1_7.Value;
                    item.p1_7_1 = _itemdb.p1_7_1;
                }
                /*limitacio funciopnal*/
                item.p1_1 = _itemdb.p1_1.Value;
                if (item.p1_1)
                {
                    item.p1_11 = _itemdb.p1_11.Value;
                    item.p1_12 = _itemdb.p1_12.Value;
                    item.p1_13 = _itemdb.p1_13.Value;
                    item.p1_14 = _itemdb.p1_14.Value;
                    item.p1_15 = _itemdb.p1_15.Value;
                    item.p1_16 = _itemdb.p1_16.Value;
                    item.p1_16_1 = _itemdb.p1_16_1;
                }
                /*otros*/
                item.p1_2 = _itemdb.p1_2.Value;
                if (item.p1_2)
                {
                    item.p1_21 = _itemdb.p1_21;
                }
                item.p2 = _itemdb.p2;
                item.p3 = _itemdb.p3;
                item.p3_1 = _itemdb.p3_1;
                item.p4_1 = _itemdb.p4_1 == null ? "" : _itemdb.p4_1;
                item.p4_2 = _itemdb.p4_2.Value;
                if (!(_itemdb.p4_3 == false && (_itemdb.p4_3_1 == null || _itemdb.p4_3_1 == "")))
                {
                    item.p4_3 = _itemdb.p4_3;
                    item.p4_3_1 = _itemdb.p4_3_1;
                }
                item.p5 = _itemdb.p5;
                if (item.p5 != "3")
                {
                    item.p5_1 = _itemdb.p5_1;
                }

                item.p6 = _itemdb.p6;
                if (item.p6 == "1")
                {
                    item.p6_1 = _itemdb.p6_1;
                }
                item.p7 = _itemdb.p7;


                item.p10 = _itemdb.p10;
                if (item.p10.Value)
                {
                    item.p10_1 = _itemdb.p10_1;
                    item.p10_21 = _itemdb.p10_21;
                    item.p10_22 = _itemdb.p10_22.Value;
                    item.p10_3 = _itemdb.p10_3;
                    item.p10_4 = _itemdb.p10_4;
                }

                item.p11 = _itemdb.p11;
                if (item.p11.Value)
                {
                    item.p11_1 = _itemdb.p11_1;
                    item.p11_21 = _itemdb.p11_21.Value;
                    item.p11_22 = _itemdb.p11_22.Value;
                }

                item.p12 = _itemdb.p12;
                if (item.p12.Value)
                {
                    item.p12_1 = _itemdb.p12_1.Value;
                    if (item.p12_1 == 6)
                    {
                        item.p12_11 = _itemdb.p12_11;
                    }
                    item.p12_21 = _itemdb.p12_21;
                    item.p12_22 = _itemdb.p12_22.Value;
                    item.p12_3 = _itemdb.p12_3;
                }

                item.p13 = _itemdb.p13;
                if (item.p13.Value)
                {
                    item.p13_11 = _itemdb.p13_11.Value;
                    if (item.p13_11 == 6)
                    {
                        item.p13_12 = _itemdb.p13_12;
                    }
                    item.p13_2 = _itemdb.p13_2.Value;
                }

                item.p14A_2 = _itemdb.p14A_2;
                if (item.p14A_2 == false)
                    item.p14A_1 = _itemdb.p14A_1;

                item.p14B_2 = _itemdb.p14B_2;
                if (item.p14B_2 == false)
                    item.p14B_1 = _itemdb.p14B_1;

                item.p15 = _itemdb.p15;
                item.p16 = _itemdb.p16;
                if (item.p16 == "2")
                    item.p16_1 = _itemdb.p16_1.Value;

                item.p17 = _itemdb.p17;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region COLUMNA
        #region COLUMNA A
        public ActionResult LectorColumnaA(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaColumnaAViewModel item = new EncuestaColumnaAViewModel();
            var _itemdb = db.HISTORIA_CLINICA_COLUMNA_A.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Columna A
                item.mol_p1 = _itemdb.mol_p1;
                if (item.mol_p1.Value)
                {
                    item.mol_p1_1 = _itemdb.mol_p1_1.Value;
                    item.mol_p1_2 = _itemdb.mol_p1_2.Value;
                    item.mol_p1_3 = _itemdb.mol_p1_3.Value;
                    item.mol_p1_4 = _itemdb.mol_p1_4.Value;
                    item.mol_p1_5 = _itemdb.mol_p1_5.Value;
                    item.mol_p1_6 = _itemdb.mol_p1_6.Value;
                    item.mol_p1_7 = _itemdb.mol_p1_7.Value;
                    item.mol_p1_8 = _itemdb.mol_p1_8.Value;
                    item.mol_p1_9 = _itemdb.mol_p1_9.Value;
                }
                // 2
                item.mol_p2 = _itemdb.mol_p2;
                if (item.mol_p2.Value)
                {
                    item.mol_p2_1 = _itemdb.mol_p2_1.Value;
                    item.mol_p2_2 = _itemdb.mol_p2_2.Value;
                    item.mol_p2_3 = _itemdb.mol_p2_3.Value;
                    item.mol_p2_4 = _itemdb.mol_p2_4.Value;
                    item.mol_p2_5 = _itemdb.mol_p2_5.Value;
                    if (item.mol_p2_5)
                        item.mol_p2_5_1 = _itemdb.mol_p2_5_1;
                    item.mol_p2_6 = _itemdb.mol_p2_6.Value;
                    if (item.mol_p2_6)
                        item.mol_p2_6_1 = _itemdb.mol_p2_6_1;
                }
                // 3
                item.mol_p3_1 = _itemdb.mol_p3_1;
                item.mol_p3_2 = _itemdb.mol_p3_2;
                item.mol_p3_3 = _itemdb.mol_p3_3;
                // 4
                item.mol_p4_1 = _itemdb.mol_p4_1;
                item.mol_p4_2 = _itemdb.mol_p4_2;
                // 5
                item.mol_p5 = _itemdb.mol_p5;
                item.mol_p5_1 = _itemdb.mol_p5_1;
                // 6
                item.mol_p6 = _itemdb.mol_p6;
                if (item.mol_p6)
                {
                    item.mol_p6_1 = _itemdb.mol_p6_1.Value;
                    item.mol_p6_2 = _itemdb.mol_p6_2.Value;
                    item.mol_p6_3 = _itemdb.mol_p6_3.Value;
                    item.mol_p6_4 = _itemdb.mol_p6_4.Value;
                }
                // 7
                item.mol_p7 = _itemdb.mol_p7;
                // 8
                item.mol_p8 = _itemdb.mol_p8;
                // PROCEDIMIENTOS RELACIONADOS 
                // 1
                item.pro_p1 = _itemdb.pro_p1;
                if (item.pro_p1.Value)
                {
                    // A
                    item.pro_p1_1 = _itemdb.pro_p1_1;
                    // B 
                    item.pro_p1_21 = _itemdb.pro_p1_21;
                    item.pro_p1_22 = _itemdb.pro_p1_22.Value;
                    // C
                    item.pro_p1_3 = _itemdb.pro_p1_3;

                    // D
                    item.pro_p1_4 = _itemdb.pro_p1_4;
                }
                // 2
                item.pro_p2 = _itemdb.pro_p2;
                if (item.pro_p2.Value)
                {
                    // a
                    item.pro_p2_1 = _itemdb.pro_p2_1.Value;
                    if (item.pro_p2_1 == 6)
                    {
                        item.pro_p2_11 = _itemdb.pro_p2_11;
                    }
                    // b
                    item.pro_p2_21 = _itemdb.pro_p2_21;
                    item.pro_p2_22 = _itemdb.pro_p2_22.Value;
                    // c
                    item.pro_p2_3 = _itemdb.pro_p2_3;
                    // d
                    item.pro_p2_4_1 = _itemdb.pro_p2_4_1.Value;
                    item.pro_p2_4_2 = _itemdb.pro_p2_4_2.Value;
                }
                // PREGUNTAS A RESOLVER
                // 1
                item.pre_p1Acheck = _itemdb.pre_p1Acheck;
                if (item.pre_p1Acheck == false)
                    item.pre_p1A = _itemdb.pre_p1A;
                // 2
                item.pre_p1Bcheck = _itemdb.pre_p1Bcheck;
                if (item.pre_p1Bcheck == false)
                    item.pre_p1B = _itemdb.pre_p1B;
                // 3
                item.pre_p2 = _itemdb.pre_p2;

                // INDICACIONES
                item.ind_p1 = _itemdb.ind_p1;
                if (item.ind_p1 == "2")
                    item.ind_p1_1 = _itemdb.ind_p1_1.Value;
                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion
        #region COLUMNA B
        public ActionResult LectorColumnaB(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaColumnaBViewModel item = new EncuestaColumnaBViewModel();
            var _itemdb = db.HISTORIA_CLINICA_COLUMNA_B.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Columna B
                item.mol_p1 = _itemdb.mol_p1;
                if (item.mol_p1.Value)
                {
                    item.mol_p1_1 = _itemdb.mol_p1_1.Value;
                    item.mol_p1_2 = _itemdb.mol_p1_2.Value;
                    item.mol_p1_3 = _itemdb.mol_p1_3.Value;
                    item.mol_p1_4 = _itemdb.mol_p1_4.Value;
                    item.mol_p1_5 = _itemdb.mol_p1_5.Value;
                    item.mol_p1_6 = _itemdb.mol_p1_6.Value;
                    item.mol_p1_7 = _itemdb.mol_p1_7.Value;
                    item.mol_p1_7_1 = _itemdb.mol_p1_7_1.Value;
                    item.mol_p1_8 = _itemdb.mol_p1_8.Value;
                    item.mol_p1_8_1 = _itemdb.mol_p1_8_1;
                }
                // 2
                item.mol_p2 = _itemdb.mol_p2;
                if (item.mol_p2.Value)
                {
                    item.mol_p2_1 = _itemdb.mol_p2_1.Value;
                    item.mol_p2_2 = _itemdb.mol_p2_2.Value;
                    item.mol_p2_3 = _itemdb.mol_p2_3.Value;
                    item.mol_p2_4 = _itemdb.mol_p2_4.Value;
                    item.mol_p2_5 = _itemdb.mol_p2_5.Value;
                    if (item.mol_p2_5)
                        item.mol_p2_5_1 = _itemdb.mol_p2_5_1;
                    item.mol_p2_6 = _itemdb.mol_p2_6.Value;
                    if (item.mol_p2_6)
                        item.mol_p2_6_1 = _itemdb.mol_p2_6_1;
                }
                // 3
                item.mol_p3_1 = _itemdb.mol_p3_1;
                item.mol_p3_2 = _itemdb.mol_p3_2;
                item.mol_p3_2_1 = _itemdb.mol_p3_2_1;
                // 4
                item.mol_p4_1 = _itemdb.mol_p4_1;
                item.mol_p4_2 = _itemdb.mol_p4_2;
                // 5
                item.mol_p5 = _itemdb.mol_p5;
                // 6
                item.mol_p6 = _itemdb.mol_p6;
                if (item.mol_p6)
                {
                    item.mol_p6_1 = _itemdb.mol_p6_1.Value;
                    item.mol_p6_2 = _itemdb.mol_p6_2.Value;
                    item.mol_p6_3 = _itemdb.mol_p6_3.Value;
                    item.mol_p6_4 = _itemdb.mol_p6_4.Value;
                }
                // 7
                item.mol_p7 = _itemdb.mol_p7;
                // 8
                item.mol_p8 = _itemdb.mol_p8;
                // PROCEDIMIENTOS RELACIONADOS 
                // 1
                item.pro_p1 = _itemdb.pro_p1;
                if (item.pro_p1.Value)
                {
                    // A
                    item.pro_p1_1 = _itemdb.pro_p1_1;
                    // B 
                    item.pro_p1_21 = _itemdb.pro_p1_21;
                    item.pro_p1_22 = _itemdb.pro_p1_22.Value;
                    // C
                    item.pro_p1_3 = _itemdb.pro_p1_3;

                    // D
                    item.pro_p1_4 = _itemdb.pro_p1_4;
                }
                // 2
                item.pro_p2 = _itemdb.pro_p2;
                if (item.pro_p2.Value)
                {
                    // a
                    item.pro_p2_1 = _itemdb.pro_p2_1.Value;
                    if (item.pro_p2_1 == 6)
                    {
                        item.pro_p2_11 = _itemdb.pro_p2_11;
                    }
                    // b
                    item.pro_p2_21 = _itemdb.pro_p2_21;
                    item.pro_p2_22 = _itemdb.pro_p2_22.Value;
                    // c
                    item.pro_p2_3 = _itemdb.pro_p2_3;
                    // d
                    item.pro_p2_4_1 = _itemdb.pro_p2_4_1.Value;
                    item.pro_p2_4_2 = _itemdb.pro_p2_4_2.Value;
                }
                // PREGUNTAS A RESOLVER
                // 1
                item.pre_p1Acheck = _itemdb.pre_p1Acheck;
                if (item.pre_p1Acheck == false)
                    item.pre_p1A = _itemdb.pre_p1A;
                // 2
                item.pre_p1Bcheck = _itemdb.pre_p1Bcheck;
                if (item.pre_p1Bcheck == false)
                    item.pre_p1B = _itemdb.pre_p1B;
                // 3
                item.pre_p2 = _itemdb.pre_p2;

                // INDICACIONES
                item.ind_p1 = _itemdb.ind_p1;
                if (item.ind_p1 == "2")
                    item.ind_p1_1 = _itemdb.ind_p1_1.Value;
                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion
        #region COLUMNA C
        public ActionResult LectorColumnaC(int examen, int tipo, bool isVisible,string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaColumnaCViewModel item = new EncuestaColumnaCViewModel();
            var _itemdb = db.HISTORIA_CLINICA_COLUMNA_C.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Columna C
                item.mol_p1 = _itemdb.mol_p1;
                if (item.mol_p1.Value)
                {
                    item.mol_p1_1 = _itemdb.mol_p1_1.Value;
                    item.mol_p1_2 = _itemdb.mol_p1_2.Value;
                    item.mol_p1_3 = _itemdb.mol_p1_3.Value;
                    if (item.mol_p1_3)
                        item.mol_p1_3_1 = _itemdb.mol_p1_3_1;
                }
                // 2
                item.mol_p2 = _itemdb.mol_p2;
                if (item.mol_p2.Value)
                {
                    item.mol_p2_1 = _itemdb.mol_p2_1.Value;
                    item.mol_p2_2 = _itemdb.mol_p2_2.Value;
                    if (item.mol_p2_2)
                        item.mol_p2_2_1 = _itemdb.mol_p2_2_1;
                    item.mol_p2_3 = _itemdb.mol_p2_3.Value;
                    if (item.mol_p2_3)
                        item.mol_p2_3_1 = _itemdb.mol_p2_3_1;
                }
                // 3
                item.mol_p3_1 = _itemdb.mol_p3_1;
                // 4
                item.mol_p4_1 = _itemdb.mol_p4_1;
                item.mol_p4_2 = _itemdb.mol_p4_2;
                // 5
                item.mol_p5 = _itemdb.mol_p5;
                // 6
                item.mol_p6 = _itemdb.mol_p6;
                if (item.mol_p6)
                {
                    item.mol_p6_1 = _itemdb.mol_p6_1.Value;
                    item.mol_p6_2 = _itemdb.mol_p6_2.Value;
                    item.mol_p6_3 = _itemdb.mol_p6_3.Value;
                    item.mol_p6_4 = _itemdb.mol_p6_4.Value;
                }
                // 7
                item.mol_p7 = _itemdb.mol_p7;
                // 8
                item.mol_p8 = _itemdb.mol_p8;
                // PROCEDIMIENTOS RELACIONADOS 
                // 1
                item.pro_p1 = _itemdb.pro_p1;
                if (item.pro_p1.Value)
                {
                    // A
                    item.pro_p1_1 = _itemdb.pro_p1_1;
                    // B 
                    item.pro_p1_21 = _itemdb.pro_p1_21;
                    item.pro_p1_22 = _itemdb.pro_p1_22.Value;
                    // C
                    item.pro_p1_3 = _itemdb.pro_p1_3;

                    // D
                    item.pro_p1_4 = _itemdb.pro_p1_4;
                }
                // 2
                item.pro_p2 = _itemdb.pro_p2;
                if (item.pro_p2.Value)
                {
                    // a
                    item.pro_p2_1 = _itemdb.pro_p2_1.Value;
                    if (item.pro_p2_1 == 6)
                    {
                        item.pro_p2_11 = _itemdb.pro_p2_11;
                    }
                    // b
                    item.pro_p2_21 = _itemdb.pro_p2_21;
                    item.pro_p2_22 = _itemdb.pro_p2_22.Value;
                    // c
                    item.pro_p2_3 = _itemdb.pro_p2_3;
                    // d
                    item.pro_p2_4_1 = _itemdb.pro_p2_4_1.Value;
                    item.pro_p2_4_2 = _itemdb.pro_p2_4_2.Value;
                }
                // PREGUNTAS A RESOLVER
                // 1
                item.pre_p1Acheck = _itemdb.pre_p1Acheck;
                if (item.pre_p1Acheck == false)
                    item.pre_p1A = _itemdb.pre_p1A;
                // 2
                item.pre_p1Bcheck = _itemdb.pre_p1Bcheck;
                if (item.pre_p1Bcheck == false)
                    item.pre_p1B = _itemdb.pre_p1B;
                // 3
                item.pre_p2 = _itemdb.pre_p2;

                // INDICACIONES
                item.ind_p1 = _itemdb.ind_p1;
                if (item.ind_p1 == "2")
                    item.ind_p1_1 = _itemdb.ind_p1_1.Value;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion
        #endregion

        #region RODILLA
        public ActionResult LectorRodilla(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaRodillaViewModel item = new EncuestaRodillaViewModel();
            var _itemdb = db.HISTORIA_CLINICA_RODILLA.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Rodilla
                item.p1 = _itemdb.p1;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2.Value;
                    item.p1_3 = _itemdb.p1_3.Value;
                    item.p1_4 = _itemdb.p1_4.Value;
                    item.p1_5 = _itemdb.p1_5.Value;
                    item.p1_6 = _itemdb.p1_6.Value;
                    item.p1_7 = _itemdb.p1_7.Value;
                    item.p1_8 = _itemdb.p1_8.Value;
                    if (item.p1_8)
                        item.p1_81 = _itemdb.p1_81;
                    item.p1_9 = _itemdb.p1_9.Value;
                }

                item.p2 = _itemdb.p2;
                item.p2_4 = _itemdb.p2_4.Value;
                item.p2_5 = _itemdb.p2_5.Value;
                item.p2_6 = _itemdb.p2_6.Value;
                item.p2_7 = _itemdb.p2_7.Value;
                item.p2_8 = _itemdb.p2_8.Value;
                if (item.p2_8)
                    item.p2_81 = _itemdb.p2_81;
                item.p2_9 = _itemdb.p2_9.Value;

                item.p3 = _itemdb.p3;
                item.p4 = _itemdb.p4;
                item.p5 = _itemdb.p5;
                item.p6 = _itemdb.p6;

                item.p7_1 = _itemdb.p7_1 == null ? "" : _itemdb.p7_1;
                item.p7_11 = _itemdb.p7_11.Value;
                if (_itemdb.p7_2 != null)
                    item.p7_2 = _itemdb.p7_2.Value;
                item.p7_22 = _itemdb.p7_22;


                item.p8 = _itemdb.p8;
                if (item.p8 != "3")
                    item.p8_1 = _itemdb.p8_1;


                item.p9 = _itemdb.p9;
                if (item.p9.Value)
                {
                    item.p9_1 = _itemdb.p9_1;
                    item.p9_21 = _itemdb.p9_21;
                    item.p9_22 = _itemdb.p9_22.Value;
                    item.p9_3 = _itemdb.p9_3;
                    item.p9_4 = _itemdb.p9_4;
                }
                item.p10 = _itemdb.p10;
                if (item.p10.Value)
                {
                    item.p10_1 = _itemdb.p10_1.Value;
                    if (item.p10_1 == 6)
                        item.p10_11 = _itemdb.p10_11;
                    item.p10_21 = _itemdb.p10_21;
                    item.p10_22 = _itemdb.p10_22.Value;
                    item.p10_3 = _itemdb.p10_3;
                }

                item.p11 = _itemdb.p11;
                item.p11_1 = _itemdb.p11_1.Value;
                if (item.p11_1 == 6)
                    item.p11_11 = _itemdb.p11_11;
                item.p11_2 = _itemdb.p11_2.Value;

                item.p12Acheck = _itemdb.p12Acheck;
                if (item.p12Acheck == false)
                    item.p12A = _itemdb.p12A;

                item.p12Bcheck = _itemdb.p12Bcheck;
                if (item.p12Bcheck == false)
                    item.p12B = _itemdb.p12B;

                item.p13 = _itemdb.p13;
                item.p14 = _itemdb.p14;
                if (item.p14 == "2")
                    item.p14_1 = _itemdb.p14_1.Value;

                item.p15 = _itemdb.p15;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen ,lector=lector,isVisible=isVisible});
            }
        }
        #endregion

        #region PIE
        public ActionResult LectorPie(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaPieViewModel item = new EncuestaPieViewModel();
            var _itemdb = db.HISTORIA_CLINICA_PIE.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Pie
                item.p1 = _itemdb.p1;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2.Value;
                    item.p1_3 = _itemdb.p1_3.Value;
                    item.p1_5 = _itemdb.p1_5.Value;
                    item.p1_4 = _itemdb.p1_4.Value;
                    if (item.p1_4)
                    {
                        item.p1_41 = _itemdb.p1_41;
                    }
                }

                item.p2 = _itemdb.p2;
                item.p3 = _itemdb.p3;
                item.p3_1 = _itemdb.p3_1;

                item.p4_1 = _itemdb.p4_1 == null ? "" : _itemdb.p4_1;
                item.p4_2 = _itemdb.p4_2.Value;
                if (!(_itemdb.p4_3 == false && (_itemdb.p4_3_1 == null || _itemdb.p4_3_1 == "")))
                {
                    item.p4_3 = _itemdb.p4_3;
                    item.p4_3_1 = _itemdb.p4_3_1;
                }

                item.p5 = _itemdb.p5;
                if (item.p5 != "3")
                {
                    item.p5_1 = _itemdb.p5_1;
                }

                item.p6 = _itemdb.p6;
                if (item.p6 == "1")
                {
                    item.p6_1 = _itemdb.p6_1;
                }
                item.p7 = _itemdb.p7;
                item.p8 = _itemdb.p8;
                item.p9 = _itemdb.p9;

                item.p10 = _itemdb.p10;
                if (item.p10.Value)
                {
                    item.p10_1 = _itemdb.p10_1;
                    item.p10_21 = _itemdb.p10_21;
                    item.p10_22 = _itemdb.p10_22.Value;
                    item.p10_3 = _itemdb.p10_3;
                    item.p10_4 = _itemdb.p10_4;
                }

                item.p11 = _itemdb.p11;
                if (item.p11.Value)
                {
                    item.p11_1 = _itemdb.p11_1;
                    item.p11_21 = _itemdb.p11_21.Value;
                    item.p11_22 = _itemdb.p11_22.Value;
                }
                //12
                item.p12 = _itemdb.p12;
                if (item.p12.Value)
                {
                    item.p12_1 = _itemdb.p12_1.Value;
                    if (item.p12_1 == 6)
                    {
                        item.p12_11 = _itemdb.p12_11;
                    }
                    item.p12_21 = _itemdb.p12_21;
                    item.p12_22 = _itemdb.p12_22.Value;
                    item.p12_3 = _itemdb.p12_3;
                }

                item.p13 = _itemdb.p13;
                if (item.p13.Value)
                {
                    item.p13_11 = _itemdb.p13_11.Value;
                    if (item.p13_11 == 6)
                    {
                        item.p13_12 = _itemdb.p13_12;
                    }
                    item.p13_2 = _itemdb.p13_2.Value;
                }

                item.p14A_2 = _itemdb.p14A_2;
                if (item.p14A_2 == false)
                    item.p14A_1 = _itemdb.p14A_1;

                item.p14B_2 = _itemdb.p14B_2;
                if (item.p14B_2 == false)
                    item.p14B_1 = _itemdb.p14B_1;

                item.p15 = _itemdb.p15;
                item.p16 = _itemdb.p16;
                if (item.p16 == "2")
                    item.p16_1 = _itemdb.p16_1.Value;

                item.p17 = _itemdb.p17;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen });
            }
        }
        #endregion

        #region ONCOLOGICO
        public ActionResult LectorOncologio(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaOncologicoViewModel item = new EncuestaOncologicoViewModel();
            var _itemdb = db.HISTORIA_CLINICA_ONCOLOGICO.Where(x => x.numeroestudio == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Oncologico
                item.p1 = _itemdb.p1.Value;
                item.p1_1 = _itemdb.p1_1;

                //2 diagnostico

                item.p2 = _itemdb.p2.Value;

                item.p2_1 = _itemdb.p2_1;
                item.p2_1_1 = _itemdb.p2_1_1.Value;
                item.p2_1_2 = _itemdb.p2_1_2;
                item.p2_2 = _itemdb.p2_2;
                item.p2_3 = _itemdb.p2_3;

                // 3 Motivo 
                item.p3 = _itemdb.p3.Value;
                item.p3_1 = _itemdb.p3_1;
                //4
                item.p4 = _itemdb.p4;
                item.p4_1 = _itemdb.p4_1;

                //5
                item.p5 = _itemdb.p5;
                //Histerectomia
                item.p5_1 = _itemdb.p5_1;
                item.p5_1_1 = _itemdb.p5_1_1.Value;
                item.p5_1_2 = _itemdb.p5_1_2;
                //Prostatectomia
                item.p5_2 = _itemdb.p5_2;
                item.p5_2_1 = _itemdb.p5_2_1.Value;
                item.p5_2_2 = _itemdb.p5_2_2;
                //Colecistectomia
                item.p5_3 = _itemdb.p5_3;
                item.p5_3_1 = _itemdb.p5_3_1.Value;
                item.p5_3_2 = _itemdb.p5_3_2;
                //Otra
                item.p5_4 = _itemdb.p5_4;

                //6
                item.p6 = _itemdb.p6;
                item.p6_1 = _itemdb.p6_1;
                item.p6_2 = _itemdb.p6_2.Value;
                item.p6_3 = _itemdb.p6_3;

                //7
                item.p7 = _itemdb.p7;
                item.p7_1 = _itemdb.p7_1;
                item.p7_2 = _itemdb.p7_2.Value;
                item.p7_3 = _itemdb.p7_3;

                //8
                item.p8 = _itemdb.p8.Value;
                if (item.p8.Value)
                {
                    item.p8_1 = _itemdb.p8_1.Value;
                    if (item.p8_1 == 6)
                    {
                        item.p8_2 = _itemdb.p8_1.Value;
                    }
                    else
                    {
                        item.p8_2 = 0;
                    }
                    item.p8_3 = _itemdb.p8_3;
                    item.p8_4 = _itemdb.p8_4.Value;
                    item.p8_5 = _itemdb.p8_5;
                }

                //9
                item.p9 = _itemdb.p9;

                item.p10 = _itemdb.p10;

                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region GENERICA RM (VACIA)
        public ActionResult EncuestaVacia(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaGenericaRMViewModel item = new EncuestaGenericaRMViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;


                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaVacia(EncuestaGenericaRMViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                try
                {

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return View(item);
                }

                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        /*******************************************************************************************
         *      TEM
        *******************************************************************************************/

        #region COLONOSCOPIA
        public ActionResult LectorColonoscopia(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaColonoscopiaViewModel item = new EncuestaColonoscopiaViewModel();
            var _itemdb = db.HISTORIA_CLINICA_TEM_COLONOSCOPIA.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Colonoscopia
                item.p1 = _itemdb.p1.Value;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2;
                    item.p1_3 = _itemdb.p1_3;
                }

                item.p2 = _itemdb.p2.Value;
                if (item.p2.Value)
                    item.p2_1 = _itemdb.p2_1;

                item.p3_1 = _itemdb.p3_1.Value;
                item.p3_2 = _itemdb.p3_2.Value;
                item.p3_3 = _itemdb.p3_3.Value;
                item.p3_4 = _itemdb.p3_4.Value;
                item.p3_5 = _itemdb.p3_5.Value;
                item.p3_6 = _itemdb.p3_6.Value;
                item.p3_7 = _itemdb.p3_7;

                item.p4 = _itemdb.p4.Value;
                if (item.p4.Value)
                {
                    item.p4_1 = _itemdb.p4_1.Value;
                    item.p4_2 = _itemdb.p4_2;
                    item.p4_3 = _itemdb.p4_3;
                }

                item.p5 = _itemdb.p5.Value;
                if (item.p5.Value)
                {
                    item.p5_1 = _itemdb.p5_1.Value;
                    item.p5_2 = _itemdb.p5_2;
                    item.p5_3 = _itemdb.p5_3;
                }

                item.p6 = _itemdb.p6;
                item.p7 = _itemdb.p7;

                item.p8 = _itemdb.p8.Value;
                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region ANGIOTEM

        #region ANGIGOTEM CORONARIO
        public ActionResult LectorAngioCoronario(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaAngioCoronarioViewModel item = new EncuestaAngioCoronarioViewModel();
            var _itemdb = db.HISTORIA_CLINICA_TEM_ARTERIAS.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Coronario
                item.p1 = _itemdb.p1.Value;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2;
                    item.p1_3 = _itemdb.p1_3;
                }

                item.p2 = _itemdb.p2.Value;
                if (item.p2.Value)
                    item.p2_1 = _itemdb.p2_1;

                item.p3 = _itemdb.p3.Value;
                item.p3_1 = _itemdb.p3_1;

                item.p4 = _itemdb.p4.Value;
                item.p4_1 = _itemdb.p4_1;

                item.p5 = _itemdb.p5.Value;
                item.p5_1 = _itemdb.p5_1;

                item.p6 = _itemdb.p6.Value;
                item.p6_1 = _itemdb.p6_1;


                item.p7_1 = _itemdb.p7_1.Value;
                item.p7_2 = _itemdb.p7_2.Value;
                item.p7_3 = _itemdb.p7_3.Value;
                item.p7_4 = _itemdb.p7_4;


                item.p8 = _itemdb.p8;
                item.p9 = _itemdb.p9;
                item.p10 = _itemdb.p10;
                item.p11 = _itemdb.p11;
                item.p12 = _itemdb.p12;

                item.p13 = _itemdb.p13.Value;
                item.p14 = _itemdb.p14.Value;
                item.p15 = _itemdb.p15.Value;
                item.p16 = _itemdb.p16.Value;
                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region ANGIGOTEM AORTA
        public ActionResult LectorAngioAorta(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaAngioAortaViewModel item = new EncuestaAngioAortaViewModel();
            var _itemdb = db.HISTORIA_CLINICA_TEM_AORTA.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Colonoscopia
                item.p1 = _itemdb.p1.Value;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2;
                    item.p1_3 = _itemdb.p1_3;
                }

                item.p2 = _itemdb.p2.Value;
                item.p2_1 = _itemdb.p2_1;

                item.p3 = _itemdb.p3.Value;
                item.p3_1 = _itemdb.p3_1;

                item.p4 = _itemdb.p4.Value;
                item.p4_1 = _itemdb.p4_1;

                /*item.p5_1 = _itemdb.p5_1;
                item.p5_2 = _itemdb.p5_2;*/
                item.p5_3 = _itemdb.p5_3;

                item.p6 = _itemdb.p6;
                item.p7_1 = _itemdb.p7_1.Value;
                if (item.p7_1 == false)
                    item.p7 = _itemdb.p7;

                item.p8_1 = _itemdb.p8_1.Value;
                if (item.p8_1 == false)
                    item.p8 = _itemdb.p8;

                item.p9 = _itemdb.p9;
                item.p10 = _itemdb.p10;

                item.p11_1 = _itemdb.p11_1.Value;
                item.p11_2 = _itemdb.p11_2.Value;
                item.p11_3 = _itemdb.p11_3.Value;
                item.p11_4 = _itemdb.p11_4.Value;
                item.p11_5 = _itemdb.p11_5.Value;
                item.p11_6 = _itemdb.p11_6.Value;
                item.p12 = _itemdb.p12;
                //if (item.p12 == "2")
                //item.p12_1 = _itemdb.p12_1.Value;
                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region ANGIGOTEM CEREBRAL (Deprecate)
        //public ActionResult EncuestaAngioCerebral(int examen, int tipo,bool isVisible)
        //{
        //    var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
        //    var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
        //    if (_encu != null)
        //    {
        //        var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
        //        EncuestaAngioCerebralViewModel item = new EncuestaAngioCerebralViewModel();
        //        item._examen = _exam;
        //        item._paciente = _paci;
        //        item.numeroexamen = _exam.codigo;
        //        item.equipoAsignado = 0;
        //        item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
        //        item.sexo = _paci.sexo;
        //        item.tipo_encu = tipo;

        //        ViewBag.tiempo = new SelectList((new Variable()).getTiempoTEM(), "codigo", "nombre");
        //        ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
        //        ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
        //        ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
        //        ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
        //        return View(item);


        //    }
        //    else
        //        return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EncuestaAngioCerebral(EncuestaAngioCerebralViewModel item)
        //{
        //    var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
        //    var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
        //    if (_encu != null)
        //    {
        //        var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
        //        item._examen = _exam;
        //        item._paciente = _paci;
        //        item.numeroexamen = _exam.codigo;
        //        item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
        //        item.sexo = _paci.sexo;

        //        ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
        //        ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
        //        ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
        //        ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
        //        ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

        //        try
        //        {
        //            #region Cargar Coronario
        //            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
        //            HISTORIA_CLINICA_TEM_CEREBRAL encu = new HISTORIA_CLINICA_TEM_CEREBRAL();
        //            encu.numeroexamen = item.numeroexamen;
        //            encu.usu_reg = user.ProviderUserKey.ToString();
        //            encu.fec_reg = DateTime.Now;

        //            encu.p1 = item.p1.Value;
        //            if (encu.p1.Value)
        //            {
        //                encu.p1_1 = item.p1_1;
        //                encu.p1_2 = item.p1_2;
        //                encu.p1_3 = item.p1_3;
        //            }

        //            encu.p2 = item.p2.Value;
        //            if (encu.p2.Value)
        //                encu.p2_1 = item.p2_1;

        //            encu.p3 = item.p3.Value;
        //            if (encu.p3.Value)
        //            {
        //                encu.p3_1 = item.p3_1;
        //                encu.p3_2 = item.p3_2;
        //            }

        //            encu.p4 = item.p4.Value;
        //            if (encu.p4.Value)
        //            {
        //                encu.p4_1 = item.p4_1;
        //                encu.p4_2 = item.p4_2;
        //            }

        //            encu.p5_1 = item.p5_1;
        //            encu.p5_2 = item.p5_2;
        //            encu.p5_3 = item.p5_3;
        //            encu.p5_4 = item.p5_4;

        //            encu.p6 = "";
        //            encu.p7 = item.p7;
        //            encu.p8 = item.p8;
        //            encu.p9 = item.p9;
        //            encu.p10 = item.p10;
        //            encu.p11 = 0;

        //            db.HISTORIA_CLINICA_TEM_CEREBRAL.Add(encu);

        //            #endregion

        //            _exam.equipoAsignado = item.equipoAsignado;
        //            _encu.fec_paso3 = DateTime.Now;
        //            _encu.estado = 1;
        //            db.SaveChanges();
        //        }
        //        catch (Exception)
        //        {
        //            return View(item);
        //        }
        //        return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        //    }
        //    else
        //        return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        //}
        #endregion

        #region ANGIOTEM CEREBRAL

        public ActionResult LectorTEMCerebro(int examen, int tipo, bool isVisible,string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaTEMCerebroViewModel item = new EncuestaTEMCerebroViewModel();

            //cargar datos de la encuesta cerebro
            var _cerebro = db.HISTORIA_CLINICA_TEM_CEREBRO.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_cerebro != null)
            {
                item = GetEntity_TEM(_cerebro);
                item._examen = _exam;
                item._paciente = _paci;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        public EncuestaTEMCerebroViewModel GetEntity_TEM(HISTORIA_CLINICA_TEM_CEREBRO item)
        {
            var entidad = new EncuestaTEMCerebroViewModel();
            entidad.numeroexamen = item.numeroexamen;

            entidad.p1 = item.p1;
            if (entidad.p1.Value)
            {
                entidad.p1_1 = item.p1a[0].ToString() == "1";
                entidad.p1_2 = item.p1a[1].ToString() == "1";
                entidad.p1_3 = item.p1a[2].ToString() == "1";
                if (entidad.p1_3)
                    entidad.p1_4 = item.p1a31;
            }
            entidad.p2 = item.p2;
            if (entidad.p2.Value)
            {
                entidad.p2_1 = item.p2a.Value;
            }
            entidad.p3 = item.p3;
            if (entidad.p3.Value)
            {
                entidad.p3_1 = item.p3a.Value;
                if (entidad.p3_1.Value)
                {
                    //1
                    entidad.p3_1_1 = item.p3a1.Value;
                    //2
                    entidad.p3_1_2 = item.p3a2.Value;
                    //3
                    entidad.p3_1_3 = item.p3a3[0].ToString() == "1";
                    entidad.p3_1_4 = item.p3a3[1].ToString() == "1";
                    entidad.p3_1_5 = item.p3a3[2].ToString() == "1";
                }
            }
            entidad.p4 = item.p4;
            if (entidad.p4.Value)
            {
                entidad.p4_1 = item.p4a;
            }
            entidad.p5 = item.p5;
            if (entidad.p5.Value)
            {
                entidad.p5_1 = item.p5a[0].ToString() == "1";
                entidad.p5_2 = item.p5a[1].ToString() == "1";
                entidad.p5_3 = item.p5a[2].ToString() == "1";
                entidad.p5_4 = item.p5a[3].ToString() == "1";
                entidad.p5_4_1 = item.p5a41;
            }
            entidad.p6 = item.p6;
            if (entidad.p6.Value)
            {
                entidad.p6_1 = item.p6a[0].ToString() == "1";
                entidad.p6_2 = item.p6a[2].ToString() == "1";
                entidad.p6_3 = item.p6a[4].ToString() == "1";

                entidad.p6_4 = item.p6a[1].ToString() == "1";
                entidad.p6_5 = item.p6a[3].ToString() == "1";
                entidad.p6_6 = item.p6a[5].ToString() == "1";

                entidad.p6_7 = item.p6a[6].ToString() == "1";
                entidad.p6_7_1 = item.p6a71;
            }
            entidad.p7 = item.p7;
            string[] s = item.p8.Split('%');
            entidad.p8 = int.Parse(s[0]);
            entidad.p8_1 = int.Parse(s[1]);
            entidad.p9 = item.p9;
            entidad.p9_1 = item.p9a1;
            entidad.p10 = item.p10;
            if (entidad.p10.Value)
            {
                entidad.p10_1 = item.p10a71;
            }
            entidad.p11 = item.p11;
            if (entidad.p11.Value)
            {
                entidad.p11_1 = item.p11a;
                string[] s11 = item.p11b.Split('%');
                entidad.p11_2 = int.Parse(s[0]);
                entidad.p11_3 = int.Parse(s[1]);
                entidad.p11_4 = item.p11c;
            }
            entidad.p12 = item.p12;
            if (entidad.p12.Value)
            {
                //Resonancia
                entidad.p12_1 = item.p12aa.Value;
                if (entidad.p12_1)
                {
                    entidad.p12_1_1 = int.Parse(item.p12a);
                    if (entidad.p12_1_1 == 9)
                        entidad.p12_1_2 = int.Parse(item.p12a41);
                    string[] s12b = item.p12b.Split('%');
                    entidad.p12_1_3 = int.Parse(s12b[0]);
                    entidad.p12_1_4 = int.Parse(s12b[1]);
                    entidad.p12_1_5 = item.p12c;
                }
                entidad.p12_2 = item.p12bb.Value;
                if (entidad.p12_2)
                {
                    entidad.p12_2_1 = int.Parse(item.p12d);
                    if (entidad.p12_2_1 == 9)
                        entidad.p12_2_2 = int.Parse(item.p12d41);
                    string[] s12b = item.p12e.Split('%');
                    entidad.p12_2_3 = int.Parse(s12b[0]);
                    entidad.p12_2_4 = int.Parse(s12b[1]);
                    entidad.p12_2_5 = item.p12f;
                }
            }
            entidad.p13A_1 = item.p13Acheck;
            if (entidad.p13A_1 == false)
                entidad.p13A = item.p13A;
            entidad.p13B_1 = item.p13Bcheck;
            if (entidad.p13B_1 == false)
                entidad.p13B = item.p13B;

            entidad.p14 = item.p14;

            entidad.p15_1 = item.p15_1.Value;
            entidad.p15_2 = item.p15_2.Value;
            entidad.p15_3 = item.p15_3.Value;
            entidad.p15_4 = item.p15_4.Value;
            entidad.p15_5 = item.p15_1.Value;

            entidad.p16 = item.p16;
            if (item.p16_1 != null)
                entidad.p16_1 = item.p16_1.Value;

            return entidad;
        }

        #endregion

        #endregion

        #region MUSCULO ESQUELETICO
        public ActionResult LectorMusculoEsqueletico(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaMusculoEsqueleticoViewModel item = new EncuestaMusculoEsqueleticoViewModel();
            var _itemdb = db.HISTORIA_CLINICA_TEM_MUSCULO.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Musculo Esqueletico
                item.p1 = _itemdb.p1.Value;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2;
                    item.p1_3 = _itemdb.p1_3;
                }

                item.p2 = _itemdb.p2.Value;
                if (item.p2.Value)
                    item.p2_1 = _itemdb.p2_1;

                item.p3_1 = _itemdb.p3_1.Value;
                item.p3_2 = _itemdb.p3_2;

                item.p4 = _itemdb.p4.Value;
                if (item.p4 != 3)
                {
                    item.p4_1 = _itemdb.p4_1;
                }

                item.p5 = _itemdb.p5.Value;
                if (item.p5.Value)
                {
                    item.p5_1 = _itemdb.p5_1;
                }
                item.p6 = _itemdb.p6;

                item.p7 = _itemdb.p7.Value;
                if (item.p7.Value)
                {
                    item.p7_1 = _itemdb.p7_1.Value;
                    item.p7_2 = _itemdb.p7_2;
                    item.p7_3 = _itemdb.p7_3;

                }

                item.p8 = _itemdb.p8.Value;
                if (item.p8.Value)
                {
                    item.p8_1 = _itemdb.p8_1.Split('%')[0];
                    item.p8_1_1 = _itemdb.p8_1.Split('%')[1];
                    item.p8_2 = _itemdb.p8_2.Split('%')[0];
                    item.p8_2_1 = _itemdb.p8_2.Split('%')[1];

                }

                item.p9 = _itemdb.p9.Value;
                if (item.p9.Value)
                {
                    item.p9_1 = _itemdb.p9_1.Value;
                    item.p9_3 = _itemdb.p9_3.Value;
                }

                if (_itemdb.p10_1 != null)
                    item.p10_1 = _itemdb.p10_1.Value;
                item.p10 = _itemdb.p10;

                if (_itemdb.p11_1 != null)
                    item.p11_1 = _itemdb.p11_1.Value;
                item.p11 = _itemdb.p11;

                item.p12 = _itemdb.p12;
                item.p13 = _itemdb.p13;

                item.p14 = _itemdb.p14.Value;

                #endregion
           
            item._examen = _exam;
            item._paciente = _paci;
            item.numeroexamen = _exam.codigo;
            item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
            item.sexo = _paci.sexo;
            item.tipo_encu = tipo;
            //ENCUESTA
            item._encuesta = getEncuestaDetalle(_encu);
            item._encuesta.isVisible = isVisible;
            ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
            ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
            ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
            ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
            ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
            return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region NEURO CEREBRO
        public ActionResult LectorNeuroCerebro(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaNeuroCerebroViewModel item = new EncuestaNeuroCerebroViewModel();
            var _itemdb = db.HISTORIA_CLINICA_TEM_NEUROCEREBRO.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar NeuroCerebro
                item.p1 = _itemdb.p1.Value;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2;
                    item.p1_3 = _itemdb.p1_3;
                }

                item.p2 = _itemdb.p2.Value;
                if (item.p2.Value)
                    item.p2_1 = _itemdb.p2_1;

                item.p3 = _itemdb.p3.Value;
                if (item.p3.Value)
                {
                    item.p3_2 = _itemdb.p3_2;
                }

                item.p4 = _itemdb.p4.Value;
                if (item.p4.Value)
                {
                    item.p4_1 = _itemdb.p4_1;

                }

                item.p5 = _itemdb.p5.Value;

                item.p6 = _itemdb.p6.Value;
                if (item.p6.Value)
                    item.p6_1 = _itemdb.p6_1;

                //7

                item.p7_1 = _itemdb.p7_1.Value;
                item.p7_2 = _itemdb.p7_2.Value;
                item.p7_3 = _itemdb.p7_3.Value;
                item.p7_4 = _itemdb.p7_4.Value;
                item.p7_5 = _itemdb.p7_5.Value;

                item.p8 = _itemdb.p8;
                item.p9 = _itemdb.p9;
                item.p10 = _itemdb.p10;
                item.p11 = _itemdb.p11;
                item.p12 = _itemdb.p12;
                item.p13 = _itemdb.p13;
                item.p14 = _itemdb.p14;
                item.p15 = _itemdb.p15;
                item.p16 = _itemdb.p16;

                item.p17_1 = _itemdb.p17_1.Value;
                item.p17_2 = _itemdb.p17_2.Value;
                item.p17_3 = _itemdb.p17_3.Value;
                item.p17_4 = _itemdb.p17_4.Value;
                item.p17_5 = _itemdb.p17_5.Value;

                #endregion
           
            item._examen = _exam;
            item._paciente = _paci;
            item.numeroexamen = _exam.codigo;
            item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
            item.sexo = _paci.sexo;
            item.tipo_encu = tipo;
            //ENCUESTA
            item._encuesta = getEncuestaDetalle(_encu);
            item._encuesta.isVisible = isVisible;
            ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
            ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
            ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
            ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
            ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
            return View(item);
            }
            else
            {

                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion

        #region GENERICA TEM
        public ActionResult LectorGenericaTEM(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaGenericaTEMModel item = new EncuestaGenericaTEMModel();
            var _itemdb = db.HISTORIA_CLINICA_TEM_GENERICA.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Generica
                item.p1 = _itemdb.p1.Value;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1.Value;
                    item.p1_2 = _itemdb.p1_2;
                    item.p1_3 = _itemdb.p1_3;
                }

                item.p2 = _itemdb.p2.Value;
                if (item.p2.Value)
                    item.p2_1 = _itemdb.p2_1;

                item.p3 = _itemdb.p3.Value;
                if (item.p3.Value)
                {
                    item.p3_2 = _itemdb.p3_2;
                }

                item.p4 = _itemdb.p4.Value;
                if (item.p4.Value)
                {
                    item.p4_1 = _itemdb.p4_1;

                }

                item.p5 = _itemdb.p5.Value;

                item.p6 = _itemdb.p6;
                if (item.p6.Value)
                    item.p6_1 = _itemdb.p6_1;

                //7
                item.p7_1 = _itemdb.p7_1.Value;
                item.p7_2 = _itemdb.p7_2.Value;
                item.p7_3 = _itemdb.p7_3.Value;
                item.p7_4 = _itemdb.p7_4.Value;
                item.p7_5 = _itemdb.p7_5.Value;
                item.p7_6 = _itemdb.p7_6.Value;
                item.p7_7 = _itemdb.p7_7.Value;
                item.p7_8 = _itemdb.p7_8.Value;
                item.p7_9 = _itemdb.p7_9.Value;
                item.p7_10 = _itemdb.p7_10.Value;

                item.p8 = _itemdb.p8;

                item.p9_1 = _itemdb.p9_1.Value;
                item.p9_2 = _itemdb.p9_2.Value;
                item.p9_3 = _itemdb.p9_3.Value;
                item.p9_4 = _itemdb.p9_4.Value;
                item.p9_5 = _itemdb.p9_5.Value;
                item.p9_6 = _itemdb.p9_6.Value;
                item.p9_7 = _itemdb.p9_7.Value;
                item.p9_8 = _itemdb.p9_8.Value;
                item.p9_9 = _itemdb.p9_9.Value;
                item.p9_10 = _itemdb.p9_10.Value;
                item.p9_11 = _itemdb.p9_11.Value;
                item.p9_12 = _itemdb.p9_12.Value;
                item.p10A_1 = _itemdb.p10A_1;
                item.p10A_2 = _itemdb.p10A_2.Value;
                item.p10B_1 = _itemdb.p10B_1;
                item.p10B_2 = _itemdb.p10B_2.Value;
                item.p11 = _itemdb.p11;
                item.p12 = _itemdb.p12;
                if (_itemdb.p12_1 != null)
                    item.p12_1 = _itemdb.p12_1.Value;
                #endregion
           
            item._examen = _exam;
            item._paciente = _paci;
            item.numeroexamen = _exam.codigo;
            item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
            item.sexo = _paci.sexo;
            item.tipo_encu = tipo;
            //ENCUESTA
            item._encuesta = getEncuestaDetalle(_encu);
            item._encuesta.isVisible = isVisible;
            ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
            ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
            ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
            ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
            ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
            return View(item);
            }
            else
            {
                var herramienta = new Herramientas.HerramientasController();
                herramienta.SolucionarProblemasEncuestaInternal(examen);
                return RedirectToAction("LectorEncuesta", "LectorEncuesta", new { examen = examen, lector = lector, isVisible = isVisible });
            }
        }
        #endregion
        //Listar Tecnica
        public string ListarTecnicas(int ncita,string tipo)
        {
            
            List<EXAMENXCITA> lista = new List<EXAMENXCITA>();
            if (tipo == "01")
            {
                lista = db.EXAMENXCITA.Where(x => x.numerocita == ncita && x.codigoestudio.Substring(7, 2) == "99" && x.codigoestudio.Substring(5, 1) == "0").ToList();

            }
            else if (tipo == "02")
            {
                lista = db.EXAMENXCITA.Where(x => x.numerocita == ncita && x.codigoestudio.Substring(6, 3) == "902").ToList();
            }
            else { }
            string tecnicas = "";

            foreach (var item in lista)
            {
                tecnicas += item.ESTUDIO.nombreestudio.ToUpper()+", ";
            }
            return (tecnicas);
        }
  



#endregion

    #region EMETAC
        public ActionResult LectorGenericaTEMEMETAC(int examen, int tipo, bool isVisible, string lector = "")
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
            EncuestaGenericaTEMEMETACModel item = new EncuestaGenericaTEMEMETACModel();
            var _itemdb = db.HISTORIA_CLINICA_GENERICA_EMETAC.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_itemdb != null)
            {
                #region Cargar Generica
                item.p1 = _itemdb.p1.Value;
                if (item.p1.Value)
                {
                    item.p1_1 = _itemdb.p1_1;
                }

                item.p2_1 = _itemdb.p2_1.Value;
                item.p2_2 = _itemdb.p2_2.Value;

                item.p3 = _itemdb.p3.Value;

                item.p4 = _itemdb.p4;
              
                item.p5 = _itemdb.p5.Value;

                item.p6 = _itemdb.p6;
                item.p7 = _itemdb.p7;
                item.p8 = _itemdb.p8.Value;
                #endregion

                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;
                //ENCUESTA
                item._encuesta = getEncuestaDetalle(_encu);
                item._encuesta.isVisible = isVisible;
                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                return View(item);
            }
            return RedirectToAction("ListaEncuesta", "Adquisicion", new { Area = "Medica" });
        }
    #endregion
}
}