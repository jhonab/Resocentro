using Oracle.ManagedDataAccess.Client;
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
    public class InfoTacticaDAO
    {

        public async Task<List<CabeceraDocumentoOracle>> getCabeceraOracle(string inicio, string fin, string empresa)
        {
            #region ORACLE
            List<CabeceraDocumentoOracle> lista = new List<CabeceraDocumentoOracle>();
            string conexion = new CobranzaDAO().getConexionOracle(int.Parse(empresa));
            using (OracleConnection connection = new OracleConnection(conexion))
            {
                connection.Open();
                OracleCommand command = new OracleCommand();
                OracleTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                string sqlTVVFACUTRA = "select f.fecha_g,f.num_fact,round(nvl(case when f.moneda=0then f.monto_b else f.monto_b*f.tipo_cambio end,0),2)monto_b,round(nvl(case when f.moneda=0then f.impuesto_m else f.impuesto_m*f.tipo_cambio end,0),2)impuesto_m,round(nvl(case when f.moneda=0then f.monto_f else f.monto_f*f.tipo_cambio end,0),2)monto_f,round(nvl(case when f.moneda=1then f.monto_b else f.monto_b/f.tipo_cambio end,0),2)monto_b_dol,round(nvl(case when f.moneda=1then f.impuesto_m else f.impuesto_m/f.tipo_cambio end,0),2)impuesto_m_dol,round(nvl(case when f.moneda=1then f.monto_f else f.monto_f/f.tipo_cambio end,0),2)monto_f_dol,f.estado,f.moneda,f.cod_doc,f.tipo_cambio,f.empresa,f.cliente,f.cod_auxiliar,f.serie from tvv_factura f where trunc(f.fecha_g) BETWEEN TO_DATE('" + inicio + "', 'dd-MM-yyyy') AND TO_DATE('" + fin + "', 'dd-MM-yyyy') ";
               
                try
                {
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandType = CommandType.Text;

                    command.CommandText = sqlTVVFACUTRA;
                    command.Prepare();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            CabeceraDocumentoOracle item = new CabeceraDocumentoOracle();
                            item.fecha = Convert.ToDateTime(reader["fecha_g"].ToString());
                            item.num_doc = (reader["num_fact"].ToString());

                            item.subtotal = Convert.ToDouble(reader["monto_b"].ToString());
                            item.igv = Convert.ToDouble(reader["impuesto_m"].ToString());
                            item.total = Convert.ToDouble(reader["monto_f"].ToString());

                            item.subtotal_dolares = Convert.ToDouble(reader["monto_b_dol"].ToString());
                            item.igv_dolares = Convert.ToDouble(reader["impuesto_m_dol"].ToString());
                            item.total_dolares = Convert.ToDouble(reader["monto_f_dol"].ToString());

                            item.estado = Convert.ToInt32(reader["estado"].ToString());
                            item.moneda = Convert.ToInt32(reader["moneda"].ToString());
                            item.tipodocumento = (reader["cod_doc"].ToString());
                            item.tipo_cambio = Convert.ToDouble(reader["tipo_cambio"].ToString());
                            item.empresa = Convert.ToInt32(reader["empresa"].ToString());
                            item.razonsocial = (reader["cliente"].ToString());
                            item.ruc = (reader["cod_auxiliar"].ToString());
                            item.serie = (reader["serie"].ToString());

                            if (item.tipodocumento == "07")
                            {
                                item.subtotal = (item.subtotal) * -1;
                                item.igv = (item.igv) * -1;
                                item.total = (item.total) * -1;

                                item.subtotal_dolares = (item.subtotal_dolares) * -1;
                                item.igv_dolares = (item.igv_dolares) * -1;
                                item.total_dolares = (item.total_dolares) * -1;
                            }

                            lista.Add(item);
                        }
                    }

                    transaction.Commit();

                }
                catch (Exception ex)
                {

                    string mensaje = "ERROR ORACLE: Obtener Data Cabecera" + ex.Message;

                    throw new Exception(mensaje);
                }
                finally
                {
                    transaction.Dispose();
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
                return lista;
            }
            #endregion
        }
        public async Task<List<DetalleDocumentoOracle>> getDetalleOracle(string inicio, string fin, string empresa)
        {
            #region ORACLE
            List<DetalleDocumentoOracle> lista = new List<DetalleDocumentoOracle>();
            string conexion = new CobranzaDAO().getConexionOracle(int.Parse(empresa));
            using (OracleConnection connection = new OracleConnection(conexion))
            {
                connection.Open();
                OracleCommand command = new OracleCommand();
                OracleTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                string sqlTVVDETFACUTRA = "select d.num_fact,d.descripcion,round(nvl(case when f.moneda=0then d.precio else d.precio*f.tipo_cambio end,0),2)precio,round(nvl(case when f.moneda=1then d.precio else d.precio/f.tipo_cambio end,0),2)precio_dol,d.cod_doc,nvl(d.TIPO_OPERACION,'-')registro_ventas,f.moneda from tvv_factura f inner join tvv_det_factura d on f.num_fact =d.num_fact  where trunc(f.fecha_g) BETWEEN TO_DATE('" + inicio + "', 'dd-MM-yyyy') AND TO_DATE('" + fin + "', 'dd-MM-yyyy') ";


                try
                {
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandType = CommandType.Text;

                    command.CommandText = sqlTVVDETFACUTRA;
                    command.Prepare();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DetalleDocumentoOracle item = new DetalleDocumentoOracle();
                            item.num_doc = (reader["num_fact"].ToString());
                            item.descripcion = (reader["descripcion"].ToString());
                            item.precio = Convert.ToDouble(reader["precio"].ToString());
                            item.precio_dol = Convert.ToDouble(reader["precio_dol"].ToString());
                            item.tipodocumento = (reader["cod_doc"].ToString());
                            item.registro_ventas = (reader["registro_ventas"].ToString());

                            if (item.tipodocumento == "07")
                            {
                                item.precio = (item.precio) * -1;
                            }
                            lista.Add(item);
                        }
                    }

                    transaction.Commit();

                }
                catch (Exception ex)
                {

                    string mensaje = "ERROR ORACLE: Obtener Data Detalle" + ex.Message;

                    throw new Exception(mensaje);
                }
                finally
                {
                    transaction.Dispose();
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
                return lista;
            }
            #endregion
        }
        public async Task<List<RGVCAFAC_Result>> getRGVCAFAC(string inicio, string fin, string empresa)
        {
            var lista = new List<RGVCAFAC_Result>();
            string queryString = "dbo.RGVCAFAC";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@fechaIni", SqlDbType.VarChar).Value = inicio;
                    command.Parameters.Add("@fechaFin", SqlDbType.VarChar).Value = fin;
                    command.Parameters.Add("@uni", SqlDbType.Int).Value = int.Parse(empresa);

                    command.CommandTimeout = 500;
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Prepare();


                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                                while (await reader.ReadAsync())
                                {
                                    RGVCAFAC_Result item = new RGVCAFAC_Result();
                                    item.N_EXAMEN = (reader["N°EXAMEN"].ToString());
                                    item.CODEAUX = (reader["CODEAUX"].ToString());
                                    item.FECRGV = (reader["FECRGV"].ToString());
                                    item.MES = (reader["MES"].ToString());
                                    item.TIDORGV = (reader["TIDORGV"].ToString());
                                    item.TDORGV = (reader["TDORGV"].ToString());
                                    item.NDORGV = (reader["NDORGV"].ToString());
                                    item.MONRGV = (reader["MONRGV"].ToString());
                                    item.RUCCLI = (reader["RUCCLI"].ToString());
                                    item.RAZCLI = (reader["RAZCLI"].ToString());
                                    item.CDCRGV = (reader["CDCRGV"].ToString());
                                    item.TERRGV = (reader["TERRGV"].ToString());
                                    item.PDSRGV = float.Parse(reader["PDSRGV"].ToString());
                                    item.VVTRGVS = Convert.ToDecimal(reader["VVTRGVS"].ToString());
                                    item.IGVRGVS = Convert.ToDecimal(reader["IGVRGVS"].ToString());
                                    item.TOTRGVS = Convert.ToDecimal(reader["TOTRGVS"].ToString());
                                    item.TCARGV = float.Parse(reader["TCARGV"].ToString());
                                    item.VVTRGVD = Convert.ToDecimal(reader["VVTRGVD"].ToString());
                                    item.IGVRGVD = Convert.ToDecimal(reader["IGVRGVD"].ToString());
                                    item.TOTRGVD = Convert.ToDecimal(reader["TOTRGVD"].ToString());
                                    item.PACEST = (reader["PACEST"].ToString());
                                    item.CODUNI = (reader["CODUNI"].ToString());
                                    item.CODSUC = (reader["CODSUC"].ToString());
                                    item.DIRCLI = (reader["DIRCLI"].ToString());
                                    item.TELCLI = (reader["TELCLI"].ToString());
                                    item.PFLAG = Convert.ToInt32(reader["PFLAG"].ToString());
                                    item.SERIE = (reader["SERIE"].ToString());
                                    item.CORRELATIVO = Convert.ToInt32(reader["CORRELATIVO"].ToString());
                                    item.DOC_FECHAEMITIO = (reader["DOC_FECHAEMITIO"].ToString());
                                    item.DOC_NUMERODOC = (reader["DOC_NUMERODOC"].ToString());
                                    item.DOC_TIPO = (reader["DOC_TIPO"].ToString());
                                    item.DOC_SERIE = (reader["DOC_SERIE"].ToString());
                                    item.DOC_NUMERO = (reader["DOC_NUMERO"].ToString());
                                    item.Medico = (reader["Medico"].ToString());
                                    item.Promotor = (reader["Promotor"].ToString());
                                    item.isProtocolo = Convert.ToBoolean(reader["isProtocolo"].ToString());
                                    item.pdf = (reader["pdf"].ToString());
                                    item.zip = (reader["zip"].ToString());
                                    item.result = (reader["result"].ToString()).Trim();
                                    item.isSendSUNAT = Convert.ToBoolean(reader["isSendSUNAT"].ToString());

                                    lista.Add(item);
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
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

        public List<RGVCAFACII_Result> getRGVCAFACII(int ano, string empresa)
        {
            List<RGVCAFACII_Result> lista = new List<RGVCAFACII_Result>();
            string queryString = "dbo.RGVCAFACII";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@year", SqlDbType.Int).Value = ano;
                    command.Parameters.Add("@uni", SqlDbType.Int).Value = int.Parse(empresa);

                    command.CommandTimeout = 500;
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Prepare();


                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    RGVCAFACII_Result item = new RGVCAFACII_Result();
                                    item.numerodocumento = (reader["numerodocumento"].ToString());
                                    item.FECRGV = (reader["FECRGV"].ToString());
                                    item.MES = (reader["MES"].ToString());
                                    item.CODUNI = (reader["CODUNI"].ToString());
                                    item.TOTRGVS = Convert.ToDecimal(reader["TOTRGVS"].ToString());
                                    lista.Add(item);
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
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
        public async Task<List<RGVFACTU_Result>> getRGVFACTU(string inicio, string fin, string empresa)
        {
            List<RGVFACTU_Result> lista = new List<RGVFACTU_Result>();
            string queryString = "dbo.RGVFACTU";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@fechaIni", SqlDbType.VarChar).Value = inicio;
                    command.Parameters.Add("@fechaFin", SqlDbType.VarChar).Value = fin;
                    command.Parameters.Add("@uni", SqlDbType.Int).Value = int.Parse(empresa);


                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Prepare();
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                                while (await reader.ReadAsync())
                                {
                                    RGVFACTU_Result item = new RGVFACTU_Result();
                                    item.N_EXAMEN = (reader["N°EXAMEN"].ToString());
                                    item.CODEAUX = (reader["CODEAUX"].ToString());
                                    item.FECRGV = (reader["FECRGV"].ToString());
                                    item.TIDORGV = (reader["TIDORGV"].ToString());
                                    item.TDORGV = (reader["TDORGV"].ToString());
                                    item.NDORGV = (reader["NDORGV"].ToString());
                                    item.MONRGV = (reader["MONRGV"].ToString());
                                    item.RUCCLI = (reader["RUCCLI"].ToString());
                                    item.RAZCLI = (reader["RAZCLI"].ToString());
                                    item.CDCRGV = (reader["CDCRGV"].ToString());
                                    item.TERRGV = (reader["TERRGV"].ToString());
                                    item.PDSRGV = float.Parse(reader["PDSRGV"].ToString());
                                    item.VVTRGVS = Convert.ToDecimal(reader["VVTRGVS"].ToString());
                                    item.IGVRGVS = Convert.ToDecimal(reader["IGVRGVS"].ToString());
                                    item.TOTRGVS = Convert.ToDecimal(reader["TOTRGVS"].ToString());
                                    item.TCARGV = float.Parse(reader["TCARGV"].ToString());
                                    item.VVTRGVD = Convert.ToDecimal(reader["VVTRGVD"].ToString());
                                    item.IGVRGVD = Convert.ToDecimal(reader["IGVRGVD"].ToString());
                                    item.TOTRGVD = Convert.ToDecimal(reader["TOTRGVD"].ToString());
                                    item.CODEST = (reader["CODEST"].ToString());
                                    item.CODEST1 = (reader["CODEST1"].ToString());
                                    item.ESTUDIO = (reader["ESTUDIO"].ToString());
                                    item.DESEST = (reader["DESEST"].ToString());
                                    item.CLAEST = (reader["CLAEST"].ToString());
                                    item.CLAEST1 = (reader["CLAEST1"].ToString());
                                    item.PACEST = (reader["PACEST"].ToString());
                                    item.CODUNI = (reader["CODUNI"].ToString());
                                    item.CODSUC = (reader["CODSUC"].ToString());
                                    item.DIRCLI = (reader["DIRCLI"].ToString());
                                    item.TELCLI = (reader["TELCLI"].ToString());
                                    item.IDMODALIDAD = Convert.ToInt32(reader["IDMODALIDAD"].ToString());
                                    item.MODALIDAD = (reader["MODALIDAD"].ToString());
                                    item.IDCLASE = Convert.ToInt32(reader["IDCLASE"].ToString());
                                    item.NOMBRECLASE = (reader["NOMBRECLASE"].ToString());
                                    item.Medico = (reader["Medico"].ToString());
                                    item.Promotor = (reader["Promotor"].ToString());
                                    item.isProtocolo = Convert.ToBoolean(reader["isProtocolo"].ToString());
                                    item.tipoIGV = Enum.GetName(typeof(TIPO_IGV), int.Parse(reader["tipoIGV"].ToString()));
                                    lista.Add(item);
                                }
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
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
        public SUCURSAL getSucursal(string unidad, string sede)
        {
            SUCURSAL sucursal = new SUCURSAL();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string sqlUpdate = "select u.nombre,s.ShortDesc,direccionfactura from sucursal s inner join UNIDADNEGOCIO u on s.codigounidad=u.codigounidad where s.codigounidad='" + unidad + "' and s.codigosucursal='" + sede + "';";

                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sqlUpdate, connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {

                        while (reader.Read())
                        {
                            sucursal.direccion = reader["direccionfactura"].ToString();
                            sucursal.ShortDesc = reader["ShortDesc"].ToString();
                            sucursal.descripcion = reader["nombre"].ToString();
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return sucursal;
        }
        public List<string> getMedicopromotorAtencion(string atencion)
        {
            List<string> lista = new List<string>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string sqlUpdate = "select m.apellidos+' '+m.nombres medico,'-' promotor from ATENCION a inner join MEDICOEXTERNO m on a.cmp=m.cmp  where a.numeroatencion='" + atencion + "';";

                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sqlUpdate, connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {

                        while (reader.Read())
                        {
                            lista.Add(reader["medico"].ToString());
                            lista.Add(reader["promotor"].ToString());
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return lista;
        }

        public List<reporte_produccionGrafica_Result> getReporteProduccion(int year, string empresa)
        {
            List<reporte_produccionGrafica_Result> lista = new List<reporte_produccionGrafica_Result>();
            string queryString = "dbo.reporte_produccionGrafica";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@ano", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@empresa", SqlDbType.VarChar).Value = empresa;

                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader;
                    try
                    {
                        reader = command.ExecuteReader();
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                reporte_produccionGrafica_Result item = new reporte_produccionGrafica_Result();
                                item.Fecha_Hora = Convert.ToDateTime(reader["Fecha Hora"].ToString());
                                item.Id_Paciente = Convert.ToInt32(reader["Id Paciente"].ToString());
                                item.N__Cita = Convert.ToInt32(reader["N° Cita"].ToString());
                                item.N__Atencion = Convert.ToInt32(reader["N° Atencion"].ToString());
                                item.N__Examen = Convert.ToInt32(reader["N° Examen"].ToString());
                                item.Empresa = (reader["Empresa"].ToString());
                                item.mes = (reader["mes"].ToString());
                                item.Sucursal = (reader["Sucursal"].ToString());
                                item.Monto_Facturado_S__ = Convert.ToDecimal(reader["Monto Facturado S/."].ToString());
                                item.modalidad = (reader["modalidad"].ToString());

                                lista.Add(item);
                            }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
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

        public List<reporte_VentasGrafica_Result> getReporteVentas(int year, string empresa)
        {
            List<reporte_VentasGrafica_Result> lista = new List<reporte_VentasGrafica_Result>();
            string queryString = "dbo.reporte_VentasGrafica";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@ano", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@empresa", SqlDbType.VarChar).Value = empresa;

                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader;
                    try
                    {
                        reader = command.ExecuteReader();
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                reporte_VentasGrafica_Result item = new reporte_VentasGrafica_Result();
                                item.fechayhora = Convert.ToDateTime(reader["fechayhora"].ToString());
                                item.codigopaciente = Convert.ToInt32(reader["codigopaciente"].ToString());
                                item.numerocita = Convert.ToInt32(reader["numerocita"].ToString());
                                item.numeroatencion = Convert.ToInt32(reader["numeroatencion"].ToString());
                                item.examen = Convert.ToInt32(reader["examen"].ToString());
                                item.mes = (reader["mes"].ToString());
                                item.sucursal = (reader["sucursal"].ToString());
                                item.Monto = Convert.ToDecimal(reader["Monto"].ToString());
                                item.modalidad = (reader["modalidad"].ToString());

                                lista.Add(item);
                            }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
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
