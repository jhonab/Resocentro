using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    class EstudiosDAO
    {
        public List<CLASE> getClase()
        {
            List<CLASE> lista = new List<CLASE>();
            string queryString = @"select* from CLASE";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new CLASE();
                            item.codigoclase = Convert.ToInt32((reader["codigoclase"]).ToString());
                            item.nombreclase = ((reader["nombreclase"]).ToString());

                            lista.Add(item);
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
        public List<Search_Estudio> getListEstudio(string clase, string ampliacion, string modalidad, string sucursal)
        {
            List<Search_Estudio> lista = new List<Search_Estudio>();
            string queryString = @"SELECT isnull(es.indicacion,'')indicacion,es.codigoestudio ,es.nombreestudio,es.codigoclase  FROM ESTUDIO es INNER JOIN EQUIPO_ESTUDIO eq ON es.codigoestudio=eq.codigoestudio WHERE(eq.bloqueado=0 AND es.codigoclase='{0}'AND SUBSTRING(es.codigoestudio,6,1)='{1}' AND SUBSTRING(es.codigoestudio,5,1)='{2}' AND SUBSTRING(es.codigoestudio,1,3)='{3}')AND SUBSTRING (es.codigoestudio,8,2)<>'99' GROUP BY es.codigoestudio,es.nombreestudio,es.codigoclase,es.indicacion ORDER BY es.nombreestudio;";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, clase, ampliacion, modalidad, sucursal), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new Search_Estudio();

                            item.codigoestudio = ((reader["codigoestudio"]).ToString());
                            item.estudio = ((reader["nombreestudio"]).ToString());
                            item.codigoclase = Convert.ToInt32((reader["codigoclase"]).ToString());
                            item.indicaciones = ((reader["indicacion"]).ToString());

                            lista.Add(item);
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

        public List<Search_Estudio> getListInsumoTecnica(string filtro,string ampliacion, string modalidad, string sucursal)
        {
            List<Search_Estudio> lista = new List<Search_Estudio>();
            string queryString = @"SELECT (case when SUBSTRING(es.codigoestudio,8,2)='99'and SUBSTRING(es.codigoestudio,7,1)!='9' then 'TECNICA' when SUBSTRING(es.codigoestudio,8,2)!='99'and SUBSTRING(es.codigoestudio,7,1)='9' then 'INSUMO' else 'ESTUDIO' end )indicacion,es.codigoestudio ,es.nombreestudio,es.codigoclase  FROM ESTUDIO es INNER JOIN EQUIPO_ESTUDIO eq ON es.codigoestudio=eq.codigoestudio WHERE eq.bloqueado=0 AND  SUBSTRING(es.codigoestudio,6,1)='{0}' AND SUBSTRING(es.codigoestudio,5,1)='{1}' AND SUBSTRING(es.codigoestudio,1,3)='{2}' and es.nombreestudio like'%{3}%' GROUP BY es.codigoestudio,es.nombreestudio,es.codigoclase,es.indicacion ORDER BY es.nombreestudio;";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString,  ampliacion, modalidad, sucursal,filtro), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new Search_Estudio();

                            item.codigoestudio = ((reader["codigoestudio"]).ToString());
                            item.estudio = ((reader["nombreestudio"]).ToString());
                            item.codigoclase = Convert.ToInt32((reader["codigoclase"]).ToString());
                            item.indicaciones = ((reader["indicacion"]).ToString());

                            lista.Add(item);
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

        public ESTUDIO getEstudio(string codigoestudio)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.ESTUDIO.SingleOrDefault(x => x.codigoestudio == codigoestudio);
            }
        }
        public Search_Estudio getPrecioEstudio(string idestudio, int idaseguradora)
        {
            Search_Estudio item = new Search_Estudio();
            string queryString = @"select preciobruto,codigomoneda from ESTUDIO_COMPANIA where codigoestudio='{0}' and codigocompaniaseguro='{1}';";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, idestudio, idaseguradora), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                item.precio = Convert.ToDouble((reader["preciobruto"]).ToString());
                                item.idmoneda = Convert.ToInt32((reader["codigomoneda"]).ToString());

                            }
                        else
                            item = null;

                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return item;
        }

        public List<Turno_Citar> getTurnoCita(string empresa, string sucursal, string modalidad, int dia, int mes, int anio)
        {
            List<Turno_Citar> lista = new List<Turno_Citar>();
            string queryString = @"EXECUTE [dbo].[listarturnoparacita] '{0}','{1}','{2}','{3}','{4}','{5}';";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, empresa, sucursal, modalidad, dia, mes, anio), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                var item = new Turno_Citar();
                                item.hora = Convert.ToDateTime((reader["hora"]).ToString());
                                item.equipo = ((reader["equipo biomedico"]).ToString());
                                item.turno = ((reader["med.turno"]).ToString());
                                item.idHorario = Convert.ToInt32((reader["cod"]).ToString());
                                item.codigoequipo = Convert.ToInt32((reader["ce"]).ToString());
                                lista.Add(item);
                            }
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }
    }
}

public class Search_Estudio
{
    public string codigoestudio { get; set; }
    public string estudio { get; set; }
    public int codigoclase { get; set; }
    public int idmoneda { get; set; }
    public double precio { get; set; }
    public string moneda { get { return idmoneda == 1 ? "Soles" : idmoneda == 2 ? "Dolares" : ""; } }
    public string simbolomoneda { get { return idmoneda == 1 ? "S/." : idmoneda == 2 ? "$" : ""; } }
    public double cobertura { get; set; }
    public double descuento { get; set; }
    public double precioneto { get { return precio + descuento; } }
    public string indicaciones { get; set; }
    public DateTime fechareserva { get; set; }
    public Turno_Citar turno { get; set; }

    public bool isEditable { get; set; }
}

public class Turno_Citar
{
    public DateTime hora { get; set; }
    public string equipo { get; set; }
    public string turno { get; set; }
    public int idHorario { get; set; }
    public int codigoequipo { get; set; }
}
