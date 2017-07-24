using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    public class PacienteDAO
    {
        public List<PACIENTE> getBuscarPacienteExacto(string dni, string apellido, string nombre)
        {
            List<PACIENTE> lista = new List<PACIENTE>();
            string query = "select * from Paciente where {0} {1} {2}";
            if (dni != "")
                dni = "dni='" + dni + "'";
            if (apellido != "")
            {
                apellido = "apellidos='" + apellido + "'";
                if (dni != "")
                    apellido = " and " + apellido;
            }
            if (nombre != "")
            {
                nombre = "nombres='" + nombre + "'";
                if (dni != "" || apellido != "")
                    nombre = " and " + nombre;
            }
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, dni, apellido, nombre), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new PACIENTE();
                            item.nacionalidad = ((reader["nacionalidad"]).ToString());
                            item.direccion = ((reader["direccion"]).ToString());
                            item.email = ((reader["email"]).ToString());
                            item.celular = ((reader["celular"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            item.fechanace = Convert.ToDateTime((reader["fechanace"]).ToString());
                            item.sexo = ((reader["sexo"]).ToString());
                            item.nombres = ((reader["nombres"]).ToString());
                            item.apellidos = ((reader["apellidos"]).ToString());
                            item.dni = ((reader["dni"]).ToString());
                            item.codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString());
                            item.tipo_doc = ((reader["tipo_doc"]).ToString());
                            item.IsProtocolo = Convert.ToBoolean((reader["IsProtocolo"]).ToString());
                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public List<PACIENTE> getBuscarPacientexCoincidencia(string filtro,string nombre,string dni)
        {
            List<PACIENTE> lista = new List<PACIENTE>();
            string query = "select nacionalidad,direccion,email,celular,telefono,fechanace,sexo,nombres,apellidos,dni,codigopaciente,tipo_doc,IsProtocolo from Paciente where apellidos like '%{0}%'";
            if (nombre != "")
                query += " and nombres like '%" + nombre + "%'";
            if (dni != "")
                query += " and dni = '" + dni + "'";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, filtro), connection);
                    connection.Open();
                    //command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new PACIENTE();
                            item.nacionalidad = ((reader["nacionalidad"]).ToString());
                            item.direccion = ((reader["direccion"]).ToString());
                            item.email = ((reader["email"]).ToString());
                            item.celular = ((reader["celular"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            item.fechanace = Convert.ToDateTime((reader["fechanace"]).ToString());
                            item.sexo = ((reader["sexo"]).ToString());
                            item.nombres = ((reader["nombres"]).ToString());
                            item.apellidos = ((reader["apellidos"]).ToString());
                            item.dni = ((reader["dni"]).ToString());
                            item.codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString());
                            item.tipo_doc = ((reader["tipo_doc"]).ToString());
                            item.IsProtocolo = Convert.ToBoolean((reader["IsProtocolo"]).ToString());
                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public PACIENTE getPaciente(int codigopaciente)
        {
            PACIENTE item = new PACIENTE();
            string query = "select * from Paciente where codigopaciente={0}";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, codigopaciente), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            item.nacionalidad = ((reader["nacionalidad"]).ToString());
                            item.direccion = ((reader["direccion"]).ToString());
                            item.email = ((reader["email"]).ToString());
                            item.celular = ((reader["celular"]).ToString());
                            item.telefono = ((reader["telefono"]).ToString());
                            item.fechanace = Convert.ToDateTime((reader["fechanace"]).ToString());
                            item.sexo = ((reader["sexo"]).ToString());
                            item.nombres = ((reader["nombres"]).ToString());
                            item.apellidos = ((reader["apellidos"]).ToString());
                            item.dni = ((reader["dni"]).ToString());
                            item.codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString());
                            item.tipo_doc = ((reader["tipo_doc"]).ToString());
                            item.IsProtocolo = Convert.ToBoolean((reader["IsProtocolo"]).ToString());
                        }
                    }
                    finally
                    {
                        reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return item;
        }
        public bool validarPaciente(string dni, string tipo_documento)
        {
            bool correct = true;
            string query = "select * from PACIENTE where tipo_doc='{0}' and dni='{1}'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, tipo_documento, dni), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        correct = !reader.HasRows;
                    }
                    finally
                    {
                        reader.Close(); 
                        connection.Dispose();
                        connection.Close();
                        
                    }
                }
            }
            return correct;
        }
        public void addPaciente(PACIENTE p)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                p.codigopaciente = db.PACIENTE.Max(x => x.codigopaciente) + 1;
                db.PACIENTE.Add(p);
                db.SaveChanges();
            }
        }
        public void updatePaciente(PACIENTE pac)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var p = db.PACIENTE.Where(x => x.codigopaciente == pac.codigopaciente).SingleOrDefault();
                if (p != null)
                {
                    p.nombres = pac.nombres;
                    p.apellidos = pac.apellidos;
                    p.fechanace = pac.fechanace;
                    p.dni = pac.dni;
                    p.tipo_doc = pac.tipo_doc;
                    p.direccion = pac.direccion;
                    p.IsProtocolo = pac.IsProtocolo;
                    p.telefono = pac.telefono;
                    p.sexo = pac.sexo.Substring(0, 1);
                    p.celular = pac.celular;
                    p.email = pac.email;
                    p.nacionalidad = pac.nacionalidad;
                }
                db.SaveChanges();
            }
        }
    }

}
