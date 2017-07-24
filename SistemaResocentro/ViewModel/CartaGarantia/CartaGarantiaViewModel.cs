using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SistemaResocentro.Models;

namespace SistemaResocentro.ViewModel
{
    public class CartaGarantiaViewModel
    {
        public CARTAGARANTIA carta { get; set; }
        public string estudios { get; set; }
        public int num_proforma { get; set; }
        public List<Estudios_proforma> list_estudios { get; set; }
        public string Adjunto { get; set; }
        public string idAdjunto { get; set; }
        public bool? isRevisada { get; set; }
    }
    public class List_VisorCarta {
        public DateTime tramite { get; set; }

        public int idPaciente { get; set; }

        public string paciente { get; set; }

        public string estudio { get; set; }

        public string telefono { get; set; }

        public string aseguradora { get; set; }

        public float cobertura { get; set; }

        public string estado { get; set; }

        public string idcarta { get; set; }

        public string usu_reg { get; set; }

        public DateTime? fec_update { get; set; }

        public string usu_update { get; set; }

        public string usu_cita { get; set; }

        public DateTime? fec_aprueba { get; set; }

        public string obs { get; set; }
    }
    public class Carta_list_Revisar
    {
        public string id_carta { get; set; }
        public DateTime fechaTramite { get; set; }
        public DateTime? fec_cita { get; set; }
        public string estado { get; set; }
        public string aseguradora { get; set; }
        public float cobertura { get; set; }
        public string num_carta { get; set; }
        public string idadjunto { get; set; }
        public int idpaciente { get; set; }
        public string paciente { get; set; }
        public int? numerocita { get; set; }
        public bool iscitado { get; set; }
        public bool isAdjunto { get; set; }
        public string user_tramita { get; set; }
        public string user_update { get; set; }
        public DateTime? fec_update { get; set; }
        public string user_revisa { get; set; }
        public DateTime? fec_revisa { get; set; }
    }
}