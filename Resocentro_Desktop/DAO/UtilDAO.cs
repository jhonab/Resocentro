using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    class UtilDAO
    {
        public List<UNIDADNEGOCIO> getEmpresa()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.UNIDADNEGOCIO.OrderBy(x => x.codigounidad).ToList();
            }
        }

        public List<SucursalesxUsuario> getSucursales(string[] sucursales)
        {
            List<SucursalesxUsuario> lista = new List<SucursalesxUsuario>();
            string queryString = @"select UN.codigounidad,su.codigosucursal,(un.codigounidad*100)+su.codigosucursal codigo , un.nombre+' - Sede '+su.ShortDesc nombre, un.nombre+' - '+su.ShortDesc shortname from SUCURSAL su inner join UNIDADNEGOCIO un on su.codigounidad =un.codigounidad where ((un.codigounidad*100)+su.codigosucursal) in ('{0}') order by un.codigounidad";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, String.Join("','", sucursales)), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new SucursalesxUsuario();
                            item.codigounidad = Convert.ToInt32((reader["codigounidad"]).ToString());
                            item.codigosucursal = Convert.ToInt32((reader["codigosucursal"]).ToString());
                            item.codigoInt = Convert.ToInt32((reader["codigo"]).ToString());
                            item.codigoString = ((reader["codigo"]).ToString());
                            item.nombre = ((reader["nombre"]).ToString());
                            item.nombreShort = ((reader["shortname"]).ToString());

                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return lista;
        }

        public List<EQUIPO> getEquipoxEmpresa(int empresa)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.EQUIPO.Where(x => x.codigounidad2 == empresa).OrderBy(x => x.nombreequipo).ToList();
            }
        }
        public List<USUARIO> getUsuarios()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.USUARIO.Where(x => x.bloqueado == false).OrderBy(x => x.ShortName).ToList();
            }
        }
        public List<PACIENTE> getMotivoBloqueo(bool isAutorizadoMantenimiento)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                int[] autorizado = { 20272, 281051, 281052, 281053 };
                if (isAutorizadoMantenimiento)
                    return db.PACIENTE.Where(x => x.apellidos.ToLower().StartsWith("bloqueado ")).OrderBy(x => x.apellidos).ToList();
                else
                    return db.PACIENTE.Where(x => x.apellidos.ToLower().StartsWith("bloqueado ") && !(autorizado.Contains(x.codigopaciente))).OrderBy(x => x.apellidos).ToList();
            }
        }

        public void setBlockTurno(int codigopaciente, int codigohorario, MySession session)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var _horario = db.HORARIO.Where(x => x.codigohorario == codigohorario).SingleOrDefault();
                if (_horario != null)
                {
                    _horario.bloquear = true;
                    _horario.codigopaciente = codigopaciente;
                    //Fizcalizacion
                    var _fiz = new FIZCALIZACION();
                    _fiz.hora = Tool.getDatetime(); ;
                    _fiz.fecha = Tool.getDatetime(); ;
                    _fiz.nombrepc = session.shortuser;
                    _fiz.descripcioncambio = "BLOQUEO EL TURNO  con el codigo: " + codigohorario;
                    _fiz.codigousuario = session.codigousuario;
                    _fiz.codigohorario = codigohorario;

                    db.FIZCALIZACION.Add(_fiz);
                    db.SaveChanges();
                }
            }
        }
        public void setUnBlockTurno(int codigopaciente, int codigohorario, MySession session)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var _horario = db.HORARIO.Where(x => x.codigohorario == codigohorario).SingleOrDefault();
                if (_horario != null)
                {
                    _horario.bloquear = false;
                    _horario.codigopaciente = null;
                    _horario.codigoestudio = null;
                    _horario.numerocita = null;
                    //Fizcalizacion
                    var _fiz = new FIZCALIZACION();
                    _fiz.hora = Tool.getDatetime();
                    _fiz.fecha = Tool.getDatetime();
                    _fiz.nombrepc = session.shortuser;
                    _fiz.descripcioncambio = "BLOQUEO EL TURNO  con el codigo: " + codigohorario;
                    _fiz.codigousuario = session.codigousuario;
                    _fiz.codigohorario = codigohorario;

                    db.FIZCALIZACION.Add(_fiz);
                    db.SaveChanges();
                }
            }
        }
        public List<ModalidadesxEmpresa> getModalidadxEmpresa(string empresa)
        {
            List<ModalidadesxEmpresa> lista = new List<ModalidadesxEmpresa>();
            string queryString = @"select * from MODALIDAD where codigounidad ='{0}' and IsEnabled=1";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, empresa), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new ModalidadesxEmpresa();
                            item.codigo = Convert.ToInt32((reader["codigomodalidad"]).ToString());
                            item.nombre = ((reader["nombre"]).ToString());
                            item.shortname = ((reader["abreviatura"]).ToString());

                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return lista;
        }

        public List<COMPANIASEGURO> getAseguradora(string nombre)
        {
            List<COMPANIASEGURO> lista = new List<COMPANIASEGURO>();
            string queryString = @"select a.codigocompaniaseguro,a.ruc,a.descripcion from COMPANIASEGURO a where  a.descripcion like '%{0}%'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, nombre), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new COMPANIASEGURO();
                            item.codigocompaniaseguro = Convert.ToInt32((reader["codigocompaniaseguro"]).ToString());
                            item.ruc = ((reader["ruc"]).ToString());
                            item.descripcion = ((reader["descripcion"]).ToString());

                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return lista;
        }

        public COMPANIASEGURO getAseguradoraxCodigo(string codigo)
        {
            COMPANIASEGURO item = new COMPANIASEGURO();
            string queryString = @"select * from COMPANIASEGURO a where  a.codigocompaniaseguro='{0}'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, codigo), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            item.codigocompaniaseguro = Convert.ToInt32((reader["codigocompaniaseguro"]).ToString());
                            item.ruc = ((reader["ruc"]).ToString());
                            item.descripcion = ((reader["descripcion"]).ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return item;
        }

        public List<CLINICAHOSPITAL> getClinica(string nombre)
        {
            List<CLINICAHOSPITAL> lista = new List<CLINICAHOSPITAL>();
            string queryString = @"select * from CLINICAHOSPITAL where  razonsocial like  '%{0}%'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, nombre), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new CLINICAHOSPITAL();
                            item.codigoclinica = Convert.ToInt32((reader["codigoclinica"]).ToString());
                            item.razonsocial = ((reader["razonsocial"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            if (reader["celular"] != null)
                                item.celular = ((reader["celular"]).ToString());
                            item.direccion = ((reader["celular"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            item.tipo = ((reader["tipo"]).ToString());

                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public CLINICAHOSPITAL getClinicaxCodigo(int codigo)
        {
            CLINICAHOSPITAL item = new CLINICAHOSPITAL();
            string queryString = @"select * from CLINICAHOSPITAL where  codigoclinica = '{0}'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, codigo), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            item.codigoclinica = Convert.ToInt32((reader["codigoclinica"]).ToString());
                            item.razonsocial = ((reader["razonsocial"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            if (reader["celular"] != null)
                                item.celular = ((reader["celular"]).ToString());
                            item.direccion = ((reader["celular"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            item.tipo = ((reader["tipo"]).ToString());

                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return item;
        }
        public List<TipoMedico> getTipoMedico()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.TipoMedico.ToList();
            }
        }
        public List<MEDICOEXTERNO> getMedico(string nombre, string cmp, string tipomedico)
        {
            List<MEDICOEXTERNO> lista = new List<MEDICOEXTERNO>();
            string queryString = @"select me.*,es.descripcion  from MEDICOEXTERNO me inner join ESPECIALIDAD es on me.codigoespecialidad=es.codigoespecialidad where me.isactivo=1 and me.tipomedico='{2}' {0} {1}";
            if (nombre != "")
                nombre = "and apellidos like '%" + nombre + "%'";
            if (cmp != "")
                cmp = "and cmp like '" + cmp + "'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, nombre, cmp, tipomedico), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new MEDICOEXTERNO();
                            item.cmp = ((reader["cmp"]).ToString());
                            item.apellidos = ((reader["apellidos"]).ToString());
                            item.nombres = ((reader["nombres"]).ToString());
                            if (reader["email"] != null)
                                item.email = ((reader["email"]).ToString());
                            item.direccion = ((reader["celular"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            item.codigoespecialidad = Convert.ToInt32((reader["codigoespecialidad"]).ToString());
                            item.hobby = ((reader["descripcion"]).ToString());
                            item.dniMedico = ((reader["dniMedico"]).ToString());

                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public MEDICOEXTERNO getMedicoxCMP(string cmp, int codigoTipoMedico)
        {
            MEDICOEXTERNO item = new MEDICOEXTERNO();
            string queryString = @"select me.*,es.descripcion  from MEDICOEXTERNO me inner join ESPECIALIDAD es on me.codigoespecialidad=es.codigoespecialidad where me.isactivo=1 and cmpCorregido='{0}' and me.tipomedico='{1}' ";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, cmp, codigoTipoMedico), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {

                            item.cmp = ((reader["cmp"]).ToString());
                            item.apellidos = ((reader["apellidos"]).ToString());
                            item.nombres = ((reader["nombres"]).ToString());
                            if (reader["email"] != null)
                                item.email = ((reader["email"]).ToString());
                            item.direccion = ((reader["celular"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            item.codigoespecialidad = Convert.ToInt32((reader["codigoespecialidad"]).ToString());
                            item.telefono = ((reader["descripcion"]).ToString());
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return item;
        }

        public MEDICOEXTERNO getMedicoxCodigo(string codigo)
        {
            MEDICOEXTERNO item = new MEDICOEXTERNO();
            string queryString = @"select me.*,es.descripcion  from MEDICOEXTERNO me inner join ESPECIALIDAD es on me.codigoespecialidad=es.codigoespecialidad where me.isactivo=1 and cmp='{0}'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, codigo), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {

                            item.cmp = ((reader["cmp"]).ToString());
                            item.apellidos = ((reader["apellidos"]).ToString());
                            item.nombres = ((reader["nombres"]).ToString());
                            if (reader["email"] != null)
                                item.email = ((reader["email"]).ToString());
                            item.direccion = ((reader["celular"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            item.codigoespecialidad = Convert.ToInt32((reader["codigoespecialidad"]).ToString());
                            item.telefono = ((reader["descripcion"]).ToString());
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return item;
        }

        public List<Motivo_Entre_Turno> getMotivoEntreTurno()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.Motivo_Entre_Turno.Where(x => x.isActivo == true).ToList();
            }
        }

        public List<Object> getCancelaciones()
        {
            return new List<Object> { 
                       new { codigo=1,nombre ="Presenta Contraindicaciones" },
                       new { codigo=2,nombre ="El Paciente no colabora" },
                       new { codigo=3,nombre ="El Paciente no desea realizar el examen" },
                       new { codigo=4,nombre ="El Paciente presenta dolor en el examen " },
                       new { codigo=4,nombre ="Equipo averiado" },
                       new { codigo=5,nombre ="Presenta Contraindicaciones para la Sedacion" },
                       new { codigo=6,nombre ="Anestesia" },
                       new { codigo=7,nombre ="Claustrofobia" },
                       new { codigo=99,nombre ="Otros" }
                    };
        }

        public List<Object> getEstadoCarta()
        {
            var lst = new List<Object>();
            lst.Add(new { nombre = "TRAMITADA" });
            lst.Add(new { nombre = "APROBADA" });
            lst.Add(new { nombre = "RECHAZADA" });
            lst.Add(new { nombre = "OBSERVADA" });
            lst.Add(new { nombre = "SIN TRAMITAR" });
            lst.Add(new { nombre = "AVERIADA" });
            lst.Add(new { nombre = "ANULADA" });
            lst.Add(new { nombre = "CITADA" });
            lst.Add(new { nombre = "CADUCADA" });
            return lst;
        }
        public List<Tipo_Afiliacion> getTipoAfiliacion()
        {
            var lst = new List<Tipo_Afiliacion>();
            lst.Add(new Tipo_Afiliacion { nombre = "Regular", codigo = 1 });
            lst.Add(new Tipo_Afiliacion { nombre = "SCTR", codigo = 2 });
            lst.Add(new Tipo_Afiliacion { nombre = "Potestativo (Independiente)", codigo = 3 });
            lst.Add(new Tipo_Afiliacion { nombre = "SCTR Independiente", codigo = 4 });
            lst.Add(new Tipo_Afiliacion { nombre = "Complementarios", codigo = 5 });
            lst.Add(new Tipo_Afiliacion { nombre = "SOAT", codigo = 6 });
            lst.Add(new Tipo_Afiliacion { nombre = "AFOCAT", codigo = 7 });
            return lst;
        }
        public List<Object> getTipoDocumentoSUNAT()
        {
            return new List<Object> { 
                       new { codigo="1",nombre ="DNI" },
                       new { codigo="6",nombre ="RUC" },
                       new { codigo="4",nombre ="Carnet de Extranjeria" },
                       new { codigo="7",nombre ="Pasaporte" },
                       new { codigo="A",nombre ="Cedula Diplomatica" },
                       new { codigo="0",nombre ="DOC. TRIB. NO. DOM. SIN. RUC" }
                    };
        }
        public List<Object> getMes()
        {
            var lst = new List<Object>();
            lst.Add(new { nombre = "Enero", codigo = 1 });
            lst.Add(new { nombre = "Febrero", codigo = 2 });
            lst.Add(new { nombre = "Marzo", codigo = 3 });
            lst.Add(new { nombre = "Abril", codigo = 4 });
            lst.Add(new { nombre = "Mayo", codigo = 5 });
            lst.Add(new { nombre = "Junio", codigo = 6 });
            lst.Add(new { nombre = "Julio", codigo = 7 });
            lst.Add(new { nombre = "Agosto", codigo = 8 });
            lst.Add(new { nombre = "Septiembre", codigo = 9 });
            lst.Add(new { nombre = "Octubre", codigo = 10 });
            lst.Add(new { nombre = "Noviembre", codigo = 11 });
            lst.Add(new { nombre = "Diciembre", codigo = 12 });
            return lst;
        }
        public List<Object> getAño(int cantidad)
        {
            int inicio = DateTime.Now.Year - cantidad;
            var lst = new List<Object>();
            for (int i = inicio; i <= DateTime.Now.Year; i++)
            {
                lst.Add(new { nombre = i.ToString(), codigo = i });
            }

            return lst;
        }

        public static int calcularEdad(DateTime fechanacimiento)
        {
            return DateTime.Today.AddTicks(-fechanacimiento.Ticks).Year - 1;
        }
    }

}


public class Adjuntos_Web
{
    public string ruta { get; set; }
    public string nombre { get; set; }
    public double size { get; set; }
    public string sizeKb { get { return size.ToString() + " KB"; } }
    public string sizeMb { get { return (size / 1024).ToString() + " MB"; } }
    public string sizeGb { get { return (size / 1024 / 1024).ToString() + " GB"; } }
}
public class Adjuntos_Desktop : INotifyPropertyChanged
{
    private string _ruta;
    public string ruta
    {
        get { return this._ruta; }
        set
        {
            if (value != _ruta)
            {
                this._ruta = value;
                NotifyPropertyChanged();
            }
        }
    }

    private string _nombre;
    public string nombre
    {
        get { return this._nombre; }
        set
        {
            if (value != _nombre)
            {
                this._nombre = value;
                NotifyPropertyChanged();
            }
        }
    }

    private double _size;
    public double size
    {
        get { return this._size; }
        set
        {
            if (value != _size)
            {
                this._size = value;
                NotifyPropertyChanged();
            }
        }
    }

    private bool _isFisico;
    public bool isFisico
    {
        get { return this._isFisico; }
        set
        {
            if (value != _isFisico)
            {
                this._isFisico = value;
                NotifyPropertyChanged();
            }
        }
    }

    private byte[] _archivo;
    public byte[] archivo
    {
        get { return this._archivo; }
        set
        {
            if (value != _archivo)
            {
                this._archivo = value;
                NotifyPropertyChanged();
            }
        }
    }

    public string sizeKb { get { return _size.ToString() + " KB"; } }
    public string sizeMb { get { return (_size / 1024).ToString() + " MB"; } }
    public string sizeGb { get { return (_size / 1024 / 1024).ToString() + " GB"; } }


    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
