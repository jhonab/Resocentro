using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.Entitys
{
    class DocumentoEntity
    {
        public string Empresa { get; set; }

        public string Sucursal { get; set; }

        public string E { get; set; }

        public string Documento { get; set; }

        public string Poliza { get; set; }

        public string tipo { get; set; }

        public string Titular { get; set; }

        public string Moneda { get; set; }

        public double Subtotal { get; set; }

        public double IGV { get; set; }

        public double Total { get; set; }

        public double TC { get; set; }

        public string Emision { get; set; }

        public string Concepto { get; set; }

        public string Carta { get; set; }

        public string Cob { get; set; }

        public string Usuario { get; set; }

        public string pathfile { get; set; }

        public string RucAlterno { get; set; }
    }
    class DetalleDocumentoEntity
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public double Unitario { get; set; }
        public double Promociones { get; set; }
        public double Carta { get; set; }
        public double Cortesia { get; set; }
        public double Total { get; set; }


        string _tipoIgv;
        public string TipoIGV {
            get
            {
                return Enum.GetName(typeof(TIPO_IGV), int.Parse(_tipoIgv));
            }
             set
            {
                this._tipoIgv = value;
            }
        }
    }
    class FormaPagoEntity
    {
        public int ID { get; set; }

        public string Modalidad { get; set; }

        public string Tarjeta { get; set; }

        public string Descripcion { get; set; }

        public double Monto { get; set; }

        public string Fecha { get; set; }

        public string Usuario { get; set; }

        public string referencia { get; set; }

        public string moneda { get; set; }
    }
    class LibroCajaEntity
    {
        public string Atencion { get; set; }
        public string Aseguradora { get; set; }
        public string Pago { get; set; }
        public double Descuento { get; set; }
        public double SubTotal { get; set; }
        public double IGV { get; set; }
        public double Total { get; set; }
        public double Neto { get; set; }
        public double Saldo { get; set; }
        public bool Cortesia { get; set; }
        public string Fecha { get; set; }
        public string Usuario { get; set; }
        public string Entrega { get; set; }
        public string Lugar { get; set; }
    }
    class CobranzaCiaEntity
    {
        public string E { get; set; }

        public string Atencion { get; set; }

        public string Emision { get; set; }

        public string Lote { get; set; }

        public bool Trama { get; set; }

        public string Usuario { get; set; }
    }
}

