using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    public class PagosDAO
    {
        public List<SedacionesAplicadas> getListSedaciones(int mes, int ano)
        {
            List<SedacionesAplicadas> lista = new List<SedacionesAplicadas>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    string sql = @"
declare @mes int
declare @ano int

set @mes='{0}'
set @ano='{1}'

select A.*,
case when(a.cant>=2 and a.anestesiologo not like '%Milagritos%') then 1+((convert(int,a.cant)-1)*0.5) when( a.anestesiologo  like '%Milagritos%') then (convert(int,cant)) else 1 end 
valor
,isnull(case 
when(a.cia like '%Particular%') then (select com.com_particulares from Com_Medico com where com.codigousuario=a.usuario)
when(a.cia like '%Essalud%') then (
	case 
		when (a.cia like '%Sabogal%')then (select com.com_Sabogal from Com_Medico com where com.codigousuario=a.usuario)
		when (a.cia like '%Almenara%')then (select com.com_almenara from Com_Medico com where com.codigousuario=a.usuario) 
		else 0 end) 
else (select com.com_asegurado from Com_Medico com where com.codigousuario=a.usuario) end,'0')  comision

from (SELECT a.fechayhora fecha,s.atencion, u.ShortName anestesiologo,
 p.codigopaciente,p.apellidos+' '+p.nombres nombre,css.descripcion cia,
 case when(css.descripcion like '%Particular%') then 'Particular' when(css.descripcion like '%Essalud%') then case when(css.descripcion like '%Sabogal%') then 'Sabogal'when(css.descripcion like '%Almenara%') then 'Almenara' else '' end else 'Asegurado' end tipo,
 (select count(ea.numeroatencion) from EXAMENXATENCION ea where ea.numeroatencion=s.atencion ) cant
 ,s.usuario
from Sedacion s 
inner join ATENCION a on s.atencion=a.numeroatencion
inner join PACIENTE p on s.paciente=p.codigopaciente
inner join COMPANIASEGURO css on a.codigocompaniaseguro=css.codigocompaniaseguro
inner join USUARIO u on s.usuario=u.codigousuario

WHERE   MONTH(a.fechayhora)=@mes
AND YEAR(a.fechayhora)=@ano
AND s.estado='1') as A

group by a.fecha,a.atencion,a.anestesiologo,a.codigopaciente,a.nombre,a.cia,a.tipo,a.cant,usuario
";
                    SqlCommand command = new SqlCommand(string.Format(sql, mes, ano), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new SedacionesAplicadas();
                            item.fecha = Convert.ToDateTime((reader["fecha"]).ToString());
                            item.anestesiologo = (reader["anestesiologo"]).ToString();
                            item.codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString());
                            item.paciente = (reader["nombre"]).ToString();
                            item.aseguradora = (reader["cia"]).ToString();
                            item.tipo = (reader["tipo"]).ToString();
                            item.cantidad = Convert.ToDouble((reader["valor"]).ToString());
                            item.valor = Convert.ToDouble((reader["comision"]).ToString());
                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return lista;
            }
        }
        public List<MedicosEncuestaSupervision> getListMedicos(int mes, int ano)
        {
            List<MedicosEncuestaSupervision> lista = new List<MedicosEncuestaSupervision>();
            using (RepositorioJhonEntities1 db = new RepositorioJhonEntities1())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    string fi = "", ff = "";
                    DateTime f = new DateTime(ano, mes, 25);
                    if (f <= new DateTime(2015, 10, 26))//cambio de fecha por contabilidad
                    {
                        if (mes == 1) {

                            ff = "25/" + (f.Month).ToString() + "/" + f.Year.ToString();
                            f = f.AddMonths(-1);
                            fi = "26/" + (f.Month).ToString() + "/" + f.Year.ToString();
                        }
                        else
                        {
                            fi = "26/" + (f.Month - 1).ToString() + "/" + f.Year.ToString();
                            ff = "25/" + (f.Month).ToString() + "/" + f.Year.ToString();
                        }
                    }
                    else
                    {
                        if (mes != 1 && mes != 12)
                        {
                            fi = "26/" + (f.Month - 1).ToString() + "/" + f.Year.ToString();
                            ff = "25/" + (f.Month).ToString() + "/" + f.Year.ToString();
                        }
                        else
                        {
                            if (mes == 12)
                            {
                                fi = "26/" + (f.AddMonths(-1).Month ).ToString() + "/" + f.Year.ToString();
                                ff = "25/" + (f.Month).ToString() + "/" + f.Year.ToString();
                            }
                            else
                            {
                                fi = "26/" + (f.AddMonths(-1).Month).ToString() + "/" + f.AddYears(-1).Year.ToString();
                                ff = "25/" + (f.Month).ToString() + "/" + f.Year.ToString();
                            }
                        }
                    }
                    MessageBoxTemporal.Show(fi + " al " + ff, "Rango de fechas son:", 10, true);
                    try
                    {
                        string sqlencuestador = "select * from ReporteContabilidad where  convert (date,fechayhora)  BETWEEN  '{0}' AND '{1}' order by fechayhora";
                        //string sqlsupervisor = "select * from ReporteContabilidad where  convert (date,fechayhora)  BETWEEN  '26-10-2014' AND '25-11-2014' order by encuestador";
                        SqlCommand command = new SqlCommand(string.Format(sqlencuestador, fi, ff), connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader["encuestador"] != DBNull.Value)
                            {
                                var item = new MedicosEncuestaSupervision();
                                item.codigo = (reader["encuestador"]).ToString();
                                item.paciente = ((reader["paciente"]).ToString());
                                item.fecha = Convert.ToDateTime((reader["fechayhora"]).ToString());
                                item.modalidad = (reader["modalidad"]).ToString();
                                item.estudio = ((reader["nombreestudio"]).ToString());
                                item.encuesta = ((reader["encuestador"]).ToString());
                                item.supervision = ((reader["supervisor"]).ToString());
                                lista.Add(item);
                            }
                        }

                        /*command = new SqlCommand(string.Format(sqlsupervisor, mes, ano, mesanterior, anoanterior), connection);
                        reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader["supervisor"] != DBNull.Value)
                            {
                                var nombre = reader["supervisor"].ToString();
                                var modalidad = reader["modalidad"].ToString();
                                var medico = lista.SingleOrDefault(x => x.medico == nombre && x.modalidad == modalidad);
                                if (medico != null)
                                {
                                    medico.supervision = Convert.ToInt32((reader["cant."]).ToString());
                                }
                                else
                                {
                                    var item = new MedicosEncuestaSupervision();
                                    item.medico = (reader["supervisor"]).ToString();
                                    item.encuesta = 0;
                                    item.supervision = Convert.ToInt32((reader["cant."]).ToString());
                                    item.modalidad = (reader["modalidad"]).ToString();
                                    lista.Add(item);
                                }
                            }
                        }*/
                    }

                    finally
                    {
                        connection.Close();
                    }
                }
                return lista;
            }
        }
        public List<MedicosEncuestaSupervision> getListTecnologo(int mes, int ano)
        {
            List<MedicosEncuestaSupervision> lista = new List<MedicosEncuestaSupervision>();
            using (RepositorioJhonEntities1 db = new RepositorioJhonEntities1())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    string fi = "", ff = "";
                    DateTime f = new DateTime(ano, mes, 25);
                    fi = "1/" + f.Month.ToString() + "/" + f.Year.ToString();
                    ff = DateTime.DaysInMonth(ano, mes).ToString() + "/" + f.Month.ToString() + "/" + f.Year.ToString();
                    MessageBoxTemporal.Show(fi + " al " + ff, "Rango de fechas son:", 10, true);
                    try
                    {
                        string sqlencuestador = "select * from ReporteContabilidad where  convert (date,fechayhora)  BETWEEN  '{0}' AND '{1}' order by fechayhora";
                        //string sqlsupervisor = "select * from ReporteContabilidad where  convert (date,fechayhora)  BETWEEN  '26-10-2014' AND '25-11-2014' order by encuestador";
                        SqlCommand command = new SqlCommand(string.Format(sqlencuestador, fi, ff), connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader["encuestador"] != DBNull.Value)
                            {
                                var item = new MedicosEncuestaSupervision();
                                item.codigo = (reader["codigo"]).ToString();
                                item.paciente = ((reader["paciente"]).ToString());
                                item.equipo = ((reader["equipo"]).ToString());
                                item.fecha = Convert.ToDateTime((reader["fechayhora"]).ToString());
                                item.tipoestudio = ((reader["tipoEstudioTecnologo"]).ToString());
                                item.modalidad = (reader["modalidad"]).ToString();
                                item.estudio = ((reader["nombreestudio"]).ToString());
                                item.encuesta = ((reader["encuestador"]).ToString());
                                item.supervision = ((reader["supervisor"]).ToString());
                                item.tecnologo = ((reader["tecnologo"]).ToString());
                                item.valor = Convert.ToDouble((reader["valor_tecnologo"]).ToString());
                                lista.Add(item);
                            }
                        }

                        /*command = new SqlCommand(string.Format(sqlsupervisor, mes, ano, mesanterior, anoanterior), connection);
                        reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader["supervisor"] != DBNull.Value)
                            {
                                var nombre = reader["supervisor"].ToString();
                                var modalidad = reader["modalidad"].ToString();
                                var medico = lista.SingleOrDefault(x => x.medico == nombre && x.modalidad == modalidad);
                                if (medico != null)
                                {
                                    medico.supervision = Convert.ToInt32((reader["cant."]).ToString());
                                }
                                else
                                {
                                    var item = new MedicosEncuestaSupervision();
                                    item.medico = (reader["supervisor"]).ToString();
                                    item.encuesta = 0;
                                    item.supervision = Convert.ToInt32((reader["cant."]).ToString());
                                    item.modalidad = (reader["modalidad"]).ToString();
                                    lista.Add(item);
                                }
                            }
                        }*/
                    }

                    finally
                    {
                        connection.Close();
                    }
                }
                return lista;
            }
        }
        public List<PostProcesadorTecnica> getListPostProcesador(int mes, int ano)
        {
            List<PostProcesadorTecnica> lista = new List<PostProcesadorTecnica>();
            using (RepositorioJhonEntities1 db = new RepositorioJhonEntities1())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    string fi = "", ff = "";
                    DateTime f = new DateTime(ano, mes, 25);
                    fi = "1/" + f.Month.ToString() + "/" + f.Year.ToString();
                    ff = DateTime.DaysInMonth(ano, mes).ToString() + "/" + f.Month.ToString() + "/" + f.Year.ToString();
                    MessageBoxTemporal.Show(fi + " al " + ff, "Rango de fechas son:", 10, true);
                    try
                    {
                        string sql = @"select 
(case when substring(codigoestudio,1,1)='1' then 
'RES'+(case 
when convert(int,substring(codigoestudio,2,2))=1 then ' - Central' 
when convert(int,substring(codigoestudio,2,2))=3 then ' - Golf' 
when convert(int,substring(codigoestudio,2,2))=4 then ' - San Borja' 
when convert(int,substring(codigoestudio,2,2))=5 then ' - Benavides' 
when convert(int,substring(codigoestudio,2,2))=6 then ' - Piura' 
when convert(int,substring(codigoestudio,2,2))=7 then ' - Polaris' 
when convert(int,substring(codigoestudio,2,2))=8 then ' - San Judas' 
when convert(int,substring(codigoestudio,2,2))=9 then ' - Javier Prado'
else'' end
)
else 'EME' end) sede,
convert(varchar(20),fecha,103) fecha,
codigo,
tecnologo,
tecnica,
valor
from ReporteContabilidad_PostProceso where  convert (date,fecha)  BETWEEN  '{0}' AND '{1}' order by fecha";

                        SqlCommand command = new SqlCommand(string.Format(sql, fi, ff), connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader["tecnologo"] != DBNull.Value)
                            {
                                var item = new PostProcesadorTecnica();
                                item.codigo = (reader["codigo"]).ToString();
                                item.tecnologo = (reader["tecnologo"]).ToString();
                                item.tecnica = (reader["tecnica"]).ToString();
                                item.cantidad = Convert.ToDouble((reader["valor"]).ToString());
                                item.sede = ((reader["sede"]).ToString());
                                item.fecha = ((reader["fecha"]).ToString());
                                lista.Add(item);
                            }
                        }
                    }

                    finally
                    {
                        connection.Close();
                    }
                }
                return lista;
            }
        }
    }
}
public class SedacionesAplicadas
{

    public string anestesiologo { get; set; }
    public int codigopaciente { get; set; }
    public string paciente { get; set; }
    public string aseguradora { get; set; }
    public string tipo { get; set; }
    public double cantidad { get; set; }
    public double valor { get; set; }
    public double comision { get { return cantidad * valor; } }

    public DateTime fecha { get; set; }
}

public class MedicosEncuestaSupervision
{
    public string codigo { get; set; }
    public string paciente { get; set; }
    public string estudio { get; set; }
    public string modalidad { get; set; }
    public string encuesta { get; set; }
    public string supervision { get; set; }

    public string tecnologo { get; set; }

    public double valor { get; set; }

    public string equipo { get; set; }

    public string tipoestudio { get; set; }

    public DateTime fecha { get; set; }
}

public class PostProcesadorTecnica
{

    public double cantidad { get; set; }

    public string tecnologo { get; set; }

    public string codigo { get; set; }

    public string tecnica { get; set; }

    public string sede { get; set; }

    public string fecha { get; set; }
}