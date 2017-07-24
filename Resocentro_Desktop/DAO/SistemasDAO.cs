using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    public class SistemasDAO
    {
        public Dictionary<string, string> getInfoChangeEstudio(string examen)
        {
            Dictionary<string, string> lista = new Dictionary<string, string>();
            string queryString = "select substring(ea.codigoestudio,1,3)idestudio,p.apellidos+', ' +p.nombres paciente, es.nombreestudio,ea.codigocompaniaseguro from EXAMENXATENCION ea inner join estudio es on ea.codigoestudio=es.codigoestudio inner join paciente p on ea.codigopaciente=p.codigopaciente where ea.codigo={0} and ea.estadoestudio<>'X'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString,examen), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                lista.Add("unidad", reader["idestudio"].ToString());
                                lista.Add("paciente", reader["paciente"].ToString());
                                lista.Add("nombreestudio", reader["nombreestudio"].ToString());
                                lista.Add("codigocompaniaseguro", reader["codigocompaniaseguro"].ToString());
                            }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();

                    }
                }
            }
            return lista;
        }

        public bool cambiarEstudioxCodigo(string examen, string estudio)
        {
            bool result = false;
            string queryString = "dbo.cambiar_estudioxatecnion";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@nexamen", SqlDbType.VarChar).Value = examen;
                    command.Parameters.Add("@newestudio", SqlDbType.VarChar).Value = estudio;

                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Prepare();
                  
                    try
                    {
                        command.ExecuteNonQuery();
                        result= true;
                    }
                    finally
                    {
                        
                        connection.Close();

                    }
                }
            }


            return result;
        }
    }
}