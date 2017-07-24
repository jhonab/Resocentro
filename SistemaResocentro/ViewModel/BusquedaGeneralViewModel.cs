using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class BusquedaGeneralViewModel
    {
        public List<PACIENTE> lstPacientes { get; set; }
        public string apellidos { get; set; }
        public string nombres { get; set; }

        public List<ListaCitas_BG> lstCita { get; set; }

        public List<ListaAdmision_BG> lstAdmision { get; set; }

        public List<ListaCartas_BG> lstcarta { get; set; }

        public List<ListaDocumento_BG> lstdocumento { get; set; }

        public List<ListaAdquisicion_BG> lstAdquisicion { get; set; }
    }
    public class ListaCitas_BG
    {

        public string estado { get; set; }

        public bool sedacion { get; set; }

        public int num_cita { get; set; }

        public DateTime fecha { get; set; }

        public string estudio { get; set; }

        public string precio { get; set; }

        public string cobertura { get; set; }

        public string equipo { get; set; }

        public string usuario { get; set; }

        public string obs { get; set; }

        public bool vip { get; set; }

        public string carta { get; set; }

        public string clinica { get; set; }
    }

    public class ListaAdmision_BG { public string estado { get; set; }
    public string ubicacion { get; set; }

    public DateTime fecha { get; set; }

    public int atencion { get; set; }

    public int examen { get; set; }

    public string estudio { get; set; }

    public string aseguradora { get; set; }

    public string prioridad { get; set; }

    public string medico { get; set; }

    public string turno { get; set; }

    public string usuario { get; set; }

    public int cita { get; set; }
    }
    public class ListaCartas_BG { public string estado { get; set; }
    public DateTime fecha { get; set; }

    public string id { get; set; }

    public string id2 { get; set; }

    public string aseguradora { get; set; }

    public string cobertura { get; set; }

    public int cita { get; set; }

    public string isadjunto { get; set; }

    public string idpaciente { get; set; }
    }
    public class ListaDocumento_BG { public string estado { get; set; }
    public string empresa { get; set; }

    public DateTime emision { get; set; }

    public DateTime pago { get; set; }

    public string documento { get; set; }

    public string numero { get; set; }

    public string total { get; set; }

    public string cita { get; set; }
    }
    public class ListaAdquisicion_BG { public int examen { get; set; }
    public string estudio { get; set; }

    public int placas { get; set; }

    public string sedacion { get; set; }

    public string contraste { get; set; }

    public DateTime fecha { get; set; }

    public string tecnologo { get; set; }

    public string cita { get; set; }

    public string equipo { get; set; }
    }
}