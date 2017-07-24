using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Encuesta.Models;

namespace Encuesta.ViewModels.Encuesta
{
    public class EncuestaExamenViewModel
    {
        public int num_examen { get; set; }
        public int? tipo_encuesta { get; set; }
        public bool isFinEncuesta { get; set; }
        public string paciente { get; set; }
        public string sede { get; set; }
        public DateTime hora_cita { get; set; }
        public DateTime hora_admision { get; set; }
        public string min_transcurri { get; set; }
        public string condicion { get; set; }
        public string nom_estudio { get; set; }
        public string nom_equipo { get; set; }
        public bool isSedacion { get; set; }
        public bool isVIP { get; set; }
        public string turnmedico { get; set; }
        public bool isAsignado { get; set; }
        public bool docEscan { get; set; }
        public bool docCarta { get; set; }
        public bool isEncuesta { get; set; }
        public string modalidad { get; set; }
        public string sexo { get; set; }
        public int numeroatencion { get; set; }
        public bool p1 { get; set; }
        public bool p2 { get; set; }
        public bool CheckBox3 { get; set; }
        public bool CheckBox4 { get; set; }
        public bool CheckBox5 { get; set; }
        public bool CheckBox6 { get; set; }
        public bool CheckBox7 { get; set; }
        public bool CheckBox8 { get; set; }
        public bool CheckBox9 { get; set; }
        public bool CheckBox10 { get; set; }
        public bool CheckBox11 { get; set; }
        public bool CheckBox12 { get; set; }
        public bool CheckBox13 { get; set; }
        public bool CheckBox14 { get; set; }
        public bool CheckBox15 { get; set; }
        public bool CheckBox16 { get; set; }
        public bool CheckBox17 { get; set; }
        public bool CheckBox18 { get; set; }
        public bool CheckBox19 { get; set; }
        public bool CheckBox20 { get; set; }
        public bool CheckBox21 { get; set; }
        public string txtOtros { get; set; }
        public bool CheckBox30 { get; set; }
        public bool CheckBox31 { get; set; }
        public string txtNumeroConsentimiento { get; set; }
        public bool isAutorizado { get; set; }
        public int tipoAutorizacion { get; set; }
        public string detalleAutorizacion { get; set; }
        public string usu_reg { get; set; }
        public DateTime fec_reg_ini { get; set; }
        public DateTime fec_reg_fin { get; set; }        

        public Models.Encuesta encuesta { get; set; }
        public EXAMENXATENCION _examen { get; set; }
        public PACIENTE _paciente { get; set; }


        public int? estado { get; set; }

        public bool isAyer { get; set; }

        public bool? isHospitalizado { get; set; }
    }
}