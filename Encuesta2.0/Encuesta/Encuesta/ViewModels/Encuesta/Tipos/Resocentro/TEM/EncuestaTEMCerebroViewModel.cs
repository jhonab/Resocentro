using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    
    public class EncuestaTEMCerebroViewModel
    {
        public EXAMENXATENCION _examen { get; set; }
        public PACIENTE _paciente { get; set; }
        public string modalidad { get; set; }

        public bool? p1 { get; set; }
        public bool p1_1 { get; set; }
        public bool p1_2 { get; set; }
        public bool p1_3 { get; set; }
        public string p1_4 { get; set; }

        public bool? p2 { get; set; }
        public bool? p2_1 { get; set; }

        public bool? p3 { get; set; }
        public bool? p3_1 { get; set; }
        public bool? p3_1_1 { get; set; }
        public bool? p3_1_2 { get; set; }
        public bool p3_1_3 { get; set; }
        public bool p3_1_4 { get; set; }
        public bool p3_1_5 { get; set; }

        public bool? p4 { get; set; }
        public bool? p4_1 { get; set; }

        public bool? p5 { get; set; }
        public bool p5_1 { get; set; }
        public bool p5_2 { get; set; }
        public bool p5_3 { get; set; }
        public bool p5_4 { get; set; }
        public string p5_4_1 { get; set; }

        public bool? p6 { get; set; }
        public bool p6_1 { get; set; }
        public bool p6_2 { get; set; }
        public bool p6_3 { get; set; }
        public bool p6_4 { get; set; }
        public bool p6_5 { get; set; }
        public bool p6_6 { get; set; }
        public bool p6_7 { get; set; }
        public string p6_7_1 { get; set; }

        public string p7 { get; set; }

        public int? p8 { get; set; }
        public int p8_1 { get; set; }

        public bool? p9 { get; set; }
        public string p9_1 { get; set; }

        public bool? p10 { get; set; }
        public string p10_1 { get; set; }

        public bool? p11 { get; set; }
        public int? p11_1 { get; set; }
        public int? p11_2 { get; set; }
        public int p11_3 { get; set; }
        public string p11_4 { get; set; }

        public bool? p12 { get; set; }
        //RESONANCIA
        public bool p12_1 { get; set; }
        public int p12_1_1 { get; set; }
        public int? p12_1_2 { get; set; }
        public int? p12_1_3 { get; set; }
        public int p12_1_4 { get; set; }
        public string p12_1_5 { get; set; }

        //TOMOGRAFIA
        public bool p12_2 { get; set; }
        public int p12_2_1 { get; set; }
        public int? p12_2_2 { get; set; }
        public int? p12_2_3 { get; set; }
        public int p12_2_4 { get; set; }
        public string p12_2_5 { get; set; }

        public string p13A { get; set; }
        public bool p13A_1 { get; set; }

        public string p13B { get; set; }
        public bool p13B_1 { get; set; }

        public string p14 { get; set; }

        public bool p15_1 { get; set; }
        public bool p15_2 { get; set; }
        public bool p15_3 { get; set; }
        public bool p15_4 { get; set; }
        public bool p15_5 { get; set; }
      
        public string p16 { get; set; }
        public int p16_1 { get; set; }

        public string sexo { get; set; }

        public int equipoAsignado { get; set; }

        public int numeroexamen { get; set; }

        public EncuestaDetalleViewModel _encuesta { get; set; }
    }

    public class ELCerebroTEM
    {
        public int numeroexamen { get; set; }
        public DateTime fec_reg { get; set; }
        public string usu_reg { get; set; }

        public bool p1 { get; set; }
        public string p1a { get; internal set; }
        public string p1a31 { get; set; }

        public void p1aSet(bool _1, bool _2, bool _3)
        {
            p1a = "";
            p1a += _1 ? "1" : "0";
            p1a += _2 ? "1" : "0";
            p1a += _3 ? "1" : "0";
        }


        public bool p2 { get; set; }
        public bool p2a { get; set; }

        public bool p3 { get; set; }
        public bool p3a { get; set; }
        public bool p3a1 { get; set; }
        public bool p3a2 { get; set; }
        public string p3a3 { get; internal set; }
        public void p3a3Set(bool _1, bool _2, bool _3)
        {
            p3a3 = "";
            p3a3 += _1 ? "1" : "0";
            p3a3 += _2 ? "1" : "0";
            p3a3 += _3 ? "1" : "0";
        }
        public bool p4 { get; set; }
        public bool p4a { get; set; }

        public bool p5 { get; set; }
        public string p5a { get; internal set; }
        public void p5aSet(bool _1, bool _2, bool _3, bool _4)
        {
            p5a = "";
            p5a += _1 ? "1" : "0";
            p5a += _2 ? "1" : "0";
            p5a += _3 ? "1" : "0";
            p5a += _4 ? "1" : "0";
        }
        public string p5a41 { get; set; }

        public bool p6 { get; set; }
        public string p6a { get; internal set; }
        public void p6aSet(bool _1, bool _2, bool _3,
            bool _4, bool _5, bool _6, bool _7)
        {
            p6a = "";
            p6a += _1 ? "1" : "0";
            p6a += _2 ? "1" : "0";
            p6a += _3 ? "1" : "0";
            p6a += _4 ? "1" : "0";
            p6a += _5 ? "1" : "0";
            p6a += _6 ? "1" : "0";
            p6a += _7 ? "1" : "0";
        }
        public string p6a71 { get; set; }

        public string p7 { get; set; }

        public string p8 { get; set; }
        public void p8Set(string _1, string _2)
        {
            p8 = string.Format("{0}{1}", _1, _2);
        }
        public bool p9 { get; set; }
        public string p9a1 { get; set; }

        public bool p10 { get; set; }
        public string p10a { get; internal set; }
        public void p10aSet(bool _1, bool _2, bool _3,
            bool _4, bool _5, bool _6, bool _7)
        {
            p10a = "";
            p10a += _1 ? "1" : "0";
            p10a += _2 ? "1" : "0";
            p10a += _3 ? "1" : "0";
            p10a += _4 ? "1" : "0";
            p10a += _5 ? "1" : "0";
            p10a += _6 ? "1" : "0";
            p10a += _7 ? "1" : "0";
        }
        public string p10a71 { get; set; }

        public bool p11 { get; set; }
        public int p11a { get; set; }
        public string p11b { get; internal set; }
        public void p11bSet(string _1, string _2)
        {
            p11b = string.Format("{0}{1}", _1, _2);
        }
        public string p11c { get; set; }

        public bool p12 { get; set; }
        public bool p12aa { get; set; }
        public bool p12bb { get; set; }
        public string p12a { get; set; }
        public string p12a41 { get; internal set; }
        public void p12a41Set(int val)
        {
            p12a41 = val.ToString();

        }
        public string p12b { get; internal set; }
        public void p12bSet(string _1, string _2)
        {
            p12b = string.Format("{0}{1}", _1, _2);
        }
        public string p12c { get; set; }
        public string p12d { get; set; }
        public string p12d41 { get; internal set; }
        public void p12d41Set(int val)
        {
            p12d41 = val.ToString();

        }
        public string p12e { get; set; }
        public void p12eSet(string _1, string _2)
        {
            p12e = string.Format("{0}{1}", _1, _2);
        }
        public string p12f { get; set; }

        public string p13A { get; set; }
        public bool p13Acheck { get; set; }
        public string p13B { get; set; }
        public bool p13Bcheck { get; set; }

        public string p14 { get; set; }

        public bool p15_1{ get; set; }
        public bool p15_2 { get; set; }
        public bool p15_3 { get; set; }
        public bool p15_4 { get; set; }
        public bool p15_5 { get; set; }
     
        public string p16 { get; set; }
        public void p16Set(string val)
        {
            p16 = val.ToString();

        }
        public int? p16_1 { get; set; }

    }
}