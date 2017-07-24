using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.Entitys
{
    class CartaEntity
    {
    }
    public partial class VisorCarta
    {
        public DateTime Tramite { get; set; }
        public int ID { get; set; }
        public string Paciente { get; set; }
        public string Estudio { get; set; }
        public string Telefono { get; set; }
        public string Aseguradora { get; set; }
        public string Clinica { get; set; }
        public double Cob { get; set; }
        public string Estado { get; set; }
        public DateTime FEC_APROB { get; set; }
        public string Observacion { get; set; }
        public string Codigo { get; set; }
        public string Usuario { get; set; }
        public DateTime fec_update { get; set; }
        public string user_update { get; set; }
        public string user_cita { get; set; }
    }
    public class RevisionCarta
    {
        public string codigocartagarantia { get; set; }
        public DateTime? fec_cita { get; set; }
        public string estado { get; set; }
        public string aseguradora { get; set; }
        public string codigocartagarantia2 { get; set; }
        public int idpaciente { get; set; }
        public string paciente { get; set; }
        public bool iscitado { get; set; }
        public bool isAdjunto { get; set; }
        public string user_tramita { get; set; }
        public string user_update { get; set; }
        public DateTime? fec_update { get; set; }


        public string user_proforma { get; set; }

        public string user_cita { get; set; }
    }
    public class Sedacion_Carta
    {
        public DateTime fecha { get; set; }

        public int codigo { get; set; }

        public string paciente { get; set; }

        public string estudio { get; set; }

        public string estado { get; set; }

        public int codigopaciente { get; set; }

        public bool revisado { get; set; }

        public string compania { get; set; }

        public string observCita { get; set; }

        public string documentos { get; set; }
    }
}
