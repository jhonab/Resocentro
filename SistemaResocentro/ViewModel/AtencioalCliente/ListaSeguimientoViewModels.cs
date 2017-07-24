using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class ListaSeguimientoViewModels
    {
        public bool adjunto { get; set; }

        public DateTime fecha { get; set; }

        public int nexamen { get; set; }

        public string estado { get; set; }

        public string sucursal { get; set; }

        public string tipo_paciente { get; set; }

        public string tipo_documento { get; set; }

        public string ndocumento { get; set; }

        public string paciente { get; set; }

        public string sexo { get; set; }

        public DateTime nacimiento { get; set; }

        public bool isProtocolo { get; set; }

        public int edad { get; set; }

        public double peso { get; set; }

        public double talla { get; set; }

        public string aseguradora { get; set; }

        public string medico { get; set; }

        public double cobertura { get; set; }

        public string estudio { get; set; }

        public string documentos { get; set; }

        public Errores_SegViewModel error { get; set; }

        public string observacion { get; set; }

        public string codigocarta { get; set; }

        public bool carta { get; set; }

        public string clinica { get; set; }
    }
    public class ListaReporteAtencionesViewModel
    {

        public DateTime fecha { get; set; }

        public string apellidos { get; set; }

        public string nombres { get; set; }

        public int examen { get; set; }

        public string estudio { get; set; }

        public string estado { get; set; }

        public string aseguradora { get; set; }

        public bool sedacion { get; set; }

        public bool contraste { get; set; }

        public string documentos { get; set; }
    }
    public class Errores_SegViewModel
    {
        public int examen { get; set;}
        public string peso { get; set; }
        public bool isCorrectpeso { get; set; }
        public string medico { get; set; }
        public bool isCorrectmedico { get; set; }
        public string documento { get; set; }
        public bool isCorrectdocumento { get; set; }
        public string carta { get; set; }
        public bool isCorrectcarta { get; set; }
        public string adjunto { get; set; }
        public bool isCorrectadjunto { get; set; }
        public string edad { get; set; }
        public bool isCorrectedad { get; set; }
        public string talla { get; set; }
        public bool isCorrecttalla { get; set; }
        public string dni { get; set; }
        public bool isCorrectdni { get; set; }
        public string obs { get; set; }

        public string adjuntocarta { get; set; }
        public bool isCorrectadjuntocarta { get; set; }
        public string clinica { get; set; }
        public bool isCorrectclinica { get; set; }

    }
    
}