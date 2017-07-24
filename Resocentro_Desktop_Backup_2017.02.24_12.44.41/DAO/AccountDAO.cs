using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    public class AccountDAO
    {
        public MySession Login(string siglas, string codigo)
        {
            MySession session = new MySession();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    string queryString = @"select c.descripcion cargo,e.sexo,u.ShortName,u.codigousuario,u.siglas,e.email,(su.codigounidad*100+su.codigosucursal) sucursal from USUARIO u 
inner join SUCURSALXUSUARIO su on u.codigousuario=su.codigousuario
inner join EMPLEADO e on u.dni=e.dni
inner join CARGO c on e.codigocargo=c.codigocargo
where
u.siglas='" + siglas + "' and u.contrasena='" + codigo + "'";

                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    List<int> sucursales = new List<int>();
                    while (reader.Read())
                    {
                        session.isLogon = true;
                        session.cargo = reader["cargo"].ToString();
                        session.shortuser = reader["ShortName"].ToString();
                        session.codigousuario = reader["codigousuario"].ToString();
                        session.siglas = reader["siglas"].ToString();
                        session.email = reader["email"].ToString();
                        sucursales.Add(Convert.ToInt32(reader["sucursal"]));
                        session.PathBuddy = Tool.PathImgBuddy + session.codigousuario + ".png";
                        session.ispathBuddy = System.IO.File.Exists(session.PathBuddy);
                        if (!session.ispathBuddy)
                        {
                           /* if (reader["sexo"].ToString() == "M")
                                session.PathBuddy = "/img/hombre.png";
                            else
                                session.PathBuddy = "/img/mujer.png";*/

                            session.PathBuddy = "/img/profile.png";
                        }

                    }
                    reader.Close();
                    if (session.codigousuario != "")
                    {
                        List<int> permisos = new List<int>();
                        command.CommandText = "select tipo_permiso from Permiso where idaplicativo=7 and codigousuario='" + session.codigousuario + "' order by tipo_permiso";
                        command.Prepare();
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            permisos.Add(Convert.ToInt32(reader["tipo_permiso"]));
                        }
                        reader.Close();
                        if (sucursales.Count > 0)
                        {
                            session.sucursales = sucursales.Select(x => x.ToString()).ToArray();
                            session.sucursales_int = sucursales.ToArray();
                        }
                        if (permisos.Count > 0)
                        {
                            session.roles = permisos.Select(x => x.ToString()).ToArray();
                        }
                    }
                    reader.Dispose();
                }
                /*
                var _usuario = db.USUARIO.SingleOrDefault(x => x.contrasena == codigo && x.siglas == siglas);
                if (_usuario != null)
                {
                    var _unidades = (from x in db.SUCURSALXUSUARIO
                                     join un in db.UNIDADNEGOCIO
                                     on x.codigounidad equals un.codigounidad
                                     where x.codigousuario == _usuario.codigousuario //&& x.codigounidad==1
                                     select new
                                     {
                                         sucursal = ((x.codigounidad * 100) + x.codigosucursal)
                                     }).ToList();

                    session.isLogon = true;
                    session.cargo = _usuario.EMPLEADO.CARGO.descripcion;
                    session.shortuser = _usuario.ShortName;
                    session.codigousuario = _usuario.codigousuario;
                    session.siglas = _usuario.siglas;
                    session.email = _usuario.EMPLEADO.email;
                    session.sucursales = _unidades.Select(x => (x.sucursal).ToString()).ToArray();
                    session.sucursales_int = _unidades.Select(x => x.sucursal).ToArray();

                    session.roles = (from x in db.Permiso where x.idaplicativo == 7 && x.codigousuario == _usuario.codigousuario select x.tipo_permiso.ToString()).ToArray();

                */
            }
            return session;
        }
    }
}

public class MySession
{
    public bool isLogon { get; set; }
    public string[] roles { get; set; }
    public string shortuser { get; set; }
    public string codigousuario { get; set; }
    public string siglas { get; set; }
    public string email { get; set; }
    public string[] sucursales { get; set; }
    public int[] sucursales_int { get; set; }
    public string cargo { get; set; }
    public bool ispathBuddy { get; set; }
    public string PathBuddy { get; set; }
}
