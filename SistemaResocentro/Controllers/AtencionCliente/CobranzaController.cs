using SistemaResocentro.Member;
using SistemaResocentro.Models;
using SistemaResocentro.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaResocentro.Controllers.AtencionCliente
{
    [Authorize]
    public class CobranzaController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: Cobranza
        public ActionResult ListaAtenciones(string fecha, string unidad)
        {
            List<CobranzaViewModel> lista = new List<CobranzaViewModel>();
            if (fecha != null && unidad != null)
            {
                string queryString = @"select CONVERT(CHAR(5),a.fechayhora,108) AS hora,a.numeroatencion,(p.apellidos+' '+p.nombres) AS paciente,a.numerocita,p.codigopaciente
 from EXAMENXATENCION ea 
inner join ATENCION a on ea.numeroatencion=a.numeroatencion
inner join PACIENTE p on ea.codigopaciente= p.codigopaciente

WHERE SUBSTRING(ea.codigoestudio,1,3)='{0}' 
and convert(date,a.fechayhora)='{1}'
 AND ea.estadoestudio='R' 

 group by  CONVERT(CHAR(5),a.fechayhora,108) ,a.numeroatencion,(p.apellidos+' '+p.nombres) ,a.numerocita,p.codigopaciente";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, unidad, fecha), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                CobranzaViewModel item = new CobranzaViewModel();
                                item.hora = reader["hora"].ToString();
                                item.numeroatencion = Convert.ToInt32(reader["numeroatencion"].ToString());
                                item.paciente = reader["paciente"].ToString();
                                item.codigopaciente = Convert.ToInt32(reader["codigopaciente"].ToString());
                                item.numerocita = Convert.ToInt32(reader["numerocita"].ToString());
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
            unidad = unidad == null ? "" : unidad;
            ViewBag.fecha = fecha == null ? DateTime.Now.ToString("MM/dd/yyyy") : fecha.Split('/')[1] + "/" + fecha.Split('/')[0] + "/" + fecha.Split('/')[2];
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            ViewBag.Sedes = new SelectList(db.SUCURSALXUSUARIO.Where(x => x.codigousuario == cod_usu && x.codigounidad == 1).Select(x => new { codigo = (x.codigounidad * 100) + x.codigosucursal, value = "Resocentro - " + x.SUCURSAL.ShortDesc }).ToList(), "codigo", "value", unidad);
            return View(lista);
        }


        public ActionResult RealizarCobranza(int atencion)
        {
            ViewBag.Dept = new SelectList(db.UBIGEO.Select(x => new { codigo = x.Departamento }).Distinct().OrderBy(x => x.codigo).ToList(), "codigo", "codigo");
            ViewBag.TipoDoc = new SelectList(new Variable().getTipoDocumentoSUNAT().ToList(), "codigo", "nombre");
            GenerarDocumento item = new GenerarDocumento();
            var _atencion = db.ATENCION.SingleOrDefault(x => x.numeroatencion == atencion);

            if (_atencion != null)
            {
                var _paciente = _atencion.PACIENTE;
                item.encabezado = new DocumentoSunat();
                item.encabezado.tipoDocReceptor = "1";//DNI
                item.encabezado.rucReceptor = _paciente.dni;
                item.encabezado.razonSocialReceptor = _paciente.apellidos + " " + _paciente.nombres;
                if (_paciente.UBIGEO != null)
                {
                    item.encabezado.ubigeoReceptor = _paciente.UBIGEO.IneiId;
                    item.encabezado.departamentoReceptor = _paciente.UBIGEO.Departamento;
                    item.encabezado.provinciaReceptor = _paciente.UBIGEO.Provincia;
                    item.encabezado.distritoReceptor = _paciente.UBIGEO.Distrito;
                    item.encabezado.direccionReceptor = _paciente.direccion;
                }
                item.encabezado.emailReceptor = _paciente.email;
                item.encabezado.celularReceptor = _paciente.celular;
                item.encabezado.telefonoReceptor = _paciente.telefono;

                var _aseguradora = _atencion.CITA.COMPANIASEGURO;
                var _compania = db.ASEGURADORA.SingleOrDefault(x => x.ruc == _aseguradora.ruc);
                if (_compania != null)
                {
                    item.encabezado.razSocialAseguradora = _compania.razonsocial;
                    item.encabezado.rucAseguradora = _compania.ruc;
                    item.encabezado.tipodocAseguradora = "6";
                    if (_compania.UBIGEO != null)
                    {
                        item.encabezado.ubigeoAseguradora = _compania.UBIGEO.IneiId;
                        item.encabezado.departamentoAseguradora = _compania.UBIGEO.Departamento;
                        item.encabezado.provinciaAseguradora = _compania.UBIGEO.Provincia;
                        item.encabezado.distritoAseguradora = _compania.UBIGEO.Distrito;
                        item.encabezado.direccionAseguradora = _compania.domiciliofiscal;
                    }
                }
                var _item = getEstudios(atencion, item);
                item.detalle = _item.detalle;
                ViewBag.TCVenta = _item.TCVenta;
                ViewBag.TCCompra = _item.TCCompra;


            }
            return View(item);
        }

        public GenerarDocumento getEstudios(int atencion, GenerarDocumento item)
        {
            item.detalle = new List<DetalleDocumentoSunat>();
            double _igv = 0, _tventa = 0, tcompra = 0;
            var _atencion = db.ATENCION.SingleOrDefault(x => x.numeroatencion == atencion);
            string queryString = @"select * from TIPOCAMBIO where convert(date,fecha)='{0}'";
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(string.Format(queryString, DateTime.Now.ToString("dd/MM/yyyy")), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {

                            _igv = Math.Round(Convert.ToDouble(reader["igv"].ToString()), 2);
                            _tventa = Math.Round(Convert.ToDouble(reader["preciodeventa"].ToString()), 2);
                            tcompra = Math.Round(Convert.ToDouble(reader["preciodecompra"].ToString()), 2);
                        }

                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Close();

                }

            }
            var _cobertura = 0.0;
            queryString = @"select count(*) cant,es.nombreestudio,esc.preciobruto,es.codigoestudio,esc.codigomoneda,c.codigocartagarantia,isnull((select cg.cobertura from CARTAGARANTIA cg where cg.codigocartagarantia=c.codigocartagarantia and cg.codigopaciente=c.codigopaciente),'0') cob from EXAMENXCITA ec inner join CITA c on ec.numerocita=c.numerocita inner join ESTUDIO es on ec.codigoestudio=es.codigoestudio inner join ESTUDIO_COMPANIA esc on ec.codigoestudio=esc.codigoestudio and ec.codigocompaniaseguro=esc.codigocompaniaseguro where ec.numerocita={0} and ec.estadoestudio='R' group by es.nombreestudio,esc.preciobruto,ec.precioestudio,es.codigoestudio,esc.codigomoneda,c.codigocartagarantia,c.codigopaciente";
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(string.Format(queryString, _atencion.numerocita.ToString()), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            DetalleDocumentoSunat d = new DetalleDocumentoSunat();
                            d.cantidad = Convert.ToInt32(reader["cant"].ToString());
                            d.igvUnitario = _igv;//igv 18%
                            d.descxItem = 0;
                            d.valorUnitarioigv = Math.Round(Convert.ToDouble(reader["preciobruto"].ToString()), 2);
                            d.descripcion = reader["nombreestudio"].ToString();
                            d.codigoitem = reader["codigoestudio"].ToString();
                            item.encabezado.tipoMoneda = reader["codigomoneda"].ToString() == "1" ? "PEN" : reader["codigomoneda"].ToString() == "2" ? "USD" : "";
                            int codigomoneda = Convert.ToInt32(reader["codigomoneda"].ToString());
                            d.simboloMoneda = codigomoneda.ToString() == "1" ? "S/." : codigomoneda.ToString() == "2" ? "$" : "";
                            _cobertura = Math.Round(Convert.ToDouble(reader["cob"].ToString()), 2);

                            //if()
                            string queryCoaseguro = "select descuento,cobertura_det,preciobruto from ESTUDIO_CARTAGAR where codigocartagarantia='{0}' and codigopaciente='{1}' and codigoestudio='{2}'";
                            SqlCommand commandCoa = new SqlCommand(string.Format(queryCoaseguro, reader["codigocartagarantia"].ToString(), _atencion.codigopaciente.ToString(), reader["codigoestudio"].ToString()), connection);
                            SqlDataReader readerCoa = commandCoa.ExecuteReader();
                            try
                            {
                                if (readerCoa.HasRows)
                                {
                                    while (readerCoa.Read())
                                    {
                                        d.porcentajeSeguro = Math.Round(Convert.ToDouble(readerCoa["cobertura_det"].ToString()), 2);
                                        d.descxSeguro = -1 * Math.Round((Convert.ToDouble(readerCoa["descuento"].ToString())) / (1 + _igv), 2);
                                        d.porcentajeCoaseguro = 100 - d.porcentajeSeguro;
                                        d.descxCoaseguro = Math.Round((Convert.ToDouble(readerCoa["preciobruto"].ToString())) / (1 + _igv), 2) - d.descxSeguro;
                                    }
                                }
                                else
                                {
                                    d.porcentajeSeguro = _cobertura;
                                    d.descxSeguro = d.valorUnitarioigv * (_cobertura / 100);
                                    d.porcentajeCoaseguro = 100 - d.porcentajeSeguro;
                                    d.descxCoaseguro = d.valorUnitarioigv - d.descxSeguro;
                                }
                            }
                            finally
                            {
                                // Always call Close when done reading.
                                readerCoa.Close();
                            }

                            if (d.valorUnitario * (d.porcentajeSeguro / 100) <= (d.descxSeguro))
                            {
                                d.descxSeguro = Math.Round(d.valorUnitario * (d.porcentajeSeguro / 100), 2);
                                d.descxCoaseguro = Math.Round(d.valorUnitario * (d.porcentajeCoaseguro / 100), 2);
                            }

                            item.detalle.Add(d);
                        }

                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Close();

                }

            }
            item.TCVenta = _tventa;
            item.TCCompra = _tventa;
            var _aseguradora = _atencion.CITA.COMPANIASEGURO;
            var _compania = db.ASEGURADORA.SingleOrDefault(x => x.ruc == _aseguradora.ruc);
            if (_compania != null)
            {
                item.encabezado.razSocialAseguradora = _compania.razonsocial;
            }
            return item;
        }

        public ActionResult ListaEstudiosCobranza(int atencion)
        {
            var item = getEstudios(atencion, new GenerarDocumento { encabezado = new DocumentoSunat(), detalle = new List<DetalleDocumentoSunat>() });
            return View(item);
        }

        public ActionResult getAseguradora(string ruc)
        {
            string queryString = "select a.ruc,a.razonsocial,a.domiciliofiscal,a.ubigeoid,isnull(cs.descripcion,'-')descripcion,isnull(cs.codigocompaniaseguro,'-')codigocompaniaseguro  from ASEGURADORA a left join COMPANIASEGURO cs on a.ruc=cs.ruc where a.ruc like'{0}%'";
            List<EmpresaSunat> lista = new List<EmpresaSunat>();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(string.Format(queryString, ruc), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            EmpresaSunat e = new EmpresaSunat();
                            e.ruc = reader["ruc"].ToString();
                            e.razonsocial = reader["razonsocial"].ToString();
                            e.domicilio = reader["domiciliofiscal"].ToString();
                            e.ubigeoId = reader["ubigeoid"].ToString();
                            e.compania = reader["descripcion"].ToString();
                            e.codigocompaniaseguro = Convert.ToInt32(reader["codigocompaniaseguro"].ToString());
                            lista.Add(e);
                        }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Close();

                }
            }
            return View(lista);
        }

    }
}