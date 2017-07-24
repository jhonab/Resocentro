using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.Entitys
{
    class CitaEntity
    {
    }
    public class DetalleCita
    {

        public int codigoexamencita { get; set; }

        public DateTime fecha { get; set; }

        public string nombreestudio { get; set; }

        public string codigoestudio { get; set; }

        public double preciobruto { get; set; }

        public double descuento { get; set; }

        public double cobertura { get; set; }
        public double precioneto { get { return preciobruto + descuento; } }

        public int codigomoneda { get; set; }
        public string moneda { get { return codigomoneda == 1 ? "Soles" : codigomoneda == 2 ? "Dolares" : ""; } }
        public string simbolomoneda { get { return codigomoneda == 1 ? "S/." : codigomoneda == 2 ? "$" : ""; } }


        public int codigomodalidad { get; set; }

        public int codigocompaniaseguro { get; set; }
    }
    public class CartaxCitar
    {
        public string codigocarta { get; set; }
        public string paciente { get; set; }
        public DateTime inicio { get; set; }
        public DateTime aprobacion { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }
        public int llamadas { get; set; }
        public string aseguradora { get; set; }
        public string estado { get; set; }
        public string adjunto { get; set; }
        public int idpaciente { get; set; }
        public bool isrevisada { get; set; }
        public string imgRevisada { get { return isrevisada ? "../img/check.png" : ""; } }

        public int codigoclinica { get; set; }

        public double monto { get; set; }

        public string comentarios { get; set; }

        public string cmp { get; set; }

        public int codigocompaniaseguro { get; set; }

        public string ruc { get; set; }

        public double cobertura { get; set; }
        public System.Windows.Visibility isVisibleTel { get { return telefono=="0" || telefono==""?System.Windows.Visibility.Collapsed:System.Windows.Visibility.Visible;} }
        public System.Windows.Visibility isVisibleCel { get { return celular == "0" || celular == "" ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible; } }
    }
    public class RegistroLlamada
    {
        public DateTime fecha{ get; set; }
        public string telefono { get; set; }
        public string usuario { get; set; }
        public string mensaje { get; set; }
    }

    public class Turno_Horario
    {
        public int codigohorario { get; set; }
        public DateTime hora { get; set; }
        public bool isBloqueado { get; set; }
        public string paciente { get; set; }
        public string estudio{ get; set; }
        public string turno { get; set; }
    }
    public class EmpresaSucursal
    {
        public int codigounidad { get; set; }
        public int codigosucursal { get; set; }
        public int codigo { get; set; }
        public int codigoShort { get; set; }
        public string empresa { get; set; }
        public string empresaShort { get; set; }
        public string sucursal { get; set; }
        public string sucursalShort { get; set; }
        public string concatenado { get; set; }
        public string concatenadoShort { get; set; }
    }
    public class EquipoEmpresaSucursal
    {
        public int codigounidad { get; set; }
        public int codigosucursal { get; set; }
        public int codigo { get; set; }
        public int codigoShort { get; set; }
        public string empresa { get; set; }
        public string empresaShort { get; set; }
        public string sucursal { get; set; }
        public string sucursalShort { get; set; }
        public string concatenado { get; set; }
        public string concatenadoShort { get; set; }

        public int codigoequipo { get; set; }

        public string equipo { get; set; }

        public string equipoShort { get; set; }
    }
    public class VisorCita
    {
        public DateTime hora { get; set; }
        public string turno { get; set; }
        public bool sedacion { get; set; }
        public string paciente { get; set; }
        public string estudio { get; set; }
        public string estado { get; set; }
        public int cita { get; set; }
        public int equipo { get; set; }
        public string imgSedacion { get { return sedacion ? "../../img/sedacion.png" : ""; } }



        public string nombreequipo { get; set; }

        public string observacion { get; set; }

        public string idpaciente { get; set; }
        public bool pacienteVisible { get { return idpaciente != ""; } }
    }
    public class VisorCitaEjm
    {
        public string titulo { get; set; }
        public List<VisorCita> estudios { get; set; }
    }
    public class VisorCitaEjmX
    {
        public List<VisorCitaEjm> stk1 { get; set; }
        public List<VisorCitaEjm> stk2 { get; set; }
    }
    public class CitarxConfirmar
    {
        public int codigopaciente { get; set; }
        public string paciente { get; set; }
        public DateTime fecha_reserva { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }
        public string email{ get; set; }
        public int llamadas { get; set; }
        public int numerocita { get; set; }
        public System.Windows.Visibility isVisibleTel { get { return telefono == "0" || telefono == "" ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible; } }
        public System.Windows.Visibility isVisibleCel { get { return celular == "0" || celular == "" ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible; } }
        public System.Windows.Visibility isVisibleEmail { get { return email == " " || email == "" ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible; } }

        public string observacion { get; set; }

        public string codigocartagarantia { get; set; }


        public bool sedacion { get; set; }

        public string hora { get; set; }

        public string equipo { get; set; }
    }
}
