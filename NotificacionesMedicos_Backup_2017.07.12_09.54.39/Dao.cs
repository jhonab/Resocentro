using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionesMedicos
{
    public class Perifoneo
    {
        public string tipo { get; set; }
        public string ubicacion { get; set; }
    }
    public class Super_Resul
    {
        public int examen { get; set; }
        public string equipo { get; set; }
        public string estudio { get; set; }
        public string supervisor { get; set; }

        public string paciente { get; set; }

        public DateTime inicio { get; set; }
    }
    public  class ConfiguracionPrevia{
        public bool supervisor { get; set; }
        public bool enfermera { get; set; }
        public bool tecnologo { get; set; }
        public string[] equipos { get; set; }
    }
    public class SisEncuesta
    {
        /*
        public List<Super_Resul> BuscarNewSupervisiones()
        {
            string supervisor = "";
            DATABASEGENERALEntities1 dt = new DATABASEGENERALEntities();


            using (SqlConnection connection = new SqlConnection(dt.Database.Connection.ConnectionString))
            {
                string queryString = @"select top(1){0} dia from param_supervision where convert(time,hora)<=convert(time,getdate()) order by hora desc";
                SqlCommand command = new SqlCommand(string.Format(queryString, DateTime.Now.ToString("dddd").Replace("é", "e").Replace("á", "a")), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        supervisor = reader["dia"].ToString();
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            var _pac = (from ea in dt.EXAMENXATENCION
                        join en in dt.Encuesta on ea.codigo equals en.numeroexamen into en_join
                        from en in en_join.DefaultIfEmpty()
                        join e in dt.EQUIPO on new { equipoAsignado = (int)ea.equipoAsignado } equals new { equipoAsignado = e.codigoequipo } into e_join
                        from e in e_join.DefaultIfEmpty()
                        where ea.estadoestudio == "A"
                     && ea.codigoestudio.Substring(1 - 1, 3) == "101"
                     && en.estado == 1
                            //&& en.estado < 4
                     && en.SolicitarValidacion == true
                        orderby ea.codigo
                        select new Super_Resul
                        {
                            paciente = ea.ATENCION.PACIENTE.apellidos + ", " + ea.ATENCION.PACIENTE.nombres,
                            examen = ea.codigo,
                            equipo = e.ShortDesc,
                            estudio = ea.ESTUDIO.nombreestudio,
                            supervisor = supervisor,
                            inicio = en.fec_Solicitavalidacion.Value
                        }).ToList();

            return _pac;

        }
        */
        public string getusuario(string session)
        {
            using (DATABASEGENERALEntities1 db = new DATABASEGENERALEntities1())
            {
                var usu = db.USUARIO.SingleOrDefault(x => x.username == session);
                if (usu == null)
                    return "";
                else
                    return usu.codigousuario;
            }
        }

        public List<EQUIPO> getEquipo()
        {
            
            using (DATABASEGENERALEntities1 db = new DATABASEGENERALEntities1())
            {
                return db.EQUIPO.Where(x => x.codigounidad2 == 1 && x.estado == "1").ToList();
                
            }
            
        }
      
        public List<Perifoneo> BuscarPerifoneo()
        {
            using (RepositorioJhonEntities db = new RepositorioJhonEntities())
            {
                List<Perifoneo> lista = new List<Perifoneo>();
                var fecha = DateTime.Now;
                var _lis = (from a in db.AlertasEncuestas
                            where a.fecha.Day == fecha.Day && a.fecha.Month == fecha.Month && a.fecha.Year == fecha.Year && a.isReproducido == false
                            select a).ToList();
                foreach (var item in _lis)
                {
                    Perifoneo p = new Perifoneo();
                    if (item.tipo_solicitud.ToLower() == "enfermera")
                        p.tipo = "enfermera.mp3";
                    else if (item.tipo_solicitud.ToLower() == "supervisor")
                        p.tipo = "supervisor.mp3";
                    else
                        p.tipo = "";


                    p.ubicacion = item.ubicacion_solicitante.ToLower() + ".mp3";
                    item.isReproducido = true;
                    db.SaveChanges();
                    lista.Add(p);

                }
                return lista;
            }
        }

      
    }
}
