using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Encuesta;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Encuesta
{
    public class VisorCargaController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        //
        // GET: /VisorCarga/

        public ActionResult VisorCarga()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Variable _u = new Variable();

            //Lista de Equipos

            IList<VisorCargaViewModell> list = new List<VisorCargaViewModell>();


            string queryString = @"exec getCargaEncuesta '" + String.Join(",", user.sucursales.ToArray()) + "'";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format(queryString, user.sucursales.ToString()), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                VisorCargaViewModell visor =null;
                    string _cequipo = "";
                try
                {
                   
                    while (reader.Read())
                    {
                        if (_cequipo != reader["codigoequipo"].ToString())
                        {
                            if (visor != null)
                                list.Add(visor);
                            visor = new VisorCargaViewModell();
                            visor.codigoequipo = Convert.ToInt32(reader["codigoequipo"].ToString());
                            visor.nombreequipo = (reader["nombreequipo"].ToString());
                            visor.estudios = new List<EstudiosA>();
                            _cequipo = reader["codigoequipo"].ToString();
                        }
                        
                           visor.estudios.Add(
                            new EstudiosA
                            {
                                codigo = Convert.ToInt32(reader["codigo"].ToString()),
                                paciente = reader["paciente"].ToString(),
                                estudio = reader["nombreestudio"].ToString(),
                                inicio = reader["fec_ini_tecno"]!=DBNull.Value?Convert.ToDateTime(reader["fec_ini_tecno"].ToString()):(DateTime?)null,
                                min_transcurri = reader["minuto"].ToString()
                            }
                            );
                      
                        

                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    if (visor != null)
                        list.Add(visor);
                    reader.Close();
                }
            }

/*
            var equipo = (from eq in db.EQUIPO
                          where eq.estado == "1" &&
                          (user.sucursales_int).Contains(eq.codigounidad2 * 100 + eq.codigosucursal2) //sucursales asignadas
                          select eq).AsParallel().ToList();
            foreach (var item in equipo)
            {
                VisorCargaViewModell visor = new VisorCargaViewModell();
                visor.codigoequipo = item.codigoequipo;
                visor.nombreequipo = item.nombreequipo;
                visor.estudios = (from ea in db.EXAMENXATENCION
                                  join en in db.Encuesta on new { numeroexamen = ea.codigo } equals new { numeroexamen = en.numeroexamen }
                                  where ea.equipoAsignado == item.codigoequipo
                                  && ea.estadoestudio == "A"
                                  && en.SolicitarValidacion == null

                                  orderby ea.equipoAsignado
                                  select new EstudiosA
                                  {
                                      codigo = ea.codigo,
                                      paciente = ea.ATENCION.PACIENTE.apellidos + " " + ea.ATENCION.PACIENTE.nombres,
                                      estudio = ea.ESTUDIO.nombreestudio,
                                      inicio = en.fec_ini_tecno,
                                      min_transcurri = 
                                  }).AsParallel().ToList();
                list.Add(visor);
            }*/


            return View(list);
        }


        public ActionResult getEquipo(int examen)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _estudio = db.EXAMENXATENCION.Where(x => x.codigo == examen).Single().codigoestudio;
            var uni = int.Parse(_estudio.Substring(0, 1));
            var sede = int.Parse(_estudio.Substring(1, 2));
            return Json(new SelectList((from eq in db.EQUIPO
                                        where eq.estado == "1" &&
                                        eq.codigounidad2 == uni &&
                                        eq.codigosucursal2 == sede
                                        select eq).AsQueryable(), "codigoequipo", "nombreequipo"), JsonRequestBehavior.AllowGet);
        }


    }
}
