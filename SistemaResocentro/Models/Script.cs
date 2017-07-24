using SistemaResocentro.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SistemaResocentro.Models
{
    public class Script_Sql
    {
        public List<ListaSeguimientoViewModels> SeguimientoProduccion(string empresa, string estados, string inicio, string fin)
        {
            string queryString = @"
declare @empresa varchar(5)
declare @inicio varchar(50)
declare @fin varchar(50)

set @empresa='{0}'
set @inicio='{1}'
set @fin='{2}'
select * ,
ISNULL(dbo.ufngetDocumentoatencion(@empresa,a.codigopaciente,a.numeroatencion,a.codigoestudio),'ND') documento from(
select REPLACE(REPLACE(REPLACE(c.observacion,CHAR(10),' ') ,CHAR(13),' ') ,'  ',' ') observacion, ch.razonsocial clinica,ISNULL(cg.codigocartagarantia,'') codigocarta,p.codigopaciente,ea.numeroatencion,ea.codigoestudio,case when cg.codigodocadjunto is null then 0 when cg.codigodocadjunto='' then 0 else 1 end carta,case when esc.nombrearchivo is not null then 1 else 0 end adj ,a.fechayhora fecha,ea.codigo examen,ea.estadoestudio estadoA,su.ShortDesc sucursal,case when cs.descripcion like '%particular%' then 'Particular'when cs.descripcion like '%essalu%' then 'EsSalud' when cs.descripcion like '%protoco%' then 'Protocolo' else 'Asegurado' end tpaciente,case when p.tipo_doc='0' then 'DNI' when p.tipo_doc='1'then 'Carnet Extranjeria' when p.tipo_doc='2' then 'RUC' when p.tipo_doc='3' then 'Pasaporte' when p.tipo_doc='4' then 'Cedula Diplomatica' else 'Otros' end tdocumento,p.dni,p.apellidos+' '+p.nombres paciente,p.sexo,p.fechanace nacimiento,p.IsProtocolo,a.edad,a.peso,a.talla,cs.descripcion Aseguradora,me.apellidos+' '+me.nombres medico, isnull(cg.cobertura,'0') cobertura,es.nombreestudio estudio from EXAMENXATENCION ea 
inner join ATENCION a on ea.numeroatencion = a.numeroatencion
inner join ESTUDIO es on ea.codigoestudio = es.codigoestudio
inner join PACIENTE p on ea.codigopaciente=p.codigopaciente
inner join UNIDADNEGOCIO un on substring(ea.codigoestudio,1,1) =un.codigounidad
inner join COMPANIASEGURO cs on ea.codigocompaniaseguro=cs.codigocompaniaseguro
inner join MEDICOEXTERNO me on a.cmp=me.cmp
inner join SUCURSAL su on un.codigounidad=su.codigounidad and substring(ea.codigoestudio,3,1)=su.codigosucursal
inner join CITA c on ea.numerocita=c.numerocita
inner join CLINICAHOSPITAL ch on c.codigoclinica =ch.codigoclinica
inner join EXAMENXCITA ec on ea.numerocita=ec.numerocita and ea.codigoestudio=ec.codigoestudio
left join CARTAGARANTIA cg on c.codigocartagarantia=cg.codigocartagarantia
left join ESCANADMISION esc on esc.numerodeatencion=ea.numeroatencion
where ea.codigoestudio like @empresa+'%'
and a.fechayhora >@inicio+' 00:00:01' and @fin+' 23:59:59'>a.fechayhora
and ea.estadoestudio in ({3})
and ec.estadoestudio!='X') as A
--where a.examen=534339
order by a.fecha

";

            List<ListaSeguimientoViewModels> lista = new List<ListaSeguimientoViewModels>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, empresa, inicio, fin, estados), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var carta = Convert.ToBoolean((reader["carta"]));
                            var doc = Convert.ToBoolean((reader["adj"]));
                            var item = new ListaSeguimientoViewModels();

                            item.adjunto = doc;
                            item.carta = carta;
                            item.fecha = Convert.ToDateTime((reader["fecha"]).ToString());
                            item.nexamen = Convert.ToInt32((reader["examen"]).ToString());
                            item.estado = ((reader["estadoA"]).ToString());
                            item.clinica = ((reader["clinica"]).ToString());
                            item.sucursal = ((reader["sucursal"]).ToString());
                            item.tipo_paciente = ((reader["tpaciente"]).ToString());
                            item.tipo_documento = ((reader["tdocumento"]).ToString());
                            item.ndocumento = ((reader["dni"]).ToString());
                            item.paciente = ((reader["paciente"]).ToString());
                            item.sexo = ((reader["sexo"]).ToString());
                            item.nacimiento = Convert.ToDateTime((reader["nacimiento"]).ToString());
                            item.isProtocolo = Convert.ToBoolean((reader["IsProtocolo"]).ToString());
                            item.edad = Convert.ToInt32((reader["edad"]).ToString());
                            item.peso = Convert.ToDouble((reader["peso"]).ToString());
                            item.talla = Convert.ToDouble((reader["talla"]).ToString());
                            item.aseguradora = ((reader["Aseguradora"]).ToString());
                            item.medico = ((reader["medico"]).ToString());
                            item.cobertura = Convert.ToDouble((reader["cobertura"]).ToString());
                            item.estudio = ((reader["estudio"]).ToString());
                            item.documentos = ((reader["documento"]).ToString());
                            item.observacion = ((reader["observacion"]).ToString());
                            item.codigocarta = ((reader["codigocarta"]).ToString());
                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }
            return lista;
        }

        public List<Sedaciones_ProgramadasViewModel> getSedaciones(string fecha)
        {
            string queryString = "select isnull((select isnull(us.shortname,'')from USUARIO us where us.codigousuario=ec.sedador),'')sedador,convert(varchar(20),c.fechareserva,103)+' '+ convert(varchar(20),ec.horacita,108)fecha,ec.codigoexamencita numerocita,p.apellidos+', '+p.nombres paciente,es.nombreestudio estudio,year(getdate())-year(p.fechanace) edad,(select min(e.nombreequipo) from equipo e where ec.codigoequipo=e.codigoequipo )equipo,cs.descripcion aseguradora, ch.razonsocial clinica from CITA c inner join CLINICAHOSPITAL ch on c.codigoclinica=ch.codigoclinica inner join COMPANIASEGURO cs on c.codigocompaniaseguro=cs.codigocompaniaseguro inner join EXAMENXCITA ec on c.numerocita=ec.numerocita inner join paciente p on c.codigopaciente = p.codigopaciente inner join ESTUDIO es on ec.codigoestudio=es.codigoestudio where convert(date,fechareserva)='{0}' and c.sedacion=1 and ec.estadoestudio!='X' and SUBSTRING(ec.codigoestudio,7,1)!='9' and SUBSTRING(ec.codigoestudio,8,2)!='99'";
            List<Sedaciones_ProgramadasViewModel> lista = new List<Sedaciones_ProgramadasViewModel>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, fecha), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new Sedaciones_ProgramadasViewModel();
                            item.fecha = Convert.ToDateTime((reader["fecha"]).ToString());
                            item.numerocita = ((reader["numerocita"]).ToString());
                            item.sedador = ((reader["sedador"]).ToString());
                            item.paciente = ((reader["paciente"]).ToString());
                            item.estudio = ((reader["estudio"]).ToString());
                            item.edad = ((reader["edad"]).ToString());
                            item.equipo = ((reader["equipo"]).ToString());
                            item.aseguradora = ((reader["aseguradora"]).ToString());
                            item.clinica = ((reader["clinica"]).ToString());
                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }
            return lista;
        }

        public List<ListaReporteAtencionesViewModel> ReporteAtenciones(string sede, string inicio, string fin)
        {
            List<ListaReporteAtencionesViewModel> lista = new List<ListaReporteAtencionesViewModel>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    string queryString = "exec sp_getReporteAtencionalCliente '{1}','{2}','{0}'";

                    SqlCommand command = new SqlCommand(string.Format(queryString, sede, inicio, fin), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new ListaReporteAtencionesViewModel();

                            item.fecha = Convert.ToDateTime((reader["fecha"]).ToString());
                            item.examen = Convert.ToInt32((reader["codigo"]).ToString());
                            item.estado = ((reader["estado"]).ToString());
                            item.aseguradora = ((reader["aseguradora"]).ToString());
                            item.apellidos = ((reader["apellidos"]).ToString());
                            item.nombres = ((reader["nombres"]).ToString());
                            item.estudio = ((reader["estudio"]).ToString());
                            item.sedacion = Convert.ToBoolean((reader["sedacion"]).ToString());
                            item.contraste = Convert.ToBoolean((reader["contraste"]).ToString());
                            item.documentos = ((reader["documentos"]).ToString());
                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }
            return lista;
        }
    }
}
