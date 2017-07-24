using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    class DocumentoDAO
    {
        public List<DocumentoEntity> getDocumento(string documento, int codigopaciente)
        {
            List<DocumentoEntity> lista = new List<DocumentoEntity>();
            string query = @"select un.NombreSmall Empresa,isnull(su.ShortDesc,'-') Sucursal,d.estado E,d.numerodocumento,poliza, case when tipodocumento='01' then 'Factura' when tipodocumento='03' then 'Boleta' when tipodocumento='07' then 'Nota Credito' else '' end tipodocumento,titular,m.descripcion moneda,ROUND(subtotal, 2) subtotal,ROUND(d.tipocambio, 2) tipocambio,ROUND(igv, 2) igv,ROUND(total, 2)  total,fechaemitio,porconcepto,codigocarta,cobertura,u.ShortName,d.pathFile,d.rucalterno FROM [dbo].[DOCUMENTO] d
inner join USUARIO u on d.codigousuario=u.codigousuario
inner join UNIDADNEGOCIO un on d.codigounidad= un.codigounidad
left join SUCURSAL su on d.codigounidad= su.codigounidad and d.codigosucursal=su.codigosucursal
inner join MONEDA m on d.tipomoneda=m.codigomoneda
where d.numerodocumento = '{0}' and codigopaciente='{1}'";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, documento, codigopaciente), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new DocumentoEntity();
                            item.Empresa = ((reader["Empresa"]).ToString());
                            item.Sucursal = ((reader["Sucursal"]).ToString());
                            item.E = ((reader["E"]).ToString());
                            item.Documento = ((reader["numerodocumento"]).ToString());
                            item.Poliza = ((reader["poliza"]).ToString());
                            item.tipo = ((reader["tipodocumento"]).ToString());
                            item.Titular = ((reader["titular"]).ToString());
                            item.Moneda = ((reader["moneda"]).ToString());
                            item.Subtotal = Convert.ToDouble((reader["subtotal"]).ToString());
                            item.TC = Convert.ToDouble((reader["tipocambio"]).ToString());
                            item.IGV = Convert.ToDouble((reader["igv"]).ToString());
                            item.Total = Convert.ToDouble((reader["total"]).ToString());
                            item.Emision = ((reader["fechaemitio"]).ToString());
                            item.Concepto = ((reader["porconcepto"]).ToString());
                            item.Carta = ((reader["codigocarta"]).ToString());
                            item.Cob = ((reader["cobertura"]).ToString());
                            item.Usuario = ((reader["ShortName"]).ToString());
                            item.pathfile = reader["pathFile"].ToString();
                            item.RucAlterno = reader["rucalterno"].ToString();

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

        public List<DetalleDocumentoEntity> getDetalleDocumento(string documento, int codigopaciente)
        {
            List<DetalleDocumentoEntity> lista = new List<DetalleDocumentoEntity>();
            string query = @"select codigodetalle,descripcion,ROUND(valorventa,2) valorventa,ROUND(desc_cortesia,2) desc_cortesia,ROUND(desc_carta,2) desc_carta,ROUND(desc_promociones,2) desc_promociones,ROUND(preciounitario,2) preciounitario,tipoIGV FROM [dbo].[DETALLEDOCUMENTO] where numerodocumento = '{0}' and codigopaciente='{1}'";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, documento, codigopaciente), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new DetalleDocumentoEntity();
                            item.Id = ((reader["codigodetalle"]).ToString());
                            item.Descripcion = ((reader["descripcion"]).ToString());
                            item.Cortesia = Convert.ToDouble((reader["desc_cortesia"]).ToString());
                            item.Carta = Convert.ToDouble((reader["desc_carta"]).ToString());
                            item.Promociones = Convert.ToDouble((reader["desc_promociones"]).ToString());
                            item.Unitario = Convert.ToDouble((reader["preciounitario"]).ToString());
                            item.Total = Convert.ToDouble((reader["valorventa"]).ToString());
                            item.TipoIGV = ((reader["tipoIGV"]).ToString());

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

        public List<COMPANIASEGURO> getAseguradoraDocumento(string ruc)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.COMPANIASEGURO.Where(x => x.ruc == ruc).ToList();
            }
        }

        public List<FormaPagoEntity> getFormaPagoDocumento(string documento, int codigopaciente,string empresa,string sede)
        {
            List<FormaPagoEntity> lista = new List<FormaPagoEntity>();
            string query = @"select codigoformapago,mp.descripcion modalidad,t.descripcion tarjeta,m.descripcion,ROUND(monto,2) monto,fechadepago,u.ShortName,isnull(fp.numeroReferencia,'') referencia ,m.descripcion moneda FROM [dbo].[FORMADEPAGO] fp 
inner join TARJETA t on fp.codigotarjeta=t.codigotarjeta
inner join MONEDA  m on fp.codigomoneda = m.codigomoneda
inner join MODALIDADPAGO mp on fp.codigomodalidadpago=mp.codigomodalidadpago
inner join USUARIO u on fp.codigousuario=u.codigousuario
where numerodocumento = '{0}' and codigopaciente='{1}' AND fp.codigounidad='" +empresa+"' AND fp.codigosucursal='"+sede+"'";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, documento, codigopaciente), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new FormaPagoEntity();
                            item.ID = Convert.ToInt32((reader["codigoformapago"]).ToString());
                            item.Modalidad = ((reader["modalidad"]).ToString());
                            item.Tarjeta = ((reader["tarjeta"]).ToString());
                            item.Descripcion = ((reader["descripcion"]).ToString());
                            item.Monto = Convert.ToDouble((reader["monto"]).ToString());
                            item.Fecha = ((reader["fechadepago"]).ToString());
                            item.Usuario = ((reader["ShortName"]).ToString());
                            item.referencia = ((reader["referencia"]).ToString());
                            item.moneda = ((reader["moneda"]).ToString());
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

        public List<CobranzaCiaEntity> getCobranzaDocumento(string documento, int codigopaciente)
        {
            List<CobranzaCiaEntity> lista = new List<CobranzaCiaEntity>();
            string query = @"select c.numeroatencion,c.fechaemision,c.estado E,c.numerodelote,isnull(c.IsExoneradoTrama,'0') IsExoneradoTrama,u.ShortName FROM COBRANZACIASEGURO c inner join USUARIO u on c.codigousuario=u.codigousuario where numerodocumento = '{0}' and codigopaciente='{1}'";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, documento, codigopaciente), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new CobranzaCiaEntity();
                            item.E = ((reader["E"]).ToString());
                            item.Atencion = ((reader["numeroatencion"]).ToString());
                            item.Emision = ((reader["fechaemision"]).ToString());
                            item.Lote = ((reader["numerodelote"]).ToString());
                            item.Trama = !(Convert.ToBoolean((reader["IsExoneradoTrama"]).ToString()));
                            item.Usuario = ((reader["ShortName"]).ToString());

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

        public List<LibroCajaEntity> getLibroCajaDocumento(string documento, int codigopaciente)
        {
            var num_documento = documento.Split('-');
            List<LibroCajaEntity> lista = new List<LibroCajaEntity>();
            string query = @"select numeroatencion, cs.descripcion aseguradora,lc.terminopago,descuento,ROUND(subtotal,2) subtotal,ROUND(igv,2) igv, ROUND(total,2)  total, pagoneto, saldo, cortesia, fechacobranza, u.ShortName, formaentrega, lugarentrega, u.ShortName from [dbo].[LIBROCAJA] lc
inner join COMPANIASEGURO cs on lc.codigocompaniaseguro=cs.codigocompaniaseguro
inner join USUARIO u on lc.codigousuario=u.codigousuario
where lc.numerodocumento = '{0}' and codigopaciente='{1}'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, num_documento[1], codigopaciente), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new LibroCajaEntity();
                            item.Atencion = ((reader["numeroatencion"]).ToString());
                            item.Aseguradora = ((reader["aseguradora"]).ToString());
                            item.Pago = ((reader["terminopago"]).ToString());
                            item.Descuento = Convert.ToDouble((reader["descuento"]).ToString());
                            item.SubTotal = Convert.ToDouble((reader["subtotal"]).ToString());
                            item.IGV = Convert.ToDouble((reader["igv"]).ToString());
                            item.Total = Convert.ToDouble((reader["total"]).ToString());
                            item.Neto = Convert.ToDouble((reader["pagoneto"]).ToString());
                            item.Saldo = Convert.ToDouble((reader["saldo"]).ToString());
                            item.Cortesia = (Convert.ToBoolean((reader["cortesia"]).ToString()));
                            item.Fecha = ((reader["fechacobranza"]).ToString());
                            item.Usuario = ((reader["shortname"]).ToString());
                            item.Entrega = ((reader["formaentrega"]).ToString());
                            item.Lugar = ((reader["lugarentrega"]).ToString());
                            item.Usuario = ((reader["ShortName"]).ToString());

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
