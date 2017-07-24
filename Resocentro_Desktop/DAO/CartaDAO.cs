using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;
using System.Data;

namespace Resocentro_Desktop.DAO
{
    public class CartaDAO
    {
        public async Task<bool> CambiarCartaxCita(int cita, string carta, int paciente)
        {
            bool result = false;
            string error = "";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand("dbo.cambiarCarta_Cita", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("@ncita", SqlDbType.Int).Value = cita;
                    command.Parameters.Add("@codigocarta", SqlDbType.VarChar).Value = carta;
                    command.Parameters.Add("@paciente", SqlDbType.Int).Value = paciente;
                    connection.Open();
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    result = Convert.ToBoolean(reader["r"].ToString());
                                    error += (reader["carta"].ToString()) + "\n";
                                    error += (reader["cita"].ToString()) + "\n";
                                    error += (reader["doc"].ToString()) + "\n";
                                }
                            }
                        }                        
                        if (!result)
                        {                            
                            throw new Exception(error);
                        }
                        return result;
                    }
                    catch (Exception ex)
                    {
                        string mensaje = "ERROR SQL: No se guardaron cambios de Carta :\n-" + ex.Message;                      
                        throw new Exception(mensaje);
                     
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }

        }
        public List<CG_SolicitudCarta> getListaSolicitudesCarta(int estadoSolicitud)
        {
            List<CG_SolicitudCarta> lista = new List<CG_SolicitudCarta>();
            string queryString = @"select sc.idSolicitud,sc.nombres,sc.apellidos,sc.num_doc,sc.email,sc.telefono,sc.celular,idAseguradora,sc.aseguradora,sc.nombre_medico,sc.apellido_medico,sc.clinica,sc.fec_registro,sc.comentario,sc.estado,ul.ShortName usu_lectura ,sc.fec_lectura
from CG_SolicitudCarta sc
left join USUARIO ul on sc.usu_lectura=ul.codigousuario
where sc.estado={0}  order by sc.fec_registro";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, estadoSolicitud.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new CG_SolicitudCarta();

                            item.idSolicitud = Convert.ToInt32((reader["idSolicitud"]).ToString());
                            item.nombres = ((reader["nombres"]).ToString());
                            item.apellidos = ((reader["apellidos"]).ToString());
                            item.num_doc = ((reader["num_doc"]).ToString());
                            item.email = ((reader["email"]).ToString());
                            if (reader["telefono"] != DBNull.Value)
                                item.telefono = Convert.ToInt32((reader["telefono"]).ToString());
                            if (reader["celular"] != DBNull.Value)
                                item.celular = Convert.ToInt32((reader["celular"]).ToString());
                            item.idAseguradora = Convert.ToInt32((reader["idAseguradora"]).ToString());
                            item.aseguradora = ((reader["aseguradora"]).ToString());
                            item.nombre_medico = ((reader["nombre_medico"]).ToString());
                            item.apellido_medico = ((reader["apellido_medico"]).ToString());
                            item.clinica = ((reader["clinica"]).ToString());
                            item.fec_registro = Convert.ToDateTime((reader["fec_registro"]).ToString());
                            item.comentario = ((reader["comentario"]).ToString());
                            item.estado = Convert.ToInt32((reader["estado"]).ToString());
                            if (reader["usu_lectura"] != DBNull.Value)
                                item.usu_lectura = ((reader["usu_lectura"]).ToString());
                            if (reader["fec_lectura"] != DBNull.Value)
                                item.fec_lectura = Convert.ToDateTime((reader["fec_lectura"]).ToString());


                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public List<CG_SolicituCarta_Historial> getSolicitudesCarta(int idsolicitud)
        {
            List<CG_SolicituCarta_Historial> lista = new List<CG_SolicituCarta_Historial>();
            string queryString = @"select* from CG_SolicituCarta_Historial sc where sc.idSolicitud={0} ";
            string queryString2 = @"select* from CG_SolicitudCarta sc where sc.idSolicitud={0} ";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, idsolicitud.ToString()), connection);
                    SqlCommand command1 = new SqlCommand(string.Format(queryString2, idsolicitud.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new CG_SolicituCarta_Historial();
                            item.idDetSol = Convert.ToInt32((reader["idDetSol"]).ToString());
                            item.idSolicitud = Convert.ToInt32((reader["idSolicitud"]).ToString());
                            item.usuario = ((reader["usuario"]).ToString());
                            item.comentarios = ((reader["comentarios"]).ToString());
                            item.fec_reg = Convert.ToDateTime((reader["fec_reg"]).ToString());

                            lista.Add(item);
                        }
                        reader = command1.ExecuteReader();
                        while (reader.Read())
                        {
                            var item = new CG_SolicituCarta_Historial();
                            item.idDetSol = 0;
                            item.idSolicitud = Convert.ToInt32((reader["idSolicitud"]).ToString());
                            item.usuario = ((reader["nombres"]).ToString()) + " " + ((reader["apellidos"]).ToString());
                            item.comentarios = ((reader["comentario"]).ToString());
                            item.fec_reg = Convert.ToDateTime((reader["fec_registro"]).ToString());

                            lista.Add(item);
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public List<CG_SolicitudCarta> updateSolicitudCarta(CG_SolicitudCarta item, string usuario)
        {
            List<CG_SolicitudCarta> lista = new List<CG_SolicitudCarta>();
            string queryString = @"update CG_SolicitudCarta set {1} where idSolicitud='{0}'";
            List<string> campos = new List<string>();
            if (item.estado != 0)
            {
                campos.Add("estado='" + item.estado + "'");
                switch (item.estado)
                {
                    case (int)Estado_SolicitudTramite.Leido:
                        new CartaDAO().insertLog(item.idSolicitud.ToString(), usuario, (int)Tipo_Log.Update, "Se actualizo el estado a Leído");
                        break;
                    case (int)Estado_SolicitudTramite.Tramitado:
                        new CartaDAO().insertLog(item.idSolicitud.ToString(), usuario, (int)Tipo_Log.Update, "Se actualizo el estado a Tramitado");
                        break;
                    case (int)Estado_SolicitudTramite.Espera:
                        new CartaDAO().insertLog(item.idSolicitud.ToString(), usuario, (int)Tipo_Log.Update, "Se actualizo el estado a Espera");
                        break;
                    case (int)Estado_SolicitudTramite.Cancelado:
                        new CartaDAO().insertLog(item.idSolicitud.ToString(), usuario, (int)Tipo_Log.Update, "Se actualizo el estado a Cancelado");
                        break;
                    default:
                        break;
                }
            }
            if (item.fec_lectura != null)
            {
                campos.Add("fec_lectura='" + item.fec_lectura.Value.ToString("dd/MM/yyyy hh:mm:ss") + "'");
                if (item.usu_lectura != null)
                    campos.Add("usu_lectura='" + item.usu_lectura + "'");
            }
            if (item.numeroproforma != null)
            {
                campos.Add("numeroproforma='" + item.numeroproforma.Value.ToString("") + "'");
            }

            if (campos.Count > 0)
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                    {
                        SqlCommand command = new SqlCommand(string.Format(queryString, item.idSolicitud, String.Join(",", campos.ToArray())), connection);
                        connection.Open();
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            return lista;
        }
        public string getNewCodigoCarta()
        {
            string codigo = "";
            string queryString = @"select isnull( 'C'+convert(varchar(10),YEAR(GETDATE()))+'-'+convert(varchar(20),max(convert(int,substring(codigocartagarantia,7,100)))+1),'C'+convert(varchar(10),YEAR(GETDATE()))+'-1') codigo from CARTAGARANTIA where codigocartagarantia like 'C'+convert(varchar(10),YEAR(GETDATE()))+'%'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            codigo = ((reader["codigo"]).ToString());

                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return codigo;
        }
        public void deleteCarta(string idcarta, int idpaciente)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var carta = db.CARTAGARANTIA.Where(x => x.codigocartagarantia == idcarta && x.codigopaciente == idpaciente).SingleOrDefault();
                if (carta != null)
                {
                    if (carta.codigodocadjunto != null)
                        if (carta.codigodocadjunto != "")
                        {
                            var _documento = db.DOCESCANEADO.Where(x => x.codigodocadjunto == carta.codigodocadjunto).SingleOrDefault();
                            if (_documento != null)
                            {
                                db.DOCESCANEADO.Remove(_documento);
                            }
                        }
                    var detalle = db.ESTUDIO_CARTAGAR.Where(x => x.codigocartagarantia == carta.codigocartagarantia && x.codigopaciente == idpaciente).ToList();
                    foreach (var item in detalle)
                    {
                        db.ESTUDIO_CARTAGAR.Remove(item);
                    }
                    db.CARTAGARANTIA.Remove(carta);
                    db.SaveChanges();
                }
            }
        }
        public List<List_Proforma> getListaProformas(string estado,string filtro)
        {
            List<List_Proforma> lista = new List<List_Proforma>();
            string queryString = @"select pro.codigodocescaneado,pro.numerodeproforma,pro.fechaemision,p.apellidos+', '+p.nombres paciente,pro.titular,cs.descripcion aseguradora,ch.razonsocial clinica,pro.contratante,us.ShortName usuario, pro.sedacion,isnull(pro.ishospitalizado,0)ishospitalizado
from PROFORMA pro
inner join PACIENTE p on pro.codigopaciente=p.codigopaciente
inner join COMPANIASEGURO cs on pro.codigocompaniaseguro=cs.codigocompaniaseguro
inner join CLINICAHOSPITAL ch on pro.codigoclinica=ch.codigoclinica
inner join MEDICOEXTERNO me on pro.cmp=me.cmp
inner join USUARIO us on pro.codigousuario=us.codigousuario
where pro.estado='{0}'
{1}
";
            string filtro_sql = " and p.apellidos='{0}'";
            if (filtro != "")
                filtro_sql = string.Format(filtro_sql, filtro);
            else
                filtro_sql = filtro;
            
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, estado,filtro), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new List_Proforma();

                            item.numeroproforma = Convert.ToInt32((reader["numerodeproforma"]).ToString());
                            item.codigodocescaneado = ((reader["codigodocescaneado"]).ToString());
                            item.fechaemision = Convert.ToDateTime((reader["fechaemision"]).ToString());
                            item.paciente = ((reader["paciente"]).ToString());
                            item.titular = ((reader["titular"]).ToString());
                            item.aseguradora = ((reader["aseguradora"]).ToString());
                            item.clinica = ((reader["clinica"]).ToString());
                            item.contratante = ((reader["contratante"]).ToString());
                            item.usuario = ((reader["usuario"]).ToString());
                            item.sedacion = Convert.ToBoolean((reader["sedacion"]).ToString());
                            item.hospitalizado = Convert.ToBoolean((reader["ishospitalizado"]).ToString());
                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public List<List_Carta> getCartaxPaciente(int codigopaciente)
        {
            List<List_Carta> lista = new List<List_Carta>();
            string queryString = @"select cg.fechatramite,cg.codigopaciente,cg.codigocartagarantia,cg.codigocartagarantia2,p.apellidos+', '+p.nombres paciente,cg.estadocarta,cs.descripcion aseguradora,us.ShortName usuario ,cg.isRevisada ,isnull(usa.ShortName,'') actualizador,cg.fec_update fec_update,isnull((select shortname from usuario u where u.codigousuario=cg.user_revisa),'Aún no se revisa')user_revisa,isnull(usp.ShortName,'') proforma,cg.obs_revision,cg.cobertura  from CARTAGARANTIA cg
inner join PACIENTE p on cg.codigopaciente=p.codigopaciente
inner join COMPANIASEGURO cs on cg.codigocompaniaseguro=cs.codigocompaniaseguro
inner join MEDICOEXTERNO me on cg.cmp=me.cmp
inner join USUARIO us on cg.codigousuario=us.codigousuario
left join USUARIO usa on cg.user_update=usa.codigousuario
left join PROFORMA pro on cg.numero_proforma=pro.numerodeproforma
left join USUARIO usp on pro.codigousuario=usp.codigousuario
where cg.codigopaciente='{0}'
";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, codigopaciente.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new List_Carta();

                            item.fechatramite = Convert.ToDateTime((reader["fechatramite"]).ToString());
                            if (reader["fec_update"] != DBNull.Value)
                                item.fechaupdate = Convert.ToDateTime((reader["fec_update"]).ToString());
                            item.codigocartagarantia = ((reader["codigocartagarantia"]).ToString());
                            item.codigocartagarantia2 = ((reader["codigocartagarantia2"]).ToString());
                            item.paciente = ((reader["paciente"]).ToString());
                            item.cobertura = ((reader["cobertura"]).ToString());
                            item.estadocarta = ((reader["estadocarta"]).ToString());
                            item.aseguradora = ((reader["aseguradora"]).ToString());
                            item.usuario = ((reader["usuario"]).ToString());
                            item.actualizador = ((reader["actualizador"]).ToString());
                            item.codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString());
                            item.isrevisada = Convert.ToBoolean((reader["isRevisada"]).ToString());
                            item.user_revisa = (reader["user_revisa"]).ToString();
                            item.user_proforma = (reader["proforma"]).ToString();
                            item.obs_revision = (reader["obs_revision"]).ToString();
                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            lista = lista.OrderBy(x => x.fechatramite).ToList();
            return lista;
        }
        public PROFORMA getProformaxCodigo(int numeroproforma)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.PROFORMA.SingleOrDefault(x => x.numerodeproforma == numeroproforma);
            }
        }
        public List<Search_Estudio> calularPrecio(List<Search_Estudio> item, int compañiaseguro)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string queryString = "select preciobruto,precioreal,codigomoneda from ESTUDIO_COMPANIA where codigoestudio='{0}' and codigocompaniaseguro='{1}'";


                foreach (var d in item)
                {
                    if (!d.isEditable)
                        using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                        {
                            SqlCommand command = new SqlCommand(string.Format(queryString, d.codigoestudio, compañiaseguro.ToString()), connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            try
                            {
                                if (reader.HasRows)
                                    while (reader.Read())
                                    {
                                        d.precio = Math.Round(Convert.ToDouble(reader["preciobruto"].ToString()), 2);
                                    }
                            }
                            finally
                            {
                                // Always call Close when done reading.
                                reader.Close();
                            }

                            connection.Close();
                        }

                }
                return item;
            }

        }

        public List<DETALLEPROFORMA> getDetalleProformaxCodigo(int numeroproforma)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.DETALLEPROFORMA.Where(x => x.numerodeproforma == numeroproforma).ToList();
            }
        }
        public void setCancelarProforma(int numeroproforma)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var proforma = db.PROFORMA.Where(x => x.numerodeproforma == numeroproforma).SingleOrDefault();
                proforma.estado = "RECHAZADA";
                db.SaveChanges();
            }
        }
        public CARTAGARANTIA getCartaxCodigo(string codigocarta, int codigopaciente)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == codigocarta && x.codigopaciente == codigopaciente);
            }
        }
        public int getPacientexCarta(string codigocarta)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var lista = db.CARTAGARANTIA.Where(x => x.codigocartagarantia == codigocarta).ToList();
                if (lista.Count == 1)
                    return lista.First().codigopaciente;
                else
                    return 0;
            }
        }
        public List<ESTUDIO_CARTAGAR> getDetalleCartaxCodigo(string codigocarta, int codigopaciente)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.ESTUDIO_CARTAGAR.Where(x => x.codigocartagarantia == codigocarta && x.codigopaciente == codigopaciente).ToList();
            }
        }
        public List<VisorCarta> getvisoCartas(int condicion, int unidad, int dia, int mes, int anio, string inicio, string apellidos, string fin)
        {
            List<VisorCarta> lista = new List<VisorCarta>();
            string queryString = @"EXEC [dbo].[visordecartas] '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, condicion, unidad, dia, mes, anio, inicio, apellidos, fin), connection);
                    connection.Open();
                    command.CommandTimeout = 200;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new VisorCarta();
                            item.Tramite = Convert.ToDateTime((reader["Tramite"]).ToString());
                            item.ID = Convert.ToInt32((reader["ID"]).ToString());
                            item.Paciente = ((reader["Paciente"]).ToString());
                            item.Estudio = ((reader["Estudio"]).ToString());
                            item.Telefono = ((reader["Telefono"]).ToString());
                            item.Aseguradora = ((reader["Aseguradora"]).ToString());
                            item.Cob = Convert.ToDouble((reader["% Cob."]).ToString());
                            item.Estado = ((reader["Estado"]).ToString());
                            item.FEC_APROB = Convert.ToDateTime((reader["FEC. APROB."]).ToString());
                            if (reader["Observacion"] != DBNull.Value)
                                item.Observacion = ((reader["Observacion"]).ToString());
                            if (reader["Codigo"] != DBNull.Value)
                                item.Codigo = ((reader["Codigo"]).ToString());
                            if (reader["Usuario"] != DBNull.Value)
                                item.Usuario = ((reader["Usuario"]).ToString());
                            if (reader["fec_update"] != DBNull.Value)
                                item.fec_update = Convert.ToDateTime((reader["fec_update"]).ToString());
                            if (reader["user_update"] != DBNull.Value)
                                item.user_update = ((reader["user_update"]).ToString());
                            if (reader["user_cita"] != DBNull.Value)
                                item.user_cita = ((reader["user_cita"]).ToString());
                            if (reader["razonsocial"] != DBNull.Value)
                                item.Clinica = ((reader["razonsocial"]).ToString());
                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }

        public List<RevisionCarta> getRevisionCartas(string fecha)
        {
            List<RevisionCarta> lista = new List<RevisionCarta>();
            string queryString = @"select 
cg.codigodocadjunto,cg.estadocarta,cg.codigocartagarantia,cg.codigocartagarantia2, p.apellidos+' '+p.nombres paciente,cs.descripcion,convert(varchar(20),c.fechareserva,103)fecha,u.ShortName,fec_update,isnull(ut.ShortName,'-') updater, isnull(up.ShortName,'-') proforma,isnull(uc.ShortName,'-') cita,cg.codigopaciente
from CITA c 
inner join EXAMENXCITA ec on c.numerocita=ec.numerocita
inner join PACIENTE p on c.codigopaciente =p.codigopaciente
inner join COMPANIASEGURO cs on c.codigocompaniaseguro = cs.codigocompaniaseguro
inner join CARTAGARANTIA cg on c.codigocartagarantia=cg.codigocartagarantia
inner join USUARIO u on cg.codigousuario=u.codigousuario
left join USUARIO ut on cg.user_update=ut.codigousuario
left join PROFORMA pro on cg.numero_proforma=pro.numerodeproforma
left join USUARIO up on pro.codigousuario=up.codigousuario
left join USUARIO uc on c.codigousuario=uc.codigousuario
where
convert(date,c.fechareserva)=convert(date,'" + fecha + @"') and cg.estadocarta='CITADA' and ec.codigoestudio like '1%'  and  (cg.isRevisada<>1 or cg.obs_revision<>'') and ec.estadoestudio<>'X'
group by cg.codigodocadjunto,cg.estadocarta,cg.codigocartagarantia,cg.codigocartagarantia2, p.apellidos+' '+p.nombres ,cs.descripcion,c.fechareserva,u.ShortName,fec_update,isnull(ut.ShortName,'-'),cg.codigopaciente,isnull(up.ShortName,'-'),isnull(uc.ShortName,'-') order by c.fechareserva
";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new RevisionCarta();
                            item.isAdjunto = (reader["codigodocadjunto"]).ToString() != "";
                            item.estado = ((reader["estadocarta"]).ToString());
                            item.codigocartagarantia = ((reader["codigocartagarantia"]).ToString());
                            item.codigocartagarantia2 = ((reader["codigocartagarantia2"]).ToString());
                            item.paciente = ((reader["paciente"]).ToString());
                            item.idpaciente = Convert.ToInt32((reader["codigopaciente"]).ToString());
                            item.fec_cita = Convert.ToDateTime((reader["fecha"]).ToString());
                            item.aseguradora = ((reader["descripcion"]).ToString());
                            item.iscitado = ((reader["fecha"])) != DBNull.Value;
                            item.user_tramita = ((reader["shortname"])).ToString();
                            item.user_update = ((reader["updater"])).ToString();
                            item.user_proforma = ((reader["proforma"])).ToString();
                            item.user_cita = ((reader["cita"])).ToString();
                            if (reader["fec_update"] != DBNull.Value)
                                item.fec_update = Convert.ToDateTime((reader["fec_update"]).ToString());
                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }


        public DOCESCANEADO getDocescaneado(string codigo)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.DOCESCANEADO.SingleOrDefault(x => x.codigodocadjunto == codigo);
            }
        }
        public bool isProductoActivo(int idcompania)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.SitedProducto.Where(x => x.codigocompaniaseguro == idcompania).AsParallel().ToList().Count() > 0;
            }
        }
        public bool isBeneficioActivo(int idcompania)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.Sunasa_Cobertura.Where(x => x.codigocompaniaseguro != 0).Select(x => x.codigocompaniaseguro).Distinct().ToList().Count() != 0;
            }
        }
        public List<SitedProducto> BuscarProducto(string nombre, string codigo, int compania)
        {
            List<SitedProducto> lista = new List<SitedProducto>();
            string queryString = @"select * from SitedProducto sp where sp.codigocompaniaseguro='{0}' and sp.IsEnabled='1' {1} {2}";
            if (nombre != "")
            {
                nombre = "and sp.descripcion like '%" + nombre + "%'";
            }
            if (codigo != "")
            {
                codigo = "and sp.SitedCodigoProducto like '%" + codigo + "%'";
            }
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, compania.ToString(), nombre, codigo), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new SitedProducto();

                            item.SitedProductoId = Convert.ToInt32((reader["SitedProductoId"]).ToString());
                            item.SitedCodigoProducto = ((reader["SitedCodigoProducto"]).ToString());
                            item.codigocompaniaseguro = Convert.ToInt32((reader["codigocompaniaseguro"]).ToString());
                            item.descripcion = ((reader["descripcion"]).ToString());
                            item.IsEnabled = Convert.ToBoolean((reader["IsEnabled"]).ToString());
                            item.SitedCodigoTedef = ((reader["SitedCodigoTedef"]).ToString());

                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public SitedProducto getProductoxNomenclatura(string codigo, int compania)
        {
            SitedProducto item = new SitedProducto();
            string queryString = @"select*from SitedProducto where codigocompaniaseguro='{0}' and SitedCodigoProducto='{1}' and IsEnabled='1' ";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, compania.ToString(), codigo), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            item.SitedProductoId = Convert.ToInt32((reader["SitedProductoId"]).ToString());
                            item.SitedCodigoProducto = ((reader["SitedCodigoProducto"]).ToString());
                            item.codigocompaniaseguro = Convert.ToInt32((reader["codigocompaniaseguro"]).ToString());
                            item.descripcion = ((reader["descripcion"]).ToString());
                            item.IsEnabled = Convert.ToBoolean((reader["IsEnabled"]).ToString());
                            item.SitedCodigoTedef = ((reader["SitedCodigoTedef"]).ToString());
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return item;
        }
        public SitedProducto getProductoxcodigo(int codigo, int compania)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.SitedProducto.Where(x => x.SitedProductoId == codigo && x.codigocompaniaseguro == compania && x.IsEnabled == true).FirstOrDefault();
            }
        }

        public List<CIE> BuscarCie(string nombre)
        {
            List<CIE> lista = new List<CIE>();
            string queryString = @"select*from CIE where descripcion like '%{0}%' or codigo like '%{0}%'";

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
                            var item = new CIE();

                            item.codigo = ((reader["codigo"]).ToString());
                            item.descripcion = ((reader["descripcion"]).ToString());

                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public CIE getCiexCodigo(string codigo)
        {
            CIE item = new CIE();
            string queryString = @"select*from CIE where codigo = '{0}'";

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
                            item.codigo = ((reader["codigo"]).ToString());
                            item.descripcion = ((reader["descripcion"]).ToString());

                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return item;
        }
        public List<Sunasa_Cobertura> BuscarBeneficio(string nombre, int compania, int idproducto)
        {
            List<Sunasa_Cobertura> lista = new List<Sunasa_Cobertura>();
            string queryString = "select*from Sunasa_Cobertura where IsActive='1' and  Nombre like '%{0}%' and codigocompaniaseguro={1} ";
            if (compania != 37)
            {
                compania = 0;
            }
            else
            {
                queryString += "and SitedProductoId='" + idproducto.ToString() + "' ";
            }
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, nombre, compania.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new Sunasa_Cobertura();

                            item.Sunasa_CoberturaId = Convert.ToInt32((reader["Sunasa_CoberturaId"]).ToString());
                            item.Tipo = ((reader["Tipo"]).ToString());
                            item.Subtipo = ((reader["Subtipo"]).ToString());
                            item.Nombre = ((reader["Nombre"]).ToString());
                            item.CodigoSited = ((reader["CodigoSited"]).ToString());
                            item.CodigoSited2 = ((reader["CodigoSited2"]).ToString());
                            item.IsActive = Convert.ToBoolean((reader["IsActive"]).ToString());
                            item.codigocompaniaseguro = Convert.ToInt32((reader["codigocompaniaseguro"]).ToString());
                            item.NombreSuSalud = ((reader["NombreSuSalud"]).ToString());
                            item.TipoDescripcion = ((reader["TipoDescripcion"]).ToString());
                            if (reader["SitedProductoId"] != DBNull.Value)
                                item.SitedProductoId = Convert.ToInt32((reader["SitedProductoId"]).ToString());
                            if (reader["SitedCodigoTedef"] != DBNull.Value)
                                item.SitedCodigoTedef = ((reader["SitedCodigoTedef"]).ToString());
                            if (reader["IsBloqueado"] != DBNull.Value)
                                item.IsBloqueado = Convert.ToBoolean((reader["IsBloqueado"]).ToString());

                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }
        public Sunasa_Cobertura getBeneficioxcodigo(int codigo)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                return db.Sunasa_Cobertura.Where(x => x.Sunasa_CoberturaId == codigo).FirstOrDefault();
            }
        }


        public List<CG_SolicitudCarta> insertLog(string codigo, string usuario, int tipoLog, string comentarios)
        {
            List<CG_SolicitudCarta> lista = new List<CG_SolicitudCarta>();
            string queryString = @"INSERT INTO CG_Log([usuario],[fecha],[tipo_log],[comentarios],[codigo])VALUES('{0}','{1}',{2},'{3}','{4}')";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, usuario, Tool.getDatetime().ToString("dd/MM/yyyy HH:mm:ss"), tipoLog.ToString(), comentarios, codigo), connection);
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }
            return lista;
        }
        public List<SitedsResult> listaAutorizacionesSited()
        {
            List<SitedsResult> lista = new List<SitedsResult>();
            string sqlSited1 = "SELECT cAsegCode as id,sAsegAPat+\" \"+ sAsegAMat+\", \"+sAsegName as asegurado ,sTituAPat+\" \"+sTituAMat+\", \"+sTituName as titular , sCntrName as contrato,cCarnNumb  as carnet,cafiltype as afiliacion, cestacode as estado,''  as poliza,cProdCode as producto FROM  DatosGenerales;";
            string sqlSited2 = "SELECT cAsegCode as id,sAsegAPat+\" \"+ sAsegAMat+\", \"+sAsegName as asegurado ,sTituAPat+\" \"+sTituAMat+\", \"+sTituName as titular , sCntrName as contrato,casegcode  as carnet,'' as afiliacion, '' as estado,nPoliNumb as poliza ,cProdCode as producto FROM  Seguros_DatosGenerales;";
            string sqlSited3 = "SELECT cAsegCode as id,sAsegAPat+\" \"+ sAsegAMat+\", \"+sAsegName as asegurado ,sTituAPat+\" \"+sTituAMat+\", \"+sTituName as titular , sCntrName as contrato,cCarnNumb  as carnet,'' as afiliacion, cestacode as estado,'' as poliza,cProdCode as producto FROM  RAM_DatosGenerales;";
            string sqlSited4 = "SELECT SSTC_DATOSGENERALES.CO_ASEGCODE AS id, AP_ASEGAPAT+' '+AP_ASEGAMAT+', '+NO_ASEGNAME AS asegurado, AP_TITUAPAT+' '+AP_TITUAMAT+', '+NO_TITUNAME AS titular, SSTC_DATOSGENERALES.DE_CNTRNAME AS contrato, SSTC_DATOSGENERALES.DE_CARNNUMB AS carnet, SSTC_DATOSGENERALES.CO_AFICODE AS afiliacion, SSTC_DATOSGENERALES.NU_POLIZA AS poliza, SSTC_DATOSGENERALES.NU_CERTIFICADO AS certificado, SSTD_COBERTURA_ACRED.CO_PRODCODE  as producto FROM SSTC_DATOSGENERALES INNER JOIN SSTD_COBERTURA_ACRED ON (SSTC_DATOSGENERALES.CO_AUTOCODE = SSTD_COBERTURA_ACRED.CO_AUTOCODE) AND (SSTC_DATOSGENERALES.CO_ASEGCODE = SSTD_COBERTURA_ACRED.CO_ASEGCODE) AND (SSTC_DATOSGENERALES.CO_IAFASCODE = SSTD_COBERTURA_ACRED.CO_IAFASCODE);";
            #region SitedsV9
            using (OleDbConnection con = new OleDbConnection(Tool.connectionStringSitedsV9))
            {
                try
                {
                    OleDbCommand command = new OleDbCommand(sqlSited1, con);
                    con.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SitedsResult item = new SitedsResult();
                        item.id = reader["id"].ToString();
                        item.asegurado = reader["asegurado"].ToString();
                        item.titular = reader["titular"].ToString();
                        item.contrato = reader["contrato"].ToString();
                        item.carnet = reader["carnet"].ToString();
                        int numero = 0;
                        if (int.TryParse(reader["afiliacion"].ToString(), out numero))
                        {
                            var afiliacion = new UtilDAO().getTipoAfiliacion().SingleOrDefault(x => x.codigo == numero);
                            if (afiliacion != null)
                            {
                                item.idafiliacion = numero;
                                item.afiliacion = afiliacion.nombre;
                            }
                            else
                            {
                                item.idafiliacion = 1;
                                item.afiliacion = "Regular";
                            }
                        }
                        else
                        {
                            if (reader["afiliacion"].ToString() == "I")
                            {
                                item.idafiliacion = 3;
                                item.afiliacion = "Potestativa (Independiente)";
                            }
                        }
                        item.poliza = "";
                        item.producto = reader["producto"].ToString();
                        lista.Add(item);
                    }
                    command = new OleDbCommand(sqlSited2, con);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SitedsResult item = new SitedsResult();
                        item.id = reader["id"].ToString();
                        item.asegurado = reader["asegurado"].ToString();
                        item.titular = reader["titular"].ToString();
                        item.contrato = reader["contrato"].ToString();
                        item.carnet = reader["carnet"].ToString();
                        item.idafiliacion = null;
                        item.afiliacion = "";
                        item.poliza = reader["poliza"].ToString();
                        item.producto = reader["producto"].ToString();
                        lista.Add(item);
                    }
                    command = new OleDbCommand(sqlSited3, con);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SitedsResult item = new SitedsResult();
                        item.id = reader["id"].ToString();
                        item.asegurado = reader["asegurado"].ToString();
                        item.titular = reader["titular"].ToString();
                        item.contrato = reader["contrato"].ToString();
                        item.carnet = reader["carnet"].ToString();
                        item.idafiliacion = null;
                        item.afiliacion = "";
                        item.poliza = "";
                        item.producto = reader["producto"].ToString();
                        lista.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            #endregion

            #region SitedsV10
            using (OleDbConnection con = new OleDbConnection(Tool.connectionStringSiteds))
            {
                try
                {
                    OleDbCommand command = new OleDbCommand(sqlSited4, con);
                    con.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SitedsResult item = new SitedsResult();
                        item.id = reader["id"].ToString();
                        item.asegurado = reader["asegurado"].ToString();
                        item.titular = reader["titular"].ToString();
                        item.contrato = reader["contrato"].ToString();
                        item.carnet = reader["carnet"].ToString();
                        int numero = 0;
                        if (int.TryParse(reader["afiliacion"].ToString(), out numero))
                        {
                            var afiliacion = new UtilDAO().getTipoAfiliacion().SingleOrDefault(x => x.codigo == numero);
                            if (afiliacion != null)
                            {
                                item.idafiliacion = numero;
                                item.afiliacion = afiliacion.nombre;
                            }
                            else
                            {
                                item.idafiliacion = 1;
                                item.afiliacion = "Regular";
                            }
                        }
                        else
                        {
                            item.idafiliacion = 1;
                            item.afiliacion = "Regular";
                        }

                        item.poliza = reader["poliza"].ToString();
                        item.certificado = reader["certificado"].ToString();
                        item.producto = reader["producto"].ToString();
                        lista.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            #endregion
            return lista;

        }

        public void deleteAutorizacionesSited(string id)
        {
            List<SitedsResult> lista = new List<SitedsResult>();
            string sqlSited1 = "delete FROM  DatosGenerales where cAsegCode=\"" + id + "\" ;";
            string sqlSited2 = "delete FROM Seguros_DatosGenerales where cAsegCode=\"" + id + "\" ;";
            string sqlSited3 = "delete FROM  RAM_DatosGenerales where cAsegCode=\"" + id + "\" ;";
            string sqlSited4 = "delete FROM  SSTC_DATOSGENERALES where CO_AsegCode=\"" + id + "\" ;";
            using (OleDbConnection con = new OleDbConnection(Tool.connectionStringSitedsV9))
            {
                try
                {
                    OleDbCommand command = new OleDbCommand(sqlSited1, con);
                    con.Open();
                    command.ExecuteNonQuery();
                    command = new OleDbCommand(sqlSited2, con);
                    command.ExecuteNonQuery();
                    command = new OleDbCommand(sqlSited3, con);
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            using (OleDbConnection con = new OleDbConnection(Tool.connectionStringSiteds))
            {
                try
                {
                    OleDbCommand command = new OleDbCommand(sqlSited4, con);
                    con.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }

        }


        public List<Sedacion_Carta> getSedaciones(string fecha)
        {
            List<Sedacion_Carta> lista = new List<Sedacion_Carta>();
            string queryString = "select ea.isRevisadoCarta,p.codigopaciente,a.fechayhora,ea.codigo,p.apellidos+' '+p.nombres paciente, es.nombreestudio,ea.estadoestudio, cs.descripcion,c.observacion ,ISNULL( [dbo].ufngetDocumentoSedacion('1',ea.codigopaciente,ea.numeroatencion),'') documentos from EXAMENXATENCION ea inner join ATENCION a on ea.numeroatencion=a.numeroatencion inner join CITA c on ea.numerocita=c.numerocita inner join ESTUDIO es on ea.codigoestudio=es.codigoestudio inner join PACIENTE p on ea.codigopaciente=p.codigopaciente inner join CARTAGARANTIA cg on c.codigocartagarantia = cg.codigocartagarantia inner join Sedacion se on ea.codigo=se.numeroexamen  inner join COMPANIASEGURO cs on  cg.codigocompaniaseguro = cs.codigocompaniaseguro where convert(date,a.fechayhora)='{0}' and ea.estadoestudio!='X' and c.sedacion=1 and SUBSTRING(ea.codigoestudio,8,2)!='99' and c.codigocartagarantia!=''";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(queryString, fecha), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new Sedacion_Carta();
                            item.revisado = Convert.ToBoolean(reader["isRevisadoCarta"].ToString());
                            item.fecha = Convert.ToDateTime(reader["fechayhora"].ToString());
                            item.codigo = Convert.ToInt32(reader["codigo"].ToString());
                            item.codigopaciente = Convert.ToInt32(reader["codigopaciente"].ToString());
                            item.paciente = (reader["paciente"].ToString());
                            item.estudio = (reader["nombreestudio"].ToString());
                            item.estado = (reader["estadoestudio"].ToString());
                            item.compania = (reader["descripcion"].ToString());
                            item.observCita = (reader["observacion"].ToString());
                            item.documentos = (reader["documentos"].ToString());

                            lista.Add(item);
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return lista;
        }

        public bool getisFile(string exam, out bool doc, out bool carta)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                doc = false;
                carta = false;
                int examen = int.Parse(exam);
                var isAdj = (from ea in db.EXAMENXATENCION
                             join ca in db.CARTAGARANTIA on ea.ATENCION.CITA.codigocartagarantia equals ca.codigocartagarantia into carta_join
                             from ca in carta_join.DefaultIfEmpty()
                             join esc in db.ESCANADMISION on ea.numeroatencion equals esc.numerodeatencion into esc_join
                             from esc in esc_join.DefaultIfEmpty()
                             where ea.codigo == examen
                             select new
                             {
                                 docEscan = esc.numerodeatencion != null ? true : false,
                                 docCarta = ca.codigodocadjunto != null ? ca.codigodocadjunto != "" ? true : false : false,
                             }).SingleOrDefault();

                doc = isAdj.docEscan;
                carta = isAdj.docCarta;
                if (doc || carta)
                    return true;
                else
                    return false;
            }

        }
    }
}

public class SitedsResult
{

    public string asegurado { get; set; }

    public string titular { get; set; }

    public string contrato { get; set; }

    public string carnet { get; set; }

    public int? idafiliacion { get; set; }
    public string afiliacion { get; set; }

    public string poliza { get; set; }

    public string id { get; set; }

    public string producto { get; set; }

    public string idproducto { get; set; }

    public string certificado { get; set; }
}
public class List_Proforma
{
    public int numeroproforma { get; set; }
    public DateTime fechaemision { get; set; }
    public string paciente { get; set; }
    public string titular { get; set; }
    public string aseguradora { get; set; }
    public string clinica { get; set; }
    public string contratante { get; set; }
    public string usuario { get; set; }

    public bool sedacion { get; set; }

    public string codigodocescaneado { get; set; }
    public string imgFile { get { return codigodocescaneado != "" ? "../../img/file.png" : ""; } }

    public bool hospitalizado { get; set; }

    public string imgHospital { get { return hospitalizado ? "../../img/check-red.png" : ""; } }
}
public class List_Carta
{
    public DateTime fechatramite { get; set; }

    public string codigocartagarantia { get; set; }

    public string codigocartagarantia2 { get; set; }

    public string paciente { get; set; }

    public string estadocarta { get; set; }

    public string aseguradora { get; set; }

    public string usuario { get; set; }

    public int codigopaciente { get; set; }

    public bool isrevisada { get; set; }
    public string imgRevisada
    {
        get
        {
            if (isrevisada)
            {
                if (obs_revision == "")
                    return "../../img/check.png";
                else
                    return "../../img/cancelar.png";
            }
            else
                return "";
        }
    }
    public string actualizador { get; set; }
    public string user_revisa { get; set; }

    public DateTime fechaupdate { get; set; }

    public string user_proforma { get; set; }

    public string obs_revision { get; set; }

    public string cobertura { get; set; }
}
public class Tipo_Afiliacion
{
    public int codigo { get; set; }
    public string nombre { get; set; }

}