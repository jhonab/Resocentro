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
                return db.USUARIO.Where(x => x.bloqueado == false).OrderBy(x => x.EMPLEADO.apellidos).Select(x => new Colaboradores { codigo = x.codigousuario, valor = x.EMPLEADO.apellidos + " " + x.EMPLEADO.nombres }).ToList();
            }
        }

        public List<ConfirmacionLecturaPago> getLogCnfirmacionPago(int mes, int year)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.ConfirmacionLecturaPago.Where(x => x.fecha.Month == mes && x.fecha.Year == year).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo">
        /// 1 reporte mensual,
        /// 2 reporte por rango de fecha
        /// </param>
        /// <param name="mes">reporte mensual MES</param>
        /// <param name="anio">reporte mensual AÑO</param>
        /// <param name="empresa">Empresa</param>
        /// <param name="inicio">Reporte por rango Fecha de Inicio</param>
        /// <param name="fin">Reporte por rango Fecha de Fin</param>
        /// <returns></returns>
        public async Task<List<ReporteAtencionesMensuales>> getReportAtenciones(int tipo, int mes, int anio, int empresa, DateTime inicio, DateTime fin)
        {

            List<ReporteAtencionesMensuales> lista = new List<ReporteAtencionesMensuales>();

            string queryString = "dbo.get_VerificacionPagoAtencionesxMes";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@tipo", SqlDbType.Int).Value = tipo;
                    command.Parameters.Add("@mes", SqlDbType.VarChar).Value = mes.ToString();
                    command.Parameters.Add("@anio", SqlDbType.VarChar).Value = anio.ToString();
                    command.Parameters.Add("@empresa", SqlDbType.VarChar).Value = empresa.ToString();
                    command.Parameters.Add("@f1", SqlDbType.VarChar).Value = inicio.ToShortDateString();
                    command.Parameters.Add("@f2", SqlDbType.VarChar).Value = fin.ToShortDateString();

                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();
                    command.Prepare();

                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        // using (var reader =  command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (await reader.ReadAsync())
                                {
                                    ReporteAtencionesMensuales item = new ReporteAtencionesMensuales();
                                    item.Sede = (reader["sede"].ToString());
                                    item.Estadoestudio = (reader["estadoestudio"].ToString());
                                    item.Fecha = Convert.ToDateTime(reader["fechayhora"].ToString());
                                    item.Codigo = Convert.ToInt32(reader["codigo"].ToString());
                                    item.Paciente = (reader["paciente"].ToString());
                                    item.Codigopaciente = Convert.ToInt32(reader["codigopaciente"].ToString());
                                    item.Numerocita = Convert.ToInt32(reader["numerocita"].ToString());
                                    item.Numeroatencion = Convert.ToInt32(reader["numeroatencion"].ToString());
                                    item.Tipopaciente = (reader["tipo"].ToString());
                                    item.Estudio = (reader["estudio"].ToString());
                                    item.Codigoestudio = (reader["codigoestudio"].ToString());
                                    item.Aseguradora = (reader["Procedencia"].ToString());
                                    item.Cobertura = Convert.ToDouble(reader["cobertura"].ToString());
                                    item.Documentos = (reader["Facturas"].ToString());
                                    item.Unidad = Convert.ToInt32(reader["unidad"].ToString());
                                    switch (Convert.ToInt32(reader["codigoestudio"].ToString().Substring(3, 2)))
                                    {
                                        case 1:
                                            item.Modalidad = "RM";
                                            break;
                                        case 2:
                                            item.Modalidad = "TEM";
                                            break;
                                        case 3:
                                            item.Modalidad = "RX";
                                            break;
                                        case 4:
                                            item.Modalidad = "MG";
                                            break;
                                        case 5:
                                            item.Modalidad = "US";
                                            break;
                                        case 6:
                                            item.Modalidad = "DEN";
                                            break;
                                        case 7:
                                            item.Modalidad = "TAC";
                                            break;
                                        case 8:
                                            item.Modalidad = "TACH";
                                            break;
                                        case 9:
                                            item.Modalidad = "PETCT";
                                            break;
                                        case 10:
                                            item.Modalidad = "GAM";
                                            break;
                                        case 11:
                                            item.Modalidad = "TACR";
                                            break;
                                        case 12:
                                            item.Modalidad = "TEMR";
                                            break;
                                        case 13:
                                            item.Modalidad = "RXD";
                                            break;
                                        case 14:
                                            item.Modalidad = "TEMD";
                                            break;
                                        case 15:
                                            item.Modalidad = "FOT";
                                            break;
                                        case 16:
                                            item.Modalidad = "FIB";
                                            break;
                                        default:
                                            item.Modalidad = "OTR";
                                            break;
                                    }
                                    switch (Convert.ToInt32(reader["codigoestudio"].ToString().Substring(6, 1)))
                                    {
                                        case 0:
                                            item.Clase = "OPE. VAR.";
                                            break;
                                        case 1:
                                            item.Clase = "CRANEO";
                                            break;
                                        case 2:
                                            item.Clase = "COLUMNA";
                                            break;
                                        case 3:
                                            item.Clase = "M.M.S.S.";
                                            break;
                                        case 4:
                                            item.Clase = "M.M.I.I.";
                                            break;
                                        case 5:
                                            item.Clase = "CUERPO";
                                            break;
                                        case 6:
                                            item.Clase = "CORAZÓN";
                                            break;
                                        case 7:
                                            item.Clase = "ARTRO";
                                            break;
                                        case 8:
                                            item.Clase = "ANGIO";
                                            break;
                                        case 9:
                                            item.Clase = "INSUMOS";
                                            break;
                                        default:
                                            item.Clase = "OTR";
                                            break;
                                    }

                                    if (item.Documentos == "")
                                        item.Comentarios = "SIN DOCUMENTOS";
                                    else if (item.Documentos.Split('#').Count() < 3 && item.Cobertura > 0 && item.Cobertura < 100)
                                        item.Comentarios = "SOLO TIENE 1 DOCUMENTO";
                                    else
                                        item.Comentarios = "OK";
                                    lista.Add(item);
                                }
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

        public async Task<List<ReporteDocumentosMensuales>> getReportDocumentos(int empresa, DateTime fi, DateTime ff)
        {

            List<ReporteDocumentosMensuales> lista = new List<ReporteDocumentosMensuales>();
            string queryString = "dbo.get_documentosFormaPago";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@empresa", SqlDbType.Int).Value = empresa;
                    command.Parameters.Add("@fi", SqlDbType.VarChar).Value = fi.ToShortDateString();
                    command.Parameters.Add("@ff", SqlDbType.VarChar).Value = ff.ToShortDateString();
                    command.CommandTimeout = 120;
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    try
                    {
                        /*
                        SqlDataAdapter da = new SqlDataAdapter();
                        DataSet dt = new DataSet();
                        da.SelectCommand = command;
                        da.Fill(dt, "ReporteDocumentosMensuales");
                        */
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            ReporteDocumentosMensuales item = new ReporteDocumentosMensuales();
                            while (await reader.ReadAsync())
                            //foreach (DataRow reader in dt.Tables[0].Rows)
                            {

                                if (item.documento != (reader["numerodocumento"].ToString()))
                                {
                                    if (item.documento != null)
                                    {
                                        item.pagos = item.listapagos.Where(x => x.tarjeta.ToLower().Contains("pendiente") && x.tarjeta.ToLower().Contains("pago")).Count();
                                        item.montopagos = item.listapagos.Where(x => !x.tarjeta.ToLower().Contains("pendiente") && !x.tarjeta.ToLower().Contains("pago")).Sum(x => x.montopago);
                                        item.saldo = Math.Round(item.total - item.montopagos, 2);
                                        lista.Add(item);
                                    }
                                    item = new ReporteDocumentosMensuales();
                                    item.listapagos = new List<PagosDocumento>();
                                }

                                item.fechadoc = Convert.ToDateTime(reader["fechaemitio"].ToString());
                                item.documento = (reader["numerodocumento"].ToString());
                                item.sede = (reader["sucursal"].ToString());
                                item.tipodocumento = (reader["tipodocumento"].ToString());
                                item.codigomonedadoc = (reader["monedadoc"].ToString());
                                item.monedadoc = (reader["moneda"].ToString());
                                item.total = Convert.ToDouble(reader["total"].ToString());
                                item.tipocambio = Convert.ToDouble(reader["tipocambio"].ToString());
                                item.usuariodoc = (reader["usuariodoc"].ToString());
                                item.razonsocial = (reader["razonsocialalterno"].ToString());
                                item.estado = (reader["estado"].ToString());
                                item.pdf = (reader["pdf"].ToString());
                                item.serie = (reader["serie"].ToString());
                                item.paciente = (reader["paciente"].ToString());


                                item.listapagos.Add(new PagosDocumento
                                {
                                    fechapago = Convert.ToDateTime(reader["fechadepago"].ToString()),
                                    codigomonedapago = (reader["monedapago"].ToString()),
                                    tarjeta = (reader["tarjeta"].ToString()),
                                    monedapago = (reader["pagomoneda"].ToString()),
                                    montopago = Convert.ToDouble(reader["pagomonto"].ToString()),
                                    usuariopago = (reader["usuariopago"].ToString())
                                });
                            }
                            if (item.documento != null)
                            {
                                item.pagos = item.listapagos.Where(x => x.tarjeta.ToLower().Contains("pendiente") && x.tarjeta.ToLower().Contains("pago")).Count();
                                item.montopagos = item.listapagos.Where(x => !x.tarjeta.ToLower().Contains("pendiente") && !x.tarjeta.ToLower().Contains("pago")).Sum(x => x.montopago);
                                item.saldo = Math.Round(item.total - item.montopagos, 2);
                                lista.Add(item);
                            }

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

        public async Task<List<ReporteDocumentosMensuales>> getReportDocumentosFactura(int empresa, DateTime fi, DateTime ff)
        {
            List<ReporteDocumentosMensuales> lista = new List<ReporteDocumentosMensuales>();
            string queryString = "dbo.get_CobranzaPendiente";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@empresa", SqlDbType.Int).Value = empresa;
                    command.Parameters.Add("@fi", SqlDbType.VarChar).Value = fi.ToShortDateString();
                    command.Parameters.Add("@ff", SqlDbType.VarChar).Value = ff.ToShortDateString();
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    try
                    {
                        /*
                        SqlDataAdapter da = new SqlDataAdapter();
                        DataSet dt = new DataSet();
                        da.SelectCommand = command;
                        da.Fill(dt, "ReporteDocumentosMensuales");
                        */
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                //foreach (DataRow reader in dt.Tables[0].Rows)
                                {
                                    ReporteDocumentosMensuales item = new ReporteDocumentosMensuales();

                                    item.fechadoc = Convert.ToDateTime(reader["fechaemitio"].ToString());
                                    item.documento = (reader["numerodocumento"].ToString());
                                    item.sede = (reader["sucursal"].ToString());
                                    item.tipodocumento = (reader["tipodocumento"].ToString());
                                    item.codigomonedadoc = (reader["monedadoc"].ToString());
                                    item.monedadoc = (reader["moneda"].ToString());
                                    item.total = Convert.ToDouble(reader["total"].ToString());
                                    item.tipocambio = Convert.ToDouble(reader["tipocambio"].ToString());
                                    item.usuariodoc = (reader["usuariodoc"].ToString());
                                    item.razonsocial = (reader["razonsocialalterno"].ToString());
                                    item.estado = (reader["estado"].ToString());
                                    item.serie = (reader["serie"].ToString());
                                    item.pdf = (reader["pdf"].ToString());
                                    item.paciente = (reader["paciente"].ToString());
                                    item.listapagos = new List<PagosDocumento>();
                                    lista.Add(item);
                                }
                            }

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

        public async Task<List<ReporteDocumentosMensuales>> getReportDocumentosCortesias(int empresa, DateTime fi, DateTime ff)
        {
            List<ReporteDocumentosMensuales> lista = new List<ReporteDocumentosMensuales>();
            string queryString = "dbo.get_documentosCortesia";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@empresa", SqlDbType.Int).Value = empresa;
                    command.Parameters.Add("@fi", SqlDbType.VarChar).Value = fi.ToShortDateString();
                    command.Parameters.Add("@ff", SqlDbType.VarChar).Value = ff.ToShortDateString();
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    try
                    {
                        /*
                        SqlDataAdapter da = new SqlDataAdapter();
                        DataSet dt = new DataSet();
                        da.SelectCommand = command;
                        da.Fill(dt, "ReporteDocumentosMensuales");
                        */
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                //foreach (DataRow reader in dt.Tables[0].Rows)
                                {
                                    ReporteDocumentosMensuales item = new ReporteDocumentosMensuales();

                                    item.fechadoc = Convert.ToDateTime(reader["fechaemitio"].ToString());
                                    item.documento = (reader["numerodocumento"].ToString());
                                    item.sede = (reader["sucursal"].ToString());
                                    item.tipodocumento = (reader["tipodocumento"].ToString());
                                    item.codigomonedadoc = (reader["monedadoc"].ToString());
                                    item.monedadoc = (reader["moneda"].ToString());
                                    item.total = Convert.ToDouble(reader["total"].ToString());
                                    item.tipocambio = Convert.ToDouble(reader["tipocambio"].ToString());
                                    item.usuariodoc = (reader["usuariodoc"].ToString());
                                    item.razonsocial = (reader["razonsocialalterno"].ToString());
                                    item.estado = (reader["estado"].ToString());
                                    item.serie = (reader["serie"].ToString());
                                    item.pdf = (reader["pdf"].ToString());
                                    item.paciente = (reader["paciente"].ToString());
                                    item.listapagos = new List<PagosDocumento>();
                                    lista.Add(item);
                                }
                            }

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

        public async Task<ReporteAtencionesMensuales> getDetalleAtencion(ReporteAtencionesMensuales item)
        {
            string queryString = @"select numerocita,convert(varchar(20),fechacreacion,13) fechacreacion,convert(varchar(20),fechareserva,103) fechareserva,u.ShortName,observacion,c.codigocartagarantia from cita c inner join USUARIO u on c.codigousuario=u.codigousuario where numerocita=" + item.Numerocita + ";select ec.codigoestudio,es.nombreestudio,CONVERT(varchar(20),ec.horacita,108)horacita,ec.estadoestudio,ec.precioestudio from EXAMENXCITA ec inner join ESTUDIO es on ec.codigoestudio=es.codigoestudio where numerocita=" + item.Numerocita + @";
            select cg.codigocartagarantia ,cg.codigocartagarantia2,cs.descripcion,cg.seguimiento,u.ShortName registrador,ut.ShortName actualizador,ur.ShortName revisador from CARTAGARANTIA cg  inner join COMPANIASEGURO cs on cg.codigocompaniaseguro=cs.codigocompaniaseguro inner join USUARIO u on cg.codigousuario=u.codigousuario left join USUARIO ut on cg.user_update=ut.ShortName left join USUARIO ur on cg.user_revisa=ur.codigousuario inner join CITA c on cg.codigocartagarantia=c.codigocartagarantia where c.numerocita='" + item.Numerocita + @"'; 
 select e.nombreestudio,ec.cobertura_det from CARTAGARANTIA cg inner join ESTUDIO_CARTAGAR ec on cg.codigocartagarantia=ec.codigocartagarantia and cg.codigopaciente=ec.codigopaciente  inner join ESTUDIO e on ec.codigoestudio=e.codigoestudio inner join CITA c on cg.codigocartagarantia=c.codigocartagarantia where c.numerocita='" + item.Numerocita + @"';  
 select d.isFisico,'' ruta,d.codigodocadjunto,d.nombrearchivo,d.cuerpoarchivo from DOCESCANEADO d inner join CARTAGARANTIA cg on d.codigodocadjunto=cg.codigodocadjunto inner join CITA c on cg.codigocartagarantia=c.codigocartagarantia where c.numerocita='" + item.Numerocita + @"';

select numeroatencion,fechayhora fecha,peso,talla , u.ShortName,numeroTicket,SitedOrden,ch.razonsocial,(case when m.cmpCorregido is null then m.cmp else m.cmpCorregido end)+'- '+m.apellidos+', '+m.nombres medico from ATENCION a inner join USUARIO u on a.codigousuario=u.codigousuario inner join cita c on a.numerocita=c.numerocita inner join CLINICAHOSPITAL ch on c.codigoclinica=ch.codigoclinica inner join MEDICOEXTERNO m on a.cmp=m.cmp where c.numerocita=" + item.Numerocita + @";select nombrearchivo,cuerpoarchivo from ESCANADMISION e inner join ATENCION a on e.numerodeatencion=a.numeroatencion where a.numerocita='" + item.Numerocita + @"';

select en.ShortName en,e.fec_ini_encu fec_ini_encu,e.fec_paso3 fec_fin_encu,te.ShortName te,e.fec_ini_tecno ini_tec,e.fec_fin_tecno fin_tec,su.ShortName su,e.fec_Solicitavalidacion ini_sup,e.fec_supervisa fin_supe,inf.ShortName inf,i.fechainforme fin_inf,va.ShortName va,i.fecharevision fin_val, i.FirmaFecha,imp.ShortName imp from EXAMENXATENCION ea inner join Encuesta e on ea.codigo=e.numeroexamen left join INFORMEMEDICO i on ea.codigo=i.numeroinforme left join USUARIO en on e.usu_reg_encu = en.codigousuario left join USUARIO te on e.usu_reg_tecno = te.codigousuario left join USUARIO su on e.usu_reg_super = su.codigousuario left join USUARIO inf on i.medicoinforma = inf.codigousuario left join USUARIO va on i.medicorevisa = va.codigousuario left join USUARIO imp on i.FirmaUsuario = imp.codigousuario   where ea.codigo =" + item.Codigo + @";

select d.numerodocumento,d.estado,d.subtotal,d.igv,d.total,d.pathFile
from DOCUMENTO d 
left join COBRANZACIASEGURO cs on d.numerodocumento=cs.numerodocumento and d.codigopaciente=cs.codigopaciente
where 
d.codigopaciente={0} and d.codigounidad ={2} and d.estado<>'A' and (substring(d.numerodocumento,5,LEN(d.numerodocumento)-4)in (select lc.numerodocumento from LIBROCAJA lc where lc.numeroatencion={1} and lc.codigopaciente={0} and lc.codigounidad ={2} )or cs.numeroatencion={1})


select d.numerodocumento,d.estado,d.subtotal,d.igv,d.total,d.pathFile
from FacturaGlobal f 
inner join DetalleFacturaGlobal df on f.idFac=df.idFac
inner join DOCUMENTO d on d.numerodocumento=f.numerodocumento and d.codigounidad=f.empresa
inner join ATENCION a on df.atencion=a.numeroatencion
where df.atencion={1} and d.estado<>'A'and f.empresa={2}
";

            item.Datos_cita = new DataCita();
            item.Datos_carta = new DataCarta();
            item.Datos_carta.lista_estudios_carta = new List<DetalleDataCarta>();
            item.Datos_carta.lista_adjuntos_carta = new List<Adjuntos_Desktop>();
            item.Datos_admision = new DataAdmision();
            item.Datos_adquisicion = new DataAdquisicion();
            item.Datos_documentos = new List<DataDocumentos>();

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, item.Codigopaciente,item.Numeroatencion.ToString(), item.Unidad.ToString()), connection);
                    connection.Open();
                    command.Prepare();
                    //SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                                item.Datos_cita.Num_cita = (reader["numerocita"].ToString());
                                item.Datos_cita.Fecha_registro = (reader["fechacreacion"].ToString());
                                item.Datos_cita.Fecha_cita = (reader["fechareserva"].ToString());
                                item.Datos_cita.Codigocarta = (reader["codigocartagarantia"].ToString());
                                item.Datos_cita.Estado_cita = "-";
                                item.Datos_cita.Usuario_registra = (reader["ShortName"].ToString());
                                item.Datos_cita.Observaciones_cita = (reader["observacion"].ToString());
                                item.Datos_cita.Listaestudios = new List<DataCita.DetalleDataCita>();

                            }
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    item.Datos_cita.Listaestudios.Add(new DataCita.DetalleDataCita()
                                    {
                                        Codigoestudio = (reader["codigoestudio"].ToString()),
                                        Nombreestudio = (reader["nombreestudio"].ToString()),
                                        Horacita = (reader["horacita"].ToString()),
                                        Precioestudio = (reader["precioestudio"].ToString()),
                                        Estadoestudio = (reader["estadoestudio"].ToString()),
                                    });
                                }
                            }
                            //CARTA
                            if (await reader.NextResultAsync())
                                while (await reader.ReadAsync())
                                {
                                    item.Datos_carta.id_carta = (reader["codigocartagarantia"].ToString());
                                    item.Datos_carta.numero_carta = (reader["codigocartagarantia2"].ToString());
                                    item.Datos_carta.aseguradora_carta = (reader["descripcion"].ToString());
                                    item.Datos_carta.observaciones_cara = (reader["seguimiento"].ToString());
                                    item.Datos_carta.tramitado = (reader["registrador"].ToString());
                                    item.Datos_carta.actualizado = (reader["actualizador"].ToString());
                                    item.Datos_carta.revisado = (reader["revisador"].ToString());

                                }
                            if (await reader.NextResultAsync())
                                while (await reader.ReadAsync())
                                {
                                    item.Datos_carta.lista_estudios_carta.Add(new DetalleDataCarta
                                    {
                                        estudio = (reader["nombreestudio"].ToString()),
                                        cobertura = Convert.ToDouble(reader["cobertura_det"].ToString())
                                    });
                                }

                            if (await reader.NextResultAsync())
                                while (await reader.ReadAsync())
                                {
                                    item.Datos_carta.lista_adjuntos_carta.Add(new Adjuntos_Desktop()
                                    {
                                        isFisico = Convert.ToBoolean(reader["isFisico"].ToString()),
                                        ruta = Tool.PathDocumentosAdjuntos + (reader["codigodocadjunto"].ToString()) + @"\" + (reader["nombrearchivo"].ToString()),
                                        nombre = (reader["nombrearchivo"].ToString()),
                                        archivo = (reader["cuerpoarchivo"] != DBNull.Value) ? (byte[])(reader["cuerpoarchivo"]) : null,
                                        size = 0
                                    });
                                }

                            //ADMISION
                            if (await reader.NextResultAsync())
                                while (await reader.ReadAsync())
                                {
                                    item.Datos_admision.Numero_admision = (reader["numeroatencion"].ToString());
                                    item.Datos_admision.Fecha_admision = (Convert.ToDateTime(reader["fecha"].ToString()).ToString("dd/MM/yyyy HH:mm "));
                                    item.Datos_admision.Peso_admision = (reader["peso"].ToString());
                                    item.Datos_admision.Altura_admision = (reader["talla"].ToString());
                                    item.Datos_admision.Usuario = (reader["ShortName"].ToString());
                                    item.Datos_admision.Ticket = (reader["numeroTicket"].ToString());
                                    item.Datos_admision.Siteds = (reader["SitedOrden"].ToString());
                                    item.Datos_admision.Clinica = (reader["razonsocial"].ToString());
                                    item.Datos_admision.Medico = (reader["medico"].ToString());
                                    item.Datos_admision.IsAdjunto = false;
                                }
                            if (await reader.NextResultAsync())
                                while (await reader.ReadAsync())
                                {
                                    item.Datos_admision.IsAdjunto = true;
                                    item.Datos_admision.Nombrearchivo = (reader["nombrearchivo"].ToString());
                                    item.Datos_admision.Cuerpoarchivo = (byte[])(reader["cuerpoarchivo"]);
                                }

                            //ADQUISICION
                            if (await reader.NextResultAsync())
                                while (await reader.ReadAsync())
                                {
                                    item.Datos_adquisicion.encuestador = (reader["en"].ToString());
                                    if (reader["fec_ini_encu"] != DBNull.Value)
                                        item.Datos_adquisicion.ini_encuestador = (Convert.ToDateTime(reader["fec_ini_encu"].ToString())).ToString("dd/MM/yy HH:mm");
                                    if (reader["fec_fin_encu"] != DBNull.Value)
                                        item.Datos_adquisicion.fin_encuestador = (Convert.ToDateTime(reader["fec_fin_encu"].ToString())).ToString("dd/MM/yy HH:mm");
                                    item.Datos_adquisicion.tecnologo = (reader["te"].ToString());
                                    if (reader["ini_tec"] != DBNull.Value)
                                        item.Datos_adquisicion.ini_tecnologo = (Convert.ToDateTime(reader["ini_tec"].ToString())).ToString("dd/MM/yy HH:mm");
                                    if (reader["fin_tec"] != DBNull.Value)
                                        item.Datos_adquisicion.fin_tecnologo = (Convert.ToDateTime(reader["fin_tec"].ToString())).ToString("dd/MM/yy HH:mm");
                                    item.Datos_adquisicion.supervisor = (reader["su"].ToString());
                                    if (reader["ini_sup"] != DBNull.Value)
                                        item.Datos_adquisicion.ini_supervisor = (Convert.ToDateTime(reader["ini_sup"].ToString())).ToString("dd/MM/yy HH:mm");
                                    if (reader["fin_supe"] != DBNull.Value)
                                        item.Datos_adquisicion.fin_supervisor = (Convert.ToDateTime(reader["fin_supe"].ToString())).ToString("dd/MM/yy HH:mm");
                                    item.Datos_adquisicion.informante = (reader["inf"].ToString());
                                    if (reader["fin_inf"] != DBNull.Value)
                                        item.Datos_adquisicion.fin_informante = (Convert.ToDateTime(reader["fin_inf"].ToString())).ToString("dd/MM/yy HH:mm");
                                    item.Datos_adquisicion.validador = (reader["va"].ToString());
                                    if (reader["fin_val"] != DBNull.Value)
                                        item.Datos_adquisicion.fin_validador = (Convert.ToDateTime(reader["fin_val"].ToString())).ToString("dd/MM/yy HH:mm");
                                    item.Datos_adquisicion.impresion = (reader["imp"].ToString());
                                    if (reader["FirmaFecha"] != DBNull.Value)
                                        item.Datos_adquisicion.fin_impresion = (Convert.ToDateTime(reader["FirmaFecha"].ToString())).ToString("dd/MM/yy HH:mm");
                                }

                            //DOCUMENTOS
                            if (await reader.NextResultAsync())
                                while (await reader.ReadAsync())
                                {
                                    var doc = new DataDocumentos();
                                    doc.Numero_documento = (reader["numerodocumento"].ToString());
                                    doc.Estado = (reader["estado"].ToString());
                                    doc.Subtotal = Convert.ToDouble(reader["subtotal"].ToString());
                                    doc.Igv = Convert.ToDouble(reader["igv"].ToString());
                                    doc.Total = Convert.ToDouble(reader["total"].ToString());
                                    doc.Path = (reader["pathFile"].ToString());
                                    item.Datos_documentos.Add(doc);
                                }
                            if (await reader.NextResultAsync())
                                while (await reader.ReadAsync())
                                {
                                    var doc = new DataDocumentos();
                                    doc.Numero_documento = (reader["numerodocumento"].ToString());
                                    doc.Estado = (reader["estado"].ToString());
                                    doc.Subtotal = Convert.ToDouble(reader["subtotal"].ToString());
                                    doc.Igv = Convert.ToDouble(reader["igv"].ToString());
                                    doc.Total = Convert.ToDouble(reader["total"].ToString());
                                    doc.Path = (reader["pathFile"].ToString());
                                    item.Datos_documentos.Add(doc);
                                }
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        //reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return item;

        }
        public async Task<List<AsignacionHorarioTecnologo>> getListaHorarioTecnologo(DateTime fecha, string unidad, int tipo)
        {
            List<AsignacionHorarioTecnologo> lista = new List<AsignacionHorarioTecnologo>();
            string queryString = @"select h.idprogramacion,h.fecha, CAST(h.fecha AS DATETIME) + CAST(h.inicio AS DATETIME) inicio, CAST(h.fecha AS DATETIME) + CAST(h.FIN AS DATETIME)fin,h.codigoequipo,e.nombreequipo,h.tecnologo,isnull(u.shortname,'*No Asignado')shortname  from horario_tecnologo h 
inner join equipo e on h.codigoequipo=e.codigoequipo
left join usuario u on h.tecnologo=u.codigousuario
where unidad={1} and CONVERT(CHAR(7), fecha, 120)='{0}' and tipo_turno={2};";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, fecha.ToString("yyyy-MM"), unidad, tipo.ToString()), connection);
                    connection.Open();
                    command.Prepare();

                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                                while (await reader.ReadAsync())
                                {
                                    AsignacionHorarioTecnologo item = new AsignacionHorarioTecnologo();
                                    item.id = Convert.ToInt32(reader["idprogramacion"].ToString());
                                    item.fecha = Convert.ToDateTime(reader["fecha"].ToString());
                                    item.incio = Convert.ToDateTime(reader["inicio"].ToString());
                                    item.fin = Convert.ToDateTime(reader["fin"].ToString());
                                    item.codigoequipo = Convert.ToInt32(reader["codigoequipo"].ToString());
                                    item.nombreequipo = (reader["nombreequipo"].ToString());
                                    item.codigotecnologo = (reader["tecnologo"].ToString());
                                    item.nombretecnologo = (reader["shortname"].ToString());
                                    lista.Add(item);
                                }
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
    }
}