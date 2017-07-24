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
        public async Task<List<ResultadosEntity>> getListaResultados(string filtro, int tipofiltro)
        {
            List<ResultadosEntity> lista = new List<ResultadosEntity>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                int examen = 0;
                int.TryParse(filtro, out examen);
                string sqlResultados = "exec getListaResultado 1,'" + filtro + "','" + examen.ToString() + "'," + tipofiltro + "";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sqlResultados, connection);
                    connection.Open();
                    command.Prepare();
                    
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                ResultadosEntity item = new ResultadosEntity();
                                while (reader.Read())
                                {
                                    if (item.numeroatencion != Convert.ToInt32(reader["numeroatencion"].ToString()))
                                    {
                                        if (item.numeroatencion != 0)
                                            lista.Add(item);
                                        item = new ResultadosEntity();
                                        item.estudios = new List<ListaEstudiosResultados>();
                                    }
                                    item.fecha = Convert.ToDateTime(reader["fechayhora"].ToString());
                                    item.paciente = reader["paciente"].ToString();
                                    item.dni = reader["dni"].ToString();
                                    item.telefono = reader["telefono"].ToString();
                                    item.direccion = reader["direccion"].ToString();
                                    item.nombreestudios = reader["nombreestudio"].ToString();
                                    item.numeroatencion = Convert.ToInt32(reader["numeroatencion"].ToString());
                                    item.codigopaciente = Convert.ToInt32(reader["codigopaciente"].ToString());
                                    item.estudios.Add(new ListaEstudiosResultados { codigo = Convert.ToInt32(reader["codigo"].ToString()), estudio = reader["nombreestudio"].ToString(),foto=0,placa=0 });


                                }
                                if (item.numeroatencion != 0)
                                    lista.Add(item);
                            }
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Dispose();
                        connection.Close();
                    }

                }
            }

            return lista;
        }

        public List<ListaEstudiosResultados> getEstudiosxAtencionResultados(int atencion)
        {
            string sqlResultados = @"select ea.codigo,e.nombreestudio from ATENCION a 
inner join EXAMENXATENCION ea on a.numeroatencion=ea.numeroatencion
inner join PACIENTE p on ea.codigopaciente=p.codigopaciente
inner join ESTUDIO e on ea.codigoestudio=e.codigoestudio
inner join CARGOINFORMES ci on ci.numeroestudio=ea.codigo 
where a.numeroatencion=" + atencion.ToString() + @" 
and ci.estadocargo='E'";
            List<ListaEstudiosResultados> lista = new List<ListaEstudiosResultados>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
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
                                ListaEstudiosResultados item = new ListaEstudiosResultados();
                                item.codigo = Convert.ToInt32(reader["codigo"].ToString());
                                item.estudio = reader["nombreestudio"].ToString();
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

        public List<ResultadosEntregadosEntity> getListaResultadosEntregados(int codigopaciente)
        {
            List<ResultadosEntregadosEntity> lista = new List<ResultadosEntregadosEntity>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string sqlResultados = @"SELECT ea.codigo ,a.fechayhora ,es.nombreestudio,ci.fechaimprime, ShortName,ci.recepciona,CASE WHEN estadocargo='O' THEN 'ENTREGADO PACIENTE' ELSE '' END AS Despacho,re.numerodelivery
from ATENCION a 
inner join EXAMENXATENCION ea on a.numeroatencion=ea.numeroatencion
inner join ESTUDIO es on ea.codigoestudio=es.codigoestudio
left join CARGOINFORMES ci on ea.codigo=ci.numeroestudio
left join USUARIO u on u.codigousuario=ci.codigousuario
left join RESULTADODELIVERY re on re.numeroatencion=ea.numeroatencion
WHERE ci.codigopaciente=" + codigopaciente.ToString();
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
                                ResultadosEntregadosEntity item = new ResultadosEntregadosEntity();

                                item.codigo = Convert.ToInt32(reader["codigo"].ToString());
                                item.fecha = Convert.ToDateTime(reader["fechayhora"].ToString());
                                item.estudio = reader["nombreestudio"].ToString();
                                item.fechasalida = Convert.ToDateTime(reader["fechaimprime"].ToString());
                                if (reader["ShortName"] != DBNull.Value)
                                    item.impreso = reader["ShortName"].ToString();
                                if (reader["recepciona"] != DBNull.Value)
                                    item.entregado = reader["recepciona"].ToString();
                                if (reader["Despacho"] != DBNull.Value)
                                    item.despacho = reader["Despacho"].ToString();
                                if (reader["numerodelivery"] != DBNull.Value)
                                    item.codigodelivery = Convert.ToInt32(reader["numerodelivery"].ToString());
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
        public async Task<List<ResultadosEntregadosEntity>> getListaResultadosEntregadosDiario(string codigousuario)
        {
            List<ResultadosEntregadosEntity> lista = new List<ResultadosEntregadosEntity>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string sqlResultados = @"SELECT isnull(re.numerodelivery,'0')numerodelivery ,p.apellidos+', '+p.nombres paciente,ea.codigo ,a.fechayhora ,es.nombreestudio,ci.fechaimprime, ShortName,ci.recepciona,CASE WHEN estadocargo='O' THEN 'ENTREGADO PACIENTE' ELSE '' END AS Despacho,re.numerodelivery
from ATENCION a 
inner join EXAMENXATENCION ea on a.numeroatencion=ea.numeroatencion
inner join PACIENTE p on ea.codigopaciente=p.codigopaciente
inner join ESTUDIO es on ea.codigoestudio=es.codigoestudio
left join CARGOINFORMES ci on ea.codigo=ci.numeroestudio
left join USUARIO u on u.codigousuario=ci.codigousuario
left join RESULTADODELIVERY re on re.numeroatencion=ea.numeroatencion
WHERE  convert(date,re.fechasalida) = convert(date,getdate()) and re.codigousuario='" + codigousuario + "' order by re.fechasalida";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sqlResultados, connection);
                    connection.Open();
                    command.Prepare();
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    ResultadosEntregadosEntity item = new ResultadosEntregadosEntity();

                                    item.codigo = Convert.ToInt32(reader["codigo"].ToString());
                                    item.paciente = reader["paciente"].ToString();
                                    item.numerodelivery = Convert.ToInt32(reader["numerodelivery"].ToString());
                                    item.fecha = Convert.ToDateTime(reader["fechayhora"].ToString());
                                    item.estudio = reader["nombreestudio"].ToString();
                                    item.fechasalida = Convert.ToDateTime(reader["fechaimprime"].ToString());
                                    if (reader["ShortName"] != DBNull.Value)
                                        item.impreso = reader["ShortName"].ToString();
                                    if (reader["recepciona"] != DBNull.Value)
                                        item.entregado = reader["recepciona"].ToString();
                                    if (reader["Despacho"] != DBNull.Value)
                                        item.despacho = reader["Despacho"].ToString();
                                    if (reader["numerodelivery"] != DBNull.Value)
                                        item.codigodelivery = Convert.ToInt32(reader["numerodelivery"].ToString());
                                    lista.Add(item);
                                }
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Dispose();
                        connection.Close();
                    }

                }
            }

            return lista;
        }
        public void setEntregadoPaciente(int numeroatencion)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {

                string sqlResultados = "update c set c.estadocargo='O' from CARGOINFORMES c inner join EXAMENXATENCION ea on c.numeroestudio=ea.codigo inner join ATENCION a on ea.numeroatencion=a.numeroatencion where c.estadocargo='E' and a.numeroatencion=" + numeroatencion;

                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sqlResultados, connection);
                    connection.Open();
                    command.Prepare();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Dispose();
                        connection.Close();
                    }

                }
            }
        }
        public async Task<bool>verificarDeudacodigo(int codigo)
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
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                             if (reader.HasRows)
                            while (reader.Read())
                            {
                                resultado = Convert.ToBoolean(reader["re"]);
                            }
                        }
                       
                    }
                    finally
                    {
                        // Always call Close when done reading.<
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
                    command.Parameters.Add("@estado", SqlDbType.Int).Value = (isCaja ? 3 : 0);
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
                string sql = @"select ta.nombre atencion,t.nombre ticket,t.fec_registro,
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
where convert(date,t.fec_registro)=convert(date,getdate()) and t.idempresa=" + empresa.ToString() + " and t.idSede=" + sede.ToString() + " and t.id_counter=" + idcounter.ToString();
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
                            item.inicio = Convert.ToDateTime(reader["fec_registro"].ToString());
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
        public void getTicketRellamada(int idticketera)
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
                string sql = "update TKT_Ticketera set fec_cancela=getdate(),usu_cancela='" + session.codigousuario + "',estado=4 where id_Ticket='" + idticketera + "'";
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
        public void getTicketTerminado(int idticketera, MySession session, bool isCaja)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                TKT_Ticketera item = new TKT_Ticketera();
                string sql = "";
                if (isCaja)
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

        public string getCantidadPacientes(int empresa, int sede)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                TKT_Ticketera item = new TKT_Ticketera();
                string sql = "select count(*) cantidad from TKT_Ticketera t  where t.idempresa=" + empresa + " and t.idSede=" + sede + "  and t.isEliminado=0 and id_counter is null and t.estado<4";
                string resultado = "";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            resultado = (reader["cantidad"].ToString());
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
