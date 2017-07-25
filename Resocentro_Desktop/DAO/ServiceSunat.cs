using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    public class ServiceSunat
    {

        public string sendBill(int empresa, int codigopaciente, string filename)
        {


            string pathMain = Tool.PathDocumentosFacturacion + codigopaciente.ToString() + "\\ZIP\\" + filename + ".zip";           
            byte[] byteArray = File.ReadAllBytes(pathMain);
            string pathCDR = "";
            if (empresa == 1)
            {
                System.Net.ServicePointManager.UseNagleAlgorithm = true;
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.CheckCertificateRevocationList = true;

                SunatServiceResocentro.billServiceClient wService;
                wService = new SunatServiceResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    byte[] returnByte = wService.sendBill(filename + ".zip", byteArray,"");
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBILL", "-");
                    wService.Close();
                    pathCDR = Tool.PathDocumentosFacturacion + codigopaciente.ToString() + @"\RESULT\R-" + filename + ".zip";
                    System.IO.File.WriteAllBytes(pathCDR, returnByte);
                    new CobranzaDAO().insertarLogDocumento(filename, "RESPONDSENDBILL", "-");

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBILL", "-", ex.Code.Name);
                    throw new Exception(ex.Message + "\n" + ex.Code.Name + " - " + CobranzaDAO.getErrorSunat(ex.Code.Name.ToString()));
                }
            }
            else if (empresa == 2)
            {
                System.Net.ServicePointManager.UseNagleAlgorithm = true;
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.CheckCertificateRevocationList = true;
                SunatServiceEmetac.billServiceClient wService;
                wService = new SunatServiceEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    byte[] returnByte = wService.sendBill(filename + ".zip", byteArray,"");
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBILL", "-");
                    wService.Close();
                    pathCDR = Tool.PathDocumentosFacturacion + codigopaciente.ToString() + @"\RESULT\R-" + filename + ".zip";
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
        public string sendSummary(int empresa, string filename)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;

            string pathMain = Tool.PathDocumentosFacturacion + "\\BAJA\\" + filename;
            byte[] byteArray = File.ReadAllBytes(pathMain);
            string ticket = "";
            if (empresa == 1)
            {
                SunatServiceResocentro.billServiceClient wService = new SunatServiceResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    ticket = wService.sendSummary(filename, byteArray,"");
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBAJA", ticket);
                    wService.Close();

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento(filename, "SENDBAJA", ticket,ex.Code.Name);
                    throw new Exception(ex.Code.Name);
                }
            }
            else if (empresa == 2)
            {
                SunatServiceEmetac.billServiceClient wService = new SunatServiceEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    ticket = wService.sendSummary(filename, byteArray,"");
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

            string pathMain = Tool.PathDocumentosFacturacion + "\\RESUMEN\\" + filename;
            byte[] byteArray = File.ReadAllBytes(pathMain);
            string ticket = "";
            if (empresa == 1)
            {
                SunatServiceResocentro.billServiceClient wService = new SunatServiceResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    ticket = wService.sendSummary(filename, byteArray,"");
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
                SunatServiceEmetac.billServiceClient wService = new SunatServiceEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    ticket = wService.sendSummary(filename, byteArray,"");
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
                SunatServiceResocentro.billServiceClient wService = new SunatServiceResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    SunatServiceResocentro.statusResponse returnstring = wService.getStatus(ticket);
                    estado = returnstring.statusCode;
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento("1-1-"+filename, "RESPONDBAJA", ticket,estado);
                    System.IO.File.WriteAllBytes(Tool.PathDocumentosFacturacion + "\\BAJA\\RESPUESTA-" + ticket + ".zip", returnByte);

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    throw new Exception(ex.Code.Name);
                }
            }
            else if (empresa == 2)
            {
                SunatServiceEmetac.billServiceClient wService = new SunatServiceEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    SunatServiceEmetac.statusResponse returnstring = wService.getStatus(ticket);
                    estado = returnstring.statusCode;
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento("1-1-" + filename, "RESPONDBAJA", ticket, estado);
                    System.IO.File.WriteAllBytes(Tool.PathDocumentosFacturacion + "\\BAJA\\RESPUESTA-" + ticket + ".zip", returnByte);

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
        public string getStatusResumen(int empresa, string ticket,string filename)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;

            string estado = "";
            if (empresa == 1)
            {
                SunatServiceResocentro.billServiceClient wService = new SunatServiceResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    SunatServiceResocentro.statusResponse returnstring = wService.getStatus(ticket);
                    estado = returnstring.statusCode;
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento("1-1-"+filename , "RESPONDSUMMARY", ticket,estado);
                    System.IO.File.WriteAllBytes(Tool.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip", returnByte);

                }
                catch (System.ServiceModel.FaultException ex)
                {
                    wService.Close();
                    throw new Exception(ex.Code.Name);
                }
            }
            else if (empresa == 2)
            {
                SunatServiceEmetac.billServiceClient wService = new SunatServiceEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    SunatServiceEmetac.statusResponse returnstring = wService.getStatus(ticket);
                    estado = returnstring.statusCode;
                    byte[] returnByte = returnstring.content;
                    wService.Close();
                    new CobranzaDAO().insertarLogDocumento("1-1-" + filename, "RESPONDSUMMARY", ticket, estado);
                    System.IO.File.WriteAllBytes(Tool.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip", returnByte);

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

        public string getCDR(DocumentoSunat item)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;

            string[] numerodoc = item.numeroDocumentoSUNAT.Split('-');
            if (numerodoc.Count() != 2)
                throw new Exception("No tiene un numero correcto de documento");

            string serie = numerodoc[0];
            int correlativo = int.Parse(numerodoc[1]);
            string pathCDR = Tool.PathDocumentosFacturacion + item.codigopaciente.ToString() + @"\RESULT\R-" + (item.rucEmisor + "-" + item.tipoDocumento.ToString("D2") + "-" + item.numeroDocumentoSUNAT) + ".zip";
            
            if (item.empresa == 1)
            {
                ServiceSunatRESOConsulta.billServiceClient wService = new ServiceSunatRESOConsulta.billServiceClient();
                try
                {

                    wService.Open();
                    ServiceSunatRESOConsulta.statusResponse returnstring = wService.getStatusCdr(item.rucEmisor, item.tipoDocumento.ToString("D2"), serie, correlativo);
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
            else if (item.empresa == 2)
            {
                ServiceSunatEMEConsulta.billServiceClient wService = new ServiceSunatEMEConsulta.billServiceClient();
                try
                {
                    wService.Open();
                    ServiceSunatEMEConsulta.statusResponse returnstring = wService.getStatusCdr(item.rucEmisor, item.tipoDocumento.ToString("D2"), serie, correlativo);
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

                SunatServiceResocentro.billServiceClient wService;
                wService = new SunatServiceResocentro.billServiceClient();
                try
                {
                    wService.Open();
                    SunatServiceResocentro.statusResponse returnstring = wService.getStatus("12345678913");
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
                SunatServiceEmetac.billServiceClient wService;
                wService = new SunatServiceEmetac.billServiceClient();
                try
                {
                    wService.Open();
                    SunatServiceEmetac.statusResponse returnstring = wService.getStatus("12345678913");
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
