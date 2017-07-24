using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Caja.impresion;
using System;
using System.Collections.Generic;
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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmUploadFacturas.xaml
    /// </summary>
    public partial class frmUploadFacturas : Window
    {
        MySession session;
        public frmUploadFacturas()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            cbosucursal.ItemsSource = new UtilDAO().getEmpresa();
            cbosucursal.SelectedValuePath = "codigounidad";
            cbosucursal.DisplayMemberPath = "nombre";
            cbosucursal.SelectedIndex = 0;
            preload();
        }

        private void preload()
        {
            List<DocumentosCancelacion> lista = new List<DocumentosCancelacion>();
            lista.Add(new DocumentosCancelacion() { numero_doc = "", total = 0.0, resultado = "" });
            radGridView1.ItemsSource = lista;
        }
        private void radGridView1_PastingCellClipboardContent(object sender, Telerik.Windows.Controls.GridViewCellClipboardEventArgs e)
        {
            bool isCorrecto = true;
            try
            {
                if (e.Cell.Column.UniqueName == "doc")
                {
                    var num_doc = e.Value.ToString().Split('-');
                    if (num_doc.Count() == 2)
                    {
                        if (num_doc[0].Length < 3 || num_doc[0].Length > 4)
                        {
                            isCorrecto = false;
                        }
                        else
                        {
                            var serie = num_doc[0].Length == 4 ? num_doc[0].Substring(1) : num_doc[0];
                            var new_num = serie + "-" + cadenasinceros(num_doc[1]);
                            e.Value = new_num.ToUpper();
                        }
                    }
                    else
                    {
                        isCorrecto = false;
                    }
                }
                if (!isCorrecto)
                {
                    if (e.Cell.Column.UniqueName == "doc")
                    {
                        e.Value = "ERROR";
                        //throw new Exception("El N° de Documento no tiene el formato adecuado");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void radGridView1_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            DocumentosCancelacion documento = e.NewData as DocumentosCancelacion;
            if (documento != null)
            {
                var num_doc = documento.numero_doc.ToString().Split('-');
                if (num_doc.Count() == 2)
                {
                    if (num_doc[0].Length < 3 || num_doc[0].Length > 4)
                    {
                        documento.resultado = "ERROR de N° de Documento";
                    }
                    else
                    {
                        var serie = num_doc[0].Length == 4 ? num_doc[0].Substring(1) : num_doc[0];
                        var new_num = serie + "-" + cadenasinceros(num_doc[1]);
                        documento.numero_doc = new_num.ToUpper();
                    }
                }
                else
                {
                    documento.resultado = "ERROR de N° de Documento";
                }
            }

        }
        private string cadenasinceros(string valor)
        {
            int j;

            for (j = 0; j < valor.Length; j++)
            {
                if (valor.Substring(j, 1) != "0")
                {
                    valor = valor.Substring(j, valor.Length - j);
                    break;
                }

            }

            return valor;
        }


        private void radRibbonButton1_Click(object sender, RoutedEventArgs e)
        {
            foreach (DocumentosCancelacion item in radGridView1.Items)
            {
                var documento = (new CobranzaDAO()).consultarDocumentoxCodigo(item.numero_doc, cbosucursal.SelectedValue.ToString(), item.total.ToString());

                item.fec_recepcion = documento.fec_recepcion;
                item.fec_emision = documento.fec_emision;
                item.resultado = documento.resultado;
            }
            radGridView1.Rebind();
        }

        private void radRibbonButton2_Click(object sender, RoutedEventArgs e)
        {
            List<DocumentosCancelacion> lista = new List<DocumentosCancelacion>();
            foreach (DocumentosCancelacion item in radGridView1.Items)
            {
                if (item.resultado == "OK")
                {
                    (new CobranzaDAO()).updateDocumentoEstado(true, item.numero_doc, item.fec_pagp, item.referencia);
                    lista.Add(item);
                }
            }
            MessageBox.Show("Exito", "EXITO", MessageBoxButton.OK, MessageBoxImage.Information);
            if (lista.Count > 0)
                imprimir(lista);
        }

        private void imprimir(List<DocumentosCancelacion> item)
        {
            try
            {

                frmPlanillaCobranza gui = new frmPlanillaCobranza();
                gui.cargarGUI(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void s(object sender, Telerik.Windows.Controls.GridViewDeletingEventArgs e)
        {
            //MessageBox.Show("s");
        }


    }
}
