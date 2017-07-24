using Microsoft.AspNet.SignalR.Client;
using Oracle.ManagedDataAccess.Client;
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Caja.impresion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Resocentro_Desktop.Interfaz.Sistemas
{
    /// <summary>
    /// Lógica de interacción para frmTest.xaml
    /// </summary>
    public partial class frmTest : Window
    {
        MySession session;
        public frmTest()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            try
            {

                string conexion = new CobranzaDAO().getConexionOracle(1);

                using (OracleConnection connection = new OracleConnection(conexion))
                {
                    connection.Open();
                    OracleCommand command = new OracleCommand();
                    OracleTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    try
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandType = CommandType.Text;

                        command.CommandText = "select sysdate from dual";
                        command.Prepare();
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        result = false;
                    }
                    finally
                    {
                        transaction.Dispose();
                        command.Dispose();
                        connection.Dispose();
                        connection.Close();
                    }
                }

                if (result)
                    MessageBox.Show("Conexion correcta al ORACLE");
                else
                    MessageBox.Show("No se pudo conectar al ORACLE");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("No se pudo conectar al ORACLE");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Conexion correcta al SQL");

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                frmTicketControl tkt1 = new frmTicketControl();
                tkt1.cargarGUI(session, new DocumentoSunat());
                tkt1.printTicket();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal void cargarGUI(MySession session)
        {
            this.session = session;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process printJob = new System.Diagnostics.Process();
                printJob = new System.Diagnostics.Process();
                printJob.StartInfo.FileName = @"\\serverweb\Facturacion\testprint.pdf";
                printJob.StartInfo.UseShellExecute = true;
                printJob.StartInfo.Verb = "printto";
                printJob.StartInfo.CreateNoWindow = true;
                printJob.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                printJob.StartInfo.Arguments = "FACTURA";
                printJob.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                var sunat = new ServiceSunat();

                if (sunat.verificarStatusConexion(1))
                    MessageBox.Show("Si hay Conexion a SUNAT con datos de Resocentro");
                else
                    MessageBox.Show("No hay Conexion a SUNAT con datos de Resocentro");

                if (sunat.verificarStatusConexion(2))
                    MessageBox.Show("Si hay Conexion a SUNAT con datos de Emetac");
                else
                    MessageBox.Show("No hay Conexion a SUNAT con datos de Emetac");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private  void Button_Click_5(object sender, RoutedEventArgs e)
        {
            using (ServiceResocentro.WSAsyncCallSeguimientoClient service = new ServiceResocentro.WSAsyncCallSeguimientoClient())
            {
                bool resultado= service.ActualizarListaEncuesta();
                
            }
        }


    }


}