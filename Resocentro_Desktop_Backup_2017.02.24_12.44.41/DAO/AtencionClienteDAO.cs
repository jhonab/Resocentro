using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    public class AtencionClienteDAO
    {
        /// <summary>
        /// lista de resultados por paciente o por codigo
        /// </summary>
        /// <param name="filtro"> parametro de busqueda</param>
        /// <param name="tipofiltro"> 1 filtro por apellidos, 2 filtro por codigo</param>
        /// <returns> retorna lista de estudios por entregar</returns>
        public List<ResultadosEntity> getListaResultados(string filtro, int tipofiltro)
        {
            List<ResultadosEntity> lista = new List<ResultadosEntity>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {

                string sqlResultados = "exec getListaResultado 1,'" + filtro + "'," + filtro + "," + tipofiltro + "";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sqlResultados, connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                ResultadosEntity item = new ResultadosEntity();
                                item.fecha = Convert.ToDateTime(reader["fechayhora"].ToString());
                                item.codigo = Convert.ToInt32(reader["codigo"].ToString());
                                item.paciente = reader["paciente"].ToString();
                                item.estudio = reader["nombreestudio"].ToString();
                                item.dni = reader["dni"].ToString();
                                item.telefono = reader["telefono"].ToString();
                                item.direccion = reader["direccion"].ToString();
                                item.numeroatencion = Convert.ToInt32(reader["numeroatencion"].ToString());
                                item.codigopaciente = Convert.ToInt32(reader["codigopaciente"].ToString());
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
        public bool verificarDeudacodigo(int codigo)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {

                string sqlResultados = "exec verificarPagoPaciente " + codigo;
                bool resultado = true;
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sqlResultados, connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                resultado = Convert.ToBoolean(reader["re"]);
                            }
                    }
                    finally
                    {
                        // Always call Close when done reading.<
                        reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }

                }
                return resultado;
            }
        }

        public TKT_Ticketera getTicket(int empresa, int sucursal, bool isCaja, int idocunter)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                TKT_Ticketera item = new TKT_Ticketera();
                string sql = "getTicket";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@empresa", SqlDbType.Int).Value = empresa;
                    command.Parameters.Add("@sucursal", SqlDbType.Int).Value = sucursal;
                    command.Parameters.Add("@estado", SqlDbType.Int).Value = (isCaja?3:0);
                    command.Parameters.Add("@idcounter", SqlDbType.Int).Value = idocunter;

                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        while (reader.Read())
                        {
                            item.id_Ticket = Convert.ToInt32(reader["id_Ticket"].ToString());
                            item.nombre = (reader["nombre"].ToString());
                            item.id_tipo_aten = Convert.ToInt32(reader["id_tipo_aten"].ToString());
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
                return item;

            }

        }
        public TKT_Ticketera getTicketxId(int idticket, int idocunter)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                TKT_Ticketera item = new TKT_Ticketera();
                string sql = "getTicketxID";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@ticket", SqlDbType.Int).Value = idticket;
                    command.Parameters.Add("@idcounter", SqlDbType.Int).Value = idocunter;

                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        while (reader.Read())
                        {
                            item.id_Ticket = Convert.ToInt32(reader["id_Ticket"].ToString());
                            item.nombre = (reader["nombre"].ToString());
                            item.id_tipo_aten = Convert.ToInt32(reader["id_tipo_aten"].ToString());
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
                return item;

            }

        }

        public List<Tickets_Counter> getTicketAtendidos(int empresa, int sede, int idcounter)
        {
            List<Tickets_Counter> lista = new List<Tickets_Counter>();

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string sql = @"select ta.nombre atencion,t.nombre ticket,
case
when t.fec_termino is not null then 'Atendido'
when t.fec_cancela is not null then 'Abandonado'
else 'Atendiendo'end tipo,
case
when t.fec_termino is not null then DATEDIFF(SECOND,t.fec_registro,t.fec_termino) 
when t.fec_cancela is not null then DATEDIFF(SECOND,t.fec_registro,t.fec_cancela)
else '0'end duracion
from TKT_Ticketera t
inner join TKT_Tipo_Atencion ta on t.id_tipo_aten=ta.id_tipo_aten
where convert(date,t.fec_registro)=convert(date,getdate()) and t.idempresa=" + empresa.ToString()+" and t.idSede="+sede.ToString()+" and t.id_counter="+idcounter.ToString();
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            Tickets_Counter item = new Tickets_Counter();
                            item.atencion = reader["atencion"].ToString();
                            item.ticket = reader["ticket"].ToString();
                            item.tipo = reader["tipo"].ToString();
                            TimeSpan result = TimeSpan.FromSeconds(Convert.ToInt32(reader["duracion"].ToString()));
                            item.duracion = result.ToString("mm':'ss");
                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                }
            }
            return lista;
        }

        public List<Tickets_Counter> getTicketEspera(int empresa, int sede)
        {
            List<Tickets_Counter> lista = new List<Tickets_Counter>();

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string sql = @"select t.id_Ticket,t.nombre,ta.nombre tipo,t.estado,DATEDIFF(minute,t.fec_registro,GETDATE()) duracion
from TKT_Ticketera t  inner join TKT_Tipo_Atencion ta on t.id_tipo_aten=ta.id_tipo_aten
where t.idempresa=" + empresa + " and t.idSede=" + sede + @"  and t.isEliminado=0 and t.estado=0  and t.id_counter is null
order by t.estado desc,t.isPreferencial desc,ta.orderby,t.fec_registro;";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            Tickets_Counter item = new Tickets_Counter();
                            item.idTicket = Convert.ToInt32(reader["id_Ticket"].ToString());
                            item.atencion = reader["tipo"].ToString();
                            item.ticket = reader["nombre"].ToString();
                            TimeSpan result = TimeSpan.FromSeconds(Convert.ToInt32(reader["duracion"].ToString()));
                            item.duracion = result.ToString("hh':'mm':'ss");
                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public  void getTicketRellamada(int idticketera)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                TKT_Ticketera item = new TKT_Ticketera();
                string sql = "update TKT_Ticketera set isRellamada=1 where id_Ticket='" + idticketera + "'";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                }
            }
        }

        public void getTicketLiberar(int idticketera)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                TKT_Ticketera item = new TKT_Ticketera();
                string sql = "update TKT_Ticketera set id_counter=null,estado=0 where id_Ticket='" + idticketera + "'";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                }
            }
        }

        public void getTicketAusente(int idticketera, MySession session)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                TKT_Ticketera item = new TKT_Ticketera();
                string sql = "update TKT_Ticketera set fec_cancela=getdate(),usu_cancela='"+session.codigousuario+"',estado=4 where id_Ticket='" + idticketera + "'";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                }
            }
        }
        public void getTicketTerminado(int idticketera, MySession session,bool isCaja)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                TKT_Ticketera item = new TKT_Ticketera();
                string sql = "";
                if(isCaja)
                    sql = "update TKT_Ticketera set fec_caja=getdate(),usu_caja='" + session.codigousuario + "',estado=5 where id_Ticket='" + idticketera + "'";
                else
                    sql = "update TKT_Ticketera set fec_termino=getdate(),usu_termino='" + session.codigousuario + "',estado=5 where id_Ticket='" + idticketera + "'";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                }
            }
        }

        public  string getCantidadPacientes(int empresa, int sede)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                TKT_Ticketera item = new TKT_Ticketera();
                string sql = "select count(*) cantidad from TKT_Ticketera t  where t.idempresa=" + empresa + " and t.idSede=" + sede + "  and t.isEliminado=0 and id_counter is null and t.estado<4";
                string resultado="";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            resultado= (reader["cantidad"].ToString());
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                }
                return "Total de Pacientes: " + resultado;
            }
        }
    }
}
