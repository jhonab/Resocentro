using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Server.DAO
{
    class Sunat
    {
        public static string PathDocumentosFacturacion { get { return @"\\serverweb\Facturacion\"; } }
        public string sendBill(int empresa, int codigopaciente, string filename)
        {


            string pathMain = PathDocumentosFacturacion + codigopaciente.ToString() + "\\ZIP\\" + filename + ".zip";
            byte[] byteArray = File.ReadAllBytes(pathMain);
            string pathCDR = "";
            if (empresa == 1)
            {
                System.Net.ServicePointManager.UseNagleAlgorithm = true;
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.CheckCertificateRevocationList = true;

                ServiceSunatResocentro.billServiceClient wService;
                wService = new ServiceSunatResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    byte[] returnByte = wService.sendBill(filename + ".zip", byteArray,"");
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBILL", "-");
                    wService.Close();
                    pathCDR = PathDocumentosFacturacion + codigopaciente.ToString() + @"\RESULT\R-" + filename + ".zip";
                    System.IO.File.WriteAllBytes(pathCDR, returnByte);
                    new CobranzaDAO().insertarLogDocumento(filename, "RESPONDSENDBILL", "-");

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBILL", "-", ex.Code.Name);
                    throw new Exception( ex.Code.Name + "| - " + CobranzaDAO.getErrorSunat(ex.Code.Name.ToString())+"\n\n"+ex.Message + "\n" );
                }
            }
            else if (empresa == 2)
            {
                System.Net.ServicePointManager.UseNagleAlgorithm = true;
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.CheckCertificateRevocationList = true;
                ServiceSunatEmetac.billServiceClient wService;
                wService = new ServiceSunatEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    byte[] returnByte = wService.sendBill(filename + ".zip", byteArray,"");
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBILL", "-");
                    wService.Close();
                    pathCDR = PathDocumentosFacturacion + codigopaciente.ToString() + @"\RESULT\R-" + filename + ".zip";
                    System.IO.File.WriteAllBytes(pathCDR, returnByte);
                    new CobranzaDAO().insertarLogDocumento(filename, "RESPONDSENDBILL", "-");

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBILL", "-", ex.Code.Name);
                    throw new Exception(ex.Message + "\n" + ex.Code.Name + " - " + CobranzaDAO.getErrorSunat(ex.Code.Name.Split('.')[1].ToString()));
                }
            }
            else
            {
                throw new Exception("No esta registrado ningun Servicio de Sunat para esta Empresa");
            }

            return pathCDR;

        }

        /// <summary>
        /// ENVIO DE BAJA
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string sendSummary(int empresa, string filename,string numerodocumento)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;

            string pathMain = PathDocumentosFacturacion + "\\BAJA\\" + filename;
            byte[] byteArray = File.ReadAllBytes(pathMain);
            string ticket = "";
            if (empresa == 1)
            {
                ServiceSunatResocentro.billServiceClient wService = new ServiceSunatResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    ticket = wService.sendSummary(filename, byteArray,"");
                    new CobranzaDAO().insertarLogDocumento(numerodocumento, "SENDBAJA", ticket);
                    wService.Close();

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBAJA", ticket, ex.Code.Name);
                    throw new Exception(ex.Code.Name);
                }
            }
            else if (empresa == 2)
            {
                ServiceSunatEmetac.billServiceClient wService = new ServiceSunatEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    ticket = wService.sendSummary(filename, byteArray, "");
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBAJA", ticket);
                    wService.Close();

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBAJA", ticket, ex.Code.Name);
                    throw new Exception(ex.Code.Name);
                }
            }
            else
            {
                throw new Exception("No esta registrado ningun Servicio de Sunat para esta Empresa");
            }

            return ticket;
        }
        /// <summary>
        /// ENVIO DE RESUMEN DE BOLETAS
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string sendSummaryResumen(int empresa, string filename)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;

            string pathMain = PathDocumentosFacturacion + "\\RESUMEN\\" + filename;
            byte[] byteArray = File.ReadAllBytes(pathMain);
            string ticket = "";
            if (empresa == 1)
            {
                ServiceSunatResocentro.billServiceClient wService = new ServiceSunatResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    ticket = wService.sendSummary(filename, byteArray, "");
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDSUMMARY", ticket);
                    wService.Close();

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDSUMMARY", ticket, ex.Code.Name);
                    throw new Exception(ex.Code.Name);
                }
            }
            else if (empresa == 2)
            {
                ServiceSunatEmetac.billServiceClient wService = new ServiceSunatEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    ticket = wService.sendSummary(filename, byteArray, "");
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDSUMMARY", ticket);
                    wService.Close();

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDSUMMARY", ticket, ex.Code.Name);
                    throw new Exception(ex.Code.Name);
                }
            }
            else
            {
                throw new Exception("No esta registrado ningun Servicio de Sunat para esta Empresa");
            }

            return ticket;
        }
        /// <summary>
        /// GET STATUS DE BAJA
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public string getStatus(int empresa, string ticket, string filename)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;

            string estado = "";
            if (empresa == 1)
            {
                ServiceSunatResocentro.billServiceClient wService = new ServiceSunatResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    ServiceSunatResocentro.statusResponse returnstring = wService.getStatus(ticket);
                    estado = returnstring.statusCode;
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento("1-1-" + filename, "RESPONDBAJA", ticket, estado);
                    System.IO.File.WriteAllBytes(PathDocumentosFacturacion + "\\BAJA\\RESPUESTA-" + ticket + ".zip", returnByte);

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    throw new Exception(ex.Code.Name);
                }
            }
            else if (empresa == 2)
            {
                ServiceSunatEmetac.billServiceClient wService = new ServiceSunatEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    ServiceSunatEmetac.statusResponse returnstring = wService.getStatus(ticket);
                    estado = returnstring.statusCode;
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento("1-1-" + filename, "RESPONDBAJA", ticket, estado);
                    System.IO.File.WriteAllBytes(PathDocumentosFacturacion + "\\BAJA\\RESPUESTA-" + ticket + ".zip", returnByte);

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    throw new Exception(ex.Code.Name);
                }
            }
            else
            {
                throw new Exception("No esta registrado ningun Servicio de Sunat para esta Empresa");
            }

            return estado;

        }
        /// <summary>
        /// GET STATUS DE RESUMEN
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public string getStatusResumen(int empresa, string ticket, string filename)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;

            string estado = "";
            if (empresa == 1)
            {
                ServiceSunatResocentro.billServiceClient wService = new ServiceSunatResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    ServiceSunatResocentro.statusResponse returnstring = wService.getStatus(ticket);
                    estado = returnstring.statusCode;
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento("1-1-" + filename, "RESPONDSUMMARY", ticket, estado);
                    System.IO.File.WriteAllBytes(PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip", returnByte);

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    throw new Exception(ex.Code.Name);
                }
            }
            else if (empresa == 2)
            {
                ServiceSunatEmetac.billServiceClient wService = new ServiceSunatEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    ServiceSunatEmetac.statusResponse returnstring = wService.getStatus(ticket);
                    estado = returnstring.statusCode;
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento("1-1-" + filename, "RESPONDSUMMARY", ticket, estado);
                    System.IO.File.WriteAllBytes(PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip", returnByte);

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    throw new Exception(ex.Code.Name);
                }
            }
            else
            {
                throw new Exception("No esta registrado ningun Servicio de Sunat para esta Empresa");
            }

            return estado;

        }

        public string getCDR(int empresa,string nuumerodocumento,int codigopaciente,string filename)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;

            string[] numerodoc = filename.Split('-');

            string rucEmisor = numerodoc[0];
            string tipoDocumento = numerodoc[1];
            string serie = numerodoc[2];
            int correlativo = int.Parse(numerodoc[3].Replace(".zip",""));
            string pathCDR = PathDocumentosFacturacion + codigopaciente.ToString() + @"\RESULT\R-" + filename;

            if (empresa == 1)
            {
                ServiceSunatConsultaResocentro.billServiceClient wService = new ServiceSunatConsultaResocentro.billServiceClient();
                try
                {

                    wService.Open();
                    ServiceSunatConsultaResocentro.statusResponse returnstring = wService.getStatusCdr(rucEmisor, tipoDocumento, serie, correlativo);
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    System.IO.File.WriteAllBytes(pathCDR, returnByte);

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    throw new Exception(ex.Code.Name);
                }
            }
            else if (empresa == 2)
            {
                ServiceSunatConsultaEmetac.billServiceClient wService = new ServiceSunatConsultaEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    ServiceSunatConsultaEmetac.statusResponse returnstring = wService.getStatusCdr(rucEmisor, tipoDocumento, serie, correlativo);
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    System.IO.File.WriteAllBytes(pathCDR, returnByte);

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    throw new Exception(ex.Code.Name);
                }
            }
            else
            {
                throw new Exception("No esta registrado ningun Servicio de Sunat para esta Empresa");
            }

            return pathCDR;

        }
        public bool verificarStatusConexion(int empresa)
        {
            bool result = false;
            if (empresa == 1)
            {
                System.Net.ServicePointManager.UseNagleAlgorithm = true;
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.CheckCertificateRevocationList = true;

                ServiceSunatResocentro.billServiceClient wService;
                wService = new ServiceSunatResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    ServiceSunatResocentro.statusResponse returnstring = wService.getStatus("12345678913");
                    wService.Close();
                    result = true;
                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    result = false;
                    throw new Exception(ex.Code.Name);
                }
            }
            else if (empresa == 2)
            {
                System.Net.ServicePointManager.UseNagleAlgorithm = true;
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.CheckCertificateRevocationList = true;
                ServiceSunatEmetac.billServiceClient wService;
                wService = new ServiceSunatEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    ServiceSunatEmetac.statusResponse returnstring = wService.getStatus("12345678913");
                    wService.Close();
                    result = true;
                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    result = false;
                    throw new Exception(ex.Code.Name);
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}
