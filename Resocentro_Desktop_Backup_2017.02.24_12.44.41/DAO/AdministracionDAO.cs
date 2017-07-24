using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    public class AdministracionDAO
    {

        public List<AsignacionSedaciones> getSedacionesRealizadas(string fecha)
        {
            List<AsignacionSedaciones> lista = new List<AsignacionSedaciones>();
            string queryString = @"  select (select count(*) from CONTROL_SEDACION s where s.examen = ea.codigo  )  existe,ec.codigoexamencita,isnull((select isnull(us.shortname,'')from USUARIO us where us.codigousuario=ec.sedador),'')sedador,convert(date,a.fechayhora) fechayhora,ea.codigo,p.apellidos+', '+p.nombres paciente,e.nombreestudio estudio from EXAMENXATENCION ea inner join ATENCION a on ea.numeroatencion=a.numeroatencion inner join ESTUDIO e on ea.codigoestudio=e.codigoestudio inner join CITA c on ea.numerocita=c.numerocita inner join EXAMENXCITA ec on ec.codigoestudio=ea.codigoestudio and ec.numerocita=ea.numerocita inner join paciente p on ea.codigopaciente = p.codigopaciente where convert(date,a.fechayhora)='{0}' and c.sedacion=1 and ec.estadoestudio!='X' and SUBSTRING(ec.codigoestudio,7,1)!='9' and SUBSTRING(ec.codigoestudio,8,2)!='99'
";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, fecha), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            AsignacionSedaciones item = new AsignacionSedaciones();
                            if ((reader["existe"]).ToString() == "1")
                                item.existe = true;
                            else
                                item.existe = false;

                            item.sedador = Convert.ToString(reader["sedador"]);
                            item.fecha = Convert.ToDateTime(reader["fechayhora"]);
                            item.codigo = Convert.ToInt32(reader["codigo"]);
                            item.paciente = Convert.ToString(reader["paciente"]);
                            item.estudio = Convert.ToString((reader["estudio"]).ToString());
                            item.detallecita = Convert.ToInt32((reader["codigoexamencita"]).ToString());
                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }
                }
                return lista;
            }
        }
        public List<Com_Medico> listSedadores()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.Com_Medico.Where(x => x.isActivo == true).ToList();
            }
        }

        public List<AsignacionInsumos> listAsignacionInsumo()
        {
            List<AsignacionInsumos> lista = new List<AsignacionInsumos>();
            string queryString = "select i.Al_InsumId ,i.Nombre , i.Comentario , isNull(i.CorrelativoPref,' ') correlativo,isnull(e.isVisibleEnfermeria,'0') isVisibleEnfermeria from AL_Insum i left join AL_Enfermeria e on i.Al_InsumId=e.Al_InsumoId where i.IsActive=1";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new AsignacionInsumos();
                            item.idinsumo = Convert.ToInt32(reader["Al_InsumId"].ToString());
                            item.nombre = (reader["Nombre"].ToString());
                            item.comentario = (reader["Comentario"].ToString());
                            item.correlativo = (reader["correlativo"].ToString());
                            item.estado = Convert.ToBoolean(reader["isVisibleEnfermeria"].ToString());
                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return lista;
        }

        public List<Colaboradores> getEmpleados()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.USUARIO.Where(x => x.bloqueado == false).OrderBy(x => x.EMPLEADO.apellidos).Select(x => new Colaboradores { codigo=x.codigousuario,valor=x.EMPLEADO.apellidos+" "+x.EMPLEADO.nombres}).ToList();
            }
        }

        public List<ConfirmacionLecturaPago> getLogCnfirmacionPago(int mes , int year)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.ConfirmacionLecturaPago.Where(x => x.fecha.Month==mes && x.fecha.Year==year).ToList();
            }
        }

    }
}
