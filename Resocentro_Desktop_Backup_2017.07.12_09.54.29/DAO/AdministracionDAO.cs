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
                    connection.Open();
                    command.Prepare();

                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    ReporteAtencionesMensuales item = new ReporteAtencionesMensuales();
                                    item.sede = (reader["sede"].ToString());
                                    item.estadoestudio = (reader["estadoestudio"].ToString());
                                    item.fecha = Convert.ToDateTime(reader["fechayhora"].ToString());
                                    item.codigo = Convert.ToInt32(reader["codigo"].ToString());
                                    item.paciente = (reader["paciente"].ToString());
                                    item.codigopaciente = Convert.ToInt32(reader["codigopaciente"].ToString());
                                    item.numerocita = Convert.ToInt32(reader["numerocita"].ToString());
                                    item.numeroatencion = Convert.ToInt32(reader["numeroatencion"].ToString());
                                    item.tipopaciente = (reader["tipo"].ToString());
                                    item.paciente = (reader["paciente"].ToString());
                                    item.estudio = (reader["estudio"].ToString());
                                    item.aseguradora = (reader["Procedencia"].ToString());
                                    item.cobertura = Convert.ToDouble(reader["cobertura"].ToString());
                                    item.documentos = (reader["Facturas"].ToString());
                                    item.unidad = Convert.ToInt32(reader["unidad"].ToString());
                                    if (item.documentos == "")
                                        item.comentarios = "SIN DOCUMENTOS";
                                    else if (item.documentos.Split('#').Count() < 3 && item.cobertura > 0 && item.cobertura < 100)
                                        item.comentarios = "SOLO TIENE 1 DOCUMENTO";
                                    else
                                        item.comentarios = "OK";
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

        public DataCita getInfoCita(int numerocita)
        {
            DataCita item = new DataCita();
            string queryString = "select numerocita,convert(varchar(20),fechareserva,103) fechareserva,u.ShortName,observacion from cita c inner join USUARIO u on c.codigousuario=u.codigousuario where numerocita=" + numerocita;

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
                            item.num_cita = (reader["numerocita"].ToString());
                            item.fecha_cita = (reader["fechareserva"].ToString());
                            item.estado_cita = "-";
                            item.usuario_registra = (reader["ShortName"].ToString());
                            item.observaciones_cita = (reader["observacion"].ToString());

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
            return item;
        }

        public DataCarta getInfoCarta(int numerocita)
        {
            DataCarta item = new DataCarta();
            string queryString = "select cg.codigocartagarantia ,cg.codigocartagarantia2,cs.descripcion,cg.seguimiento from cita c inner join CARTAGARANTIA cg on c.codigocartagarantia=cg.codigocartagarantia and c.codigopaciente=cg.codigopaciente inner join COMPANIASEGURO cs on cg.codigocompaniaseguro=cs.codigocompaniaseguro inner join USUARIO u on c.codigousuario=u.codigousuario where numerocita={0}; select e.nombreestudio,ec.cobertura_det from cita c inner join CARTAGARANTIA cg on c.codigocartagarantia=cg.codigocartagarantia and c.codigopaciente=cg.codigopaciente inner join ESTUDIO_CARTAGAR ec on cg.codigocartagarantia=ec.codigocartagarantia and cg.codigopaciente=ec.codigopaciente  inner join ESTUDIO e on ec.codigoestudio=e.codigoestudio where numerocita={0};";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, numerocita), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            item.id_carta = (reader["codigocartagarantia"].ToString());
                            item.numero_carta = (reader["codigocartagarantia2"].ToString());
                            item.aseguradora_carta = (reader["descripcion"].ToString());
                            item.observaciones_cara = (reader["seguimiento"].ToString());
                            item.lista_estudios_carta = new List<DetalleDataCarta>();
                        }
                        if (reader.NextResult())
                            while (reader.Read())
                            {
                                item.lista_estudios_carta.Add(new DetalleDataCarta
                                {
                                    estudio = (reader["nombreestudio"].ToString()),
                                    cobertura = Convert.ToDouble(reader["cobertura_det"].ToString())
                                });
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
            return item;
        }

        public DataAdmision getInfoAdmision(int numerocita)
        {
            DataAdmision item = new DataAdmision();
            string queryString = "select numeroatencion,fechayhora fecha,peso,talla from ATENCION where numerocita={0};";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, numerocita), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            item.numero_admision = (reader["numeroatencion"].ToString());
                            item.fecha_admision = (Convert.ToDateTime(reader["fecha"].ToString()).ToString("dddd dd \"de\" MMM \"del\" yyyy HH:mm "));
                            item.peso_admision = (reader["peso"].ToString());
                            item.altura_admision = (reader["talla"].ToString());

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
            return item;
        }

        public DataAdquisicion getInfoAdquisicion(int codigo)
        {
            DataAdquisicion item = new DataAdquisicion();
            string queryString = "select en.ShortName en,te.ShortName te,su.ShortName su,inf.ShortName inf,va.ShortName va from EXAMENXATENCION ea inner join Encuesta e on ea.codigo=e.numeroexamen left join INFORMEMEDICO i on ea.codigo=i.numeroinforme left join USUARIO en on e.usu_reg_encu = en.codigousuario left join USUARIO te on e.usu_reg_tecno = te.codigousuario left join USUARIO su on e.usu_reg_super = su.codigousuario left join USUARIO inf on i.medicoinforma = inf.codigousuario left join USUARIO va on i.medicorevisa = va.codigousuario  where ea.codigo ={0};";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, codigo), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            item.encuestador = (reader["en"].ToString());
                            item.tecnologo = (reader["te"].ToString());
                            item.supervisor = (reader["su"].ToString());
                            item.informante = (reader["inf"].ToString());
                            item.validador = (reader["va"].ToString());
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
            return item;
        }


        public List<DataDocumentos> getInfoDocumento(string paciente, string unidad, string atenicon)
        {
            List<DataDocumentos> lista = new List<DataDocumentos>();
            string queryString = @"select d.numerodocumento,d.estado,d.subtotal,d.igv,d.total,d.pathFile
from DOCUMENTO d 
left join COBRANZACIASEGURO cs on d.numerodocumento=cs.numerodocumento and d.codigopaciente=cs.codigopaciente
where 
d.codigopaciente={0} and d.codigounidad ={2} and d.estado<>'A' and (substring(d.numerodocumento,5,LEN(d.numerodocumento)-4)in (select lc.numerodocumento from LIBROCAJA lc where lc.numeroatencion={1} and lc.codigopaciente={0} and lc.codigounidad ={2} )or cs.numeroatencion={1})


select d.numerodocumento,d.estado,d.subtotal,d.igv,d.total,d.pathFile
from FacturaGlobal f 
inner join DetalleFacturaGlobal df on f.idFac=df.idFac
inner join DOCUMENTO d on d.numerodocumento=f.numerodocumento and d.codigounidad=f.empresa
inner join ATENCION a on df.atencion=a.numeroatencion
where df.atencion={1} and d.estado<>'A'and f.empresa={2}";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, paciente, atenicon, unidad), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new DataDocumentos();
                            item.numero_documento = (reader["numerodocumento"].ToString());
                            item.estado = (reader["estado"].ToString());
                            item.subtotal = Convert.ToDouble(reader["subtotal"].ToString());
                            item.igv = Convert.ToDouble(reader["igv"].ToString());
                            item.total = Convert.ToDouble(reader["total"].ToString());
                            item.path = (reader["pathFile"].ToString());
                            lista.Add(item);
                        }
                        if (reader.NextResult())
                            while (reader.Read())
                            {
                                var item = new DataDocumentos();
                                item.numero_documento = (reader["numerodocumento"].ToString());
                                item.estado = (reader["estado"].ToString());
                                item.subtotal = Convert.ToDouble(reader["subtotal"].ToString());
                                item.igv = Convert.ToDouble(reader["igv"].ToString());
                                item.total = Convert.ToDouble(reader["total"].ToString());
                                item.path = (reader["pathFile"].ToString());
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

        public async Task<List<AsignacionHorarioTecnologo>> getListaHorarioTecnologo(DateTime fecha, string unidad,int tipo)
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

                    SqlCommand command = new SqlCommand(string.Format(queryString,fecha.ToString("yyyy-MM"),unidad,tipo.ToString()), connection);
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
                                    item.codigotecnologo =(reader["tecnologo"].ToString());
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