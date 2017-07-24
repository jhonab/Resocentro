using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    class CitaDAO
    {
        public List<DetalleCita> getDetalleCita(string numerocita)
        {
            List<DetalleCita> lista = new List<DetalleCita>();
            string queryString = @"select c.codigomodalidad,c.codigocompaniaseguro,ec.codigomoneda,ec.codigoexamencita,es.codigoestudio,(convert(varchar(20),c.fechareserva,103)+' '+ convert(varchar(20),ec.horacita,108)) fecha,es.nombreestudio,ec.precioestudio,isnull((select eg.cobertura_det from ESTUDIO_CARTAGAR eg where eg.codigocartagarantia=c.codigocartagarantia and eg.codigoestudio=ec.codigoestudio),0) cob,isnull((select eg.descuento from ESTUDIO_CARTAGAR eg where eg.codigocartagarantia=c.codigocartagarantia and eg.codigoestudio=ec.codigoestudio),0) descuento  from EXAMENXCITA ec inner join CITA c on ec.numerocita=c.numerocita inner join ESTUDIO es on ec.codigoestudio=es.codigoestudio  where c.numerocita={0}";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, numerocita), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            lista.Add(new DetalleCita
                            {
                                codigomoneda = Convert.ToInt32(reader["codigomoneda"]),
                                codigoexamencita = Convert.ToInt32(reader["codigoexamencita"]),
                                codigomodalidad = Convert.ToInt32(reader["codigomodalidad"]),
                                codigocompaniaseguro = Convert.ToInt32(reader["codigocompaniaseguro"]),
                                fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
                                nombreestudio = (reader["nombreestudio"]).ToString(),
                                codigoestudio = (reader["codigoestudio"]).ToString(),
                                preciobruto = Convert.ToDouble((reader["precioestudio"]).ToString()),
                                descuento = Convert.ToDouble((reader["descuento"]).ToString()),
                                cobertura = Convert.ToDouble((reader["cob"]).ToString()),
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                return lista;
            }
        }
        public List<CartaxCitar> getCartasxCitas(string estado)
        {
            List<CartaxCitar> listaCarta = new List<CartaxCitar>();
            string queryString = @"
SELECT  
case when cg.isRevisada is null then 
0
else
cg.isRevisada
end Revisada,
cg.codigocartagarantia codigo,
p.codigopaciente idpaciente,
p.apellidos+' '+p.nombres Paciente,
cg.fechatramite Inicio,
cg.fechaprobacion Aprobacion,
p.telefono Telefono,
p.celular Celular,
(select count(*) from AUDITORIA_CARTAS_GARANTIA ac where ac.numerodecarta=cg.codigocartagarantia) Llamadas,
cs.descripcion Aseguradora,
cg.estadocarta estadoCarta,
case when cg.codigodocadjunto is null then 
''
else
cg.codigodocadjunto
end Adjunto,
cg.codigoclinica,
cg.monto,
cg.seguimiento,
cg.cmp,
cg.codigocompaniaseguro,
cg.ruc,
cg.cobertura
FROM CARTAGARANTIA cg 
INNER JOIN PACIENTE p ON cg.codigopaciente=p.codigopaciente
INNER JOIN  COMPANIASEGURO cs ON cg.codigocompaniaseguro=cs.codigocompaniaseguro
WHERE  cg.isCaducada is null
AND convert(date,fechatramite)>=CONVERT(date,DATEADD(MONTH,-2,GETDATE()))
and cg.estadocarta ='{0}'
and (select top(1)ce.codigoestudio from ESTUDIO_CARTAGAR ce where ce.codigopaciente=cg.codigopaciente and ce.codigocartagarantia=cg.codigocartagarantia) like '1%'
";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, estado), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            listaCarta.Add(new CartaxCitar
                            {
                                codigocarta = (reader["codigo"]).ToString(),
                                idpaciente = Convert.ToInt32(reader["idpaciente"]),
                                paciente = (reader["Paciente"]).ToString(),
                                inicio = Convert.ToDateTime(reader["Inicio"]),
                                aprobacion = Convert.ToDateTime(reader["Aprobacion"]),
                                telefono = (reader["Telefono"]).ToString(),
                                celular = (reader["Celular"]).ToString(),
                                llamadas = Convert.ToInt32(reader["Llamadas"]),
                                aseguradora = (reader["Aseguradora"]).ToString(),
                                estado = (reader["estadoCarta"]).ToString(),
                                adjunto = (reader["Adjunto"]).ToString(),
                                isrevisada = Convert.ToBoolean(reader["Revisada"]),
                                codigoclinica = reader["codigoclinica"] != DBNull.Value ? Convert.ToInt32(reader["codigoclinica"]) : 0,
                                monto = Convert.ToDouble(reader["monto"]),
                                comentarios = (reader["seguimiento"]).ToString(),
                                cmp = (reader["cmp"]).ToString(),
                                codigocompaniaseguro = Convert.ToInt32(reader["codigocompaniaseguro"]),
                                ruc = (reader["ruc"]).ToString(),
                                cobertura = Convert.ToDouble(reader["cobertura"]),
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                return listaCarta;
            }
        }
        public List<CitarxConfirmar> getCitasxConfirmar(string fecha)
        {
            List<CitarxConfirmar> listaCarta = new List<CitarxConfirmar>();
            string queryString = @"
select c.sedacion,isnull(c.codigocartagarantia,'')codigocartagarantia,c.numerocita,p.codigopaciente, p.apellidos+' '+p.nombres pac,p.telefono,p.celular,p.email,c.observacion,(select count(*) from Confirmacion_Cita cc where cc.numerocita=c.numerocita and cc.tipo=0 )llamadas,convert(varchar(20),min(ec.horacita),108) hora,(select nombreequipo from equipo where codigoequipo=min(ec.codigoequipo)) equipo from cita c inner join EXAMENXCITA ec on c.numerocita=ec.numerocita inner join PACIENTE p on c.codigopaciente=p.codigopaciente where convert(date,c.fechareserva)='{0}' and c.codigounidad=1 and ec.estadoestudio not in ('X','R') and c.isConfirmado is null  group by c.sedacion,c.codigocartagarantia,c.numerocita,p.codigopaciente, p.apellidos+' '+p.nombres ,p.telefono,p.celular,p.email,c.observacion order by hora
";
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
                            listaCarta.Add(new CitarxConfirmar
                            {
                                sedacion = Convert.ToBoolean(reader["sedacion"].ToString()),
                                codigopaciente = Convert.ToInt32(reader["codigopaciente"]),
                                codigocartagarantia = (reader["codigocartagarantia"]).ToString(),
                                paciente = (reader["pac"]).ToString(),                               
                                telefono = (reader["telefono"]).ToString(),
                                email = (reader["email"]).ToString(),
                                celular = (reader["celular"]).ToString(),
                                observacion = (reader["observacion"]).ToString(),
                                llamadas = Convert.ToInt32(reader["llamadas"]),
                                hora = (reader["hora"]).ToString(),
                                equipo = (reader["equipo"]).ToString(),
                                numerocita = Convert.ToInt32(reader["numerocita"])
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                return listaCarta;
            }
        }
        public List<RegistroLlamada> getHistorialLlamadas(string codigocarta, int codigopaciente)
        {
            List<RegistroLlamada> listaCarta = new List<RegistroLlamada>();
            string queryString = @"select ac.fecha,ac.tel_registro,ac.mensaje,u.ShortName from AUDITORIA_CARTAS_GARANTIA ac inner join USUARIO u on ac.codigousuario=u.codigousuario where ac.numerodecarta='{0}' and ac.codigopaciente='{1}'
";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, codigocarta, codigopaciente), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            listaCarta.Add(new RegistroLlamada
                            {
                                fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
                                usuario = ((reader["shortname"]).ToString()),
                                mensaje = ((reader["mensaje"]).ToString()),
                                telefono = ((reader["tel_registro"]).ToString())
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                return listaCarta;
            }
        }
        public List<Turno_Horario> getListaAdminTurnos(string codigoequipo, int dia, int mes, int anio)
        {
            List<Turno_Horario> listaCarta = new List<Turno_Horario>();
            string queryString = @"SELECT h.codigohorario AS Id,CONVERT(CHAR(5),h.hora,108)AS Hora,h.bloquear AS E,ISNULL(p.apellidos,'') paciente,ISNULL(es.nombreestudio,'') estudio,h.turnomedico AS Turno FROM HORARIO h left join PACIENTE p on h.codigopaciente=p.codigopaciente left join ESTUDIO es on h.codigoestudio=es.codigoestudio  WHERE h.codigoequipo='{0}' AND DAY(h.fecha)='{1}' AND MONTH(h.fecha)='{2}' AND YEAR(h.fecha)='{3}' ORDER BY h.hora;";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, codigoequipo, dia, mes, anio), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            listaCarta.Add(new Turno_Horario
                            {
                                codigohorario = Convert.ToInt32((reader["id"]).ToString()),
                                hora = Convert.ToDateTime((reader["hora"]).ToString()),
                                isBloqueado = Convert.ToBoolean((reader["e"]).ToString()),
                                paciente = ((reader["paciente"]).ToString()),
                                estudio = ((reader["estudio"]).ToString()),
                                turno = ((reader["turno"]).ToString())
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                return listaCarta;
            }
        }
        /// <summary>
        /// Funcion que obtiene el precio de sedacion y contraste segun compañia y modalidad
        /// </summary>
        /// <param name="codigocompania">codigo de compañia aseguradora</param>
        /// <param name="modalidad">codigo modalidad</param>
        /// <param name="sucursal">codigo de empresa y sucursal concatenados (ejem: sede central "101" )</param>
        /// <param name="tipo">Tipo a obtener 1 sedacion , 0 contraste</param>
        /// <param name="moneda"> devuelve el tipo de moneda</param>
        /// <returns> devuelve el precio de la sedacion o contraste</returns>
        public double getPrecioSedacionContraste(string codigocompania, string sucursal, int modalidad, int tipo, out string moneda)
        {
            moneda = "";
            double precio = 0;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string querysedacion = "select top(1) ec.preciobruto,m.simbolo,ec.codigoestudio from ESTUDIO_COMPANIA ec inner join moneda m on ec.codigomoneda=m.codigomoneda where ec.codigoestudio in ( select codigoestudio from ESTUDIO where nombreestudio like '%sedacion%' and codigoestudio like '" + sucursal + "%' and codigomodalidad='" + modalidad.ToString() + "') and codigocompaniaseguro='" + codigocompania.ToString() + "' order by ec.preciobruto desc";
                string querycontraste = "select top(1) ec.preciobruto,m.simbolo,ec.codigoestudio from ESTUDIO_COMPANIA ec inner join moneda m on ec.codigomoneda=m.codigomoneda where ec.codigoestudio in ( select codigoestudio from ESTUDIO where nombreestudio like '%contraste%' and codigoestudio like '" + sucursal + "%' and codigomodalidad='" + modalidad.ToString() + "') and codigocompaniaseguro='" + codigocompania.ToString() + "' order by ec.preciobruto desc";
                string queryString = "";
                if (tipo == 1)
                    queryString = querysedacion;
                else
                    queryString = querycontraste;
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            precio = Convert.ToDouble(reader["preciobruto"]);
                            moneda = reader["simbolo"].ToString();
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }
            return Math.Round(precio, 2);
        }
        public void insertRegistroLlamada(AUDITORIA_CARTAS_GARANTIA item)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                item.fecha = Tool.getDatetime(); ;
                db.AUDITORIA_CARTAS_GARANTIA.Add(item);
                db.SaveChanges();
                new CartaDAO().insertLog(item.numerodecarta, item.codigousuario, (int)Tipo_Log.Insert, "Se registro una llamada al numero " + item.tel_registro);
            }
        }

        public int getCantEstudiosRestringidos(string fecha)
        {
            int cantidad = 0;
            string queryString = @"exec [dbo].[restriccion]'{0}'";
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
                            cantidad = Convert.ToInt32(reader["CANTI"].ToString());
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                return cantidad;
            }
        }
        /// <summary>
        /// Obtiene la cantidad minima y maxima de estudios permitidos por dia
        /// </summary>
        /// <param name="tipo">0 devuelve el mínimo, 1 devuelve el máximo</param>
        /// <returns>0 devuelve el mínimo, 1 devuelve el máximo</returns>
        public int getCantEstudioPermitidos(int tipo)
        {
            int cantidadminima = 0;
            int cantidadmaxima = 0;
            string queryString = @"exec [dbo].[controlderestriccion]";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            cantidadminima = Convert.ToInt32(reader["INI"].ToString());
                            cantidadmaxima = Convert.ToInt32(reader["FIN"].ToString());
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                if (tipo == 0)
                    return cantidadminima;
                else if (tipo == 1)
                    return cantidadmaxima;
                else
                    throw new ArgumentException("El parametro solo puede ser 0 o 1 ", "tipo");
            }
        }

        public bool isTurnoLibre(DateTime fecha, int equipo)
        {
            string queryString = "SELECT  numerocita FROM HORARIO WHERE numerocita IS NOT NULL AND convert(time,hora)=convert(time,'" + fecha.ToString("HH:mm") + "') AND codigoequipo=" + equipo.ToString() + " AND convert(date,fecha)=convert(date,'" + fecha.ToShortDateString() + "')";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        return !reader.HasRows;
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }
        }

        public bool addCita(CITA cita, List<Search_Estudio> estudios)
        {
            try
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                    {
                        cita.numerocita = db.EXAMENXCITA.Select(x => x.numerocita).AsParallel().Max() + 1;
                        db.CITA.Add(cita);
                        db.SaveChanges();
                        foreach (var item in estudios)
                        {
                            EXAMENXCITA est = new EXAMENXCITA();
                            est.numerocita = cita.numerocita;
                            est.codigopaciente = cita.codigopaciente;
                            est.horacita = item.turno.hora;
                            est.precioestudio = float.Parse(item.precio.ToString());
                            est.codigocompaniaseguro = cita.codigocompaniaseguro;
                            est.ruc = cita.ruc;
                            est.codigoequipo = item.turno.codigoequipo;
                            est.codigoclase = item.codigoclase;
                            est.codigoestudio = item.codigoestudio;
                            est.codigomodalidad = int.Parse(item.codigoestudio.Substring(3, 2));
                            est.codigounidad = int.Parse(item.codigoestudio.Substring(0, 1));
                            est.estadoestudio = "C";
                            est.codigomoneda = item.idmoneda;
                            string queryString = "UPDATE HORARIO SET numerocita='" + cita.numerocita + "',codigopaciente='" + cita.codigopaciente + "',codigoestudio='" + item.codigoestudio + "' WHERE codigohorario='" + item.turno.idHorario.ToString() + "' ;";
                            SqlCommand command = new SqlCommand(string.Format(queryString), connection);
                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                            }
                            finally
                            {
                                connection.Close();
                            }

                            db.EXAMENXCITA.Add(est);
                            db.SaveChanges();
                        }

                        if (cita.codigocartagarantia != null)
                        {
                            var carta = db.CARTAGARANTIA.Where(x => x.codigocartagarantia == cita.codigocartagarantia && x.codigopaciente == cita.codigopaciente).AsParallel().SingleOrDefault();

                            carta.isCitada = true;
                            carta.estadocarta = "CITADA";
                            db.SaveChanges();
                        }

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxTemporal.Show(ex.Message, "Error", 5, true);
                return false;
            }

        }


        //VISOR CITA
        public List<EmpresaSucursal> getEmpresaSucursal()
        {
            List<EmpresaSucursal> list = new List<EmpresaSucursal>();
            string queryString = @"select s.codigounidad,s.codigosucursal,(convert(varchar(10),s.codigounidad)+'0'+convert(varchar(10),s.codigosucursal)) codigo,(convert(varchar(10),s.codigounidad)+''+convert(varchar(10),s.codigosucursal)) codigoShort,e.nombre empresa, e.NombreSmall empresaShort,s.descripcion sucursal,s.ShortDesc sucursalShort,e.nombre+' - '+s.descripcion concatenado,e.NombreSmall+' - '+s.ShortDesc concatenadoShort from  SUCURSAL s inner join UNIDADNEGOCIO e on s.codigounidad=e.codigounidad order by s.codigounidad,s.codigosucursal";
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
                            list.Add(new EmpresaSucursal
                            {
                                codigounidad = Convert.ToInt32((reader["codigounidad"]).ToString()),
                                codigosucursal = Convert.ToInt32((reader["codigosucursal"]).ToString()),
                                codigo = Convert.ToInt32((reader["codigo"]).ToString()),
                                codigoShort = Convert.ToInt32((reader["codigoShort"]).ToString()),
                                empresa = ((reader["empresa"]).ToString()),
                                empresaShort = ((reader["empresaShort"]).ToString()),
                                sucursal = ((reader["sucursal"]).ToString()),
                                sucursalShort = ((reader["sucursalShort"]).ToString()),
                                concatenado = ((reader["concatenado"]).ToString()),
                                concatenadoShort = ((reader["concatenadoShort"]).ToString())
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                return list;
            }
        }
        public List<EquipoEmpresaSucursal> getEquipoEmpresaSucursal()
        {
            List<EquipoEmpresaSucursal> list = new List<EquipoEmpresaSucursal>();
            string queryString = @"select q.codigoequipo,s.codigounidad,s.codigosucursal,(convert(varchar(10),s.codigounidad)+'0'+convert(varchar(10),s.codigosucursal)) codigo,(convert(varchar(10),s.codigounidad)+''+convert(varchar(10),s.codigosucursal)) codigoShort,e.nombre empresa, e.NombreSmall empresaShort,s.descripcion sucursal,s.ShortDesc sucursalShort,e.nombre+' - '+s.descripcion concatenado,e.NombreSmall+' - '+s.ShortDesc concatenadoShort,q.nombreequipo equipo,q.ShortDesc equipoShort from  EQUIPO q inner join  SUCURSAL s on q.codigosucursal2=s.codigosucursal and q.codigounidad2=s.codigounidad inner join UNIDADNEGOCIO e on s.codigounidad=e.codigounidad where q.estado=1 order by s.codigounidad,s.codigosucursal, q.codigoequipo 
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
                            list.Add(new EquipoEmpresaSucursal
                            {
                                codigoequipo = Convert.ToInt32((reader["codigoequipo"]).ToString()),
                                codigounidad = Convert.ToInt32((reader["codigounidad"]).ToString()),
                                codigosucursal = Convert.ToInt32((reader["codigosucursal"]).ToString()),
                                codigo = Convert.ToInt32((reader["codigo"]).ToString()),
                                codigoShort = Convert.ToInt32((reader["codigoShort"]).ToString()),
                                empresa = ((reader["empresa"]).ToString()),
                                empresaShort = ((reader["empresaShort"]).ToString()),
                                sucursal = ((reader["sucursal"]).ToString()),
                                sucursalShort = ((reader["sucursalShort"]).ToString()),
                                concatenado = ((reader["concatenado"]).ToString()),
                                concatenadoShort = ((reader["concatenadoShort"]).ToString()),
                                equipo = ((reader["equipo"]).ToString()),
                                equipoShort = ((reader["equipoShort"]).ToString()),
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                return list;
            }
        }
        public List<VisorCita> getVisorCita(int condicion, int modalidad, int dia, int mes, int ano, int equipo, int unidad, int sede)
        {
            List<VisorCita> list = new List<VisorCita>();
            string queryString = "exec dbo.visordecitas " + condicion.ToString() + "," + modalidad.ToString() + "," + dia.ToString() + "," + mes.ToString() + "," + ano.ToString() + "," + equipo.ToString() + "," + unidad.ToString() + "," + sede.ToString();
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
                            list.Add(new VisorCita
                            {
                                hora = Convert.ToDateTime((reader["HORA"]).ToString()),
                                turno = ((reader["MED."]).ToString()),
                                sedacion = Convert.ToBoolean((reader["S"]).ToString()),
                                paciente = ((reader["PACIENTE"]).ToString()),
                                estudio = ((reader["ESTUDIO"]).ToString()),
                                estado = ((reader["E"]).ToString()),
                                cita = Convert.ToInt32((reader["CITA"]).ToString()),
                                equipo = Convert.ToInt32((reader["EQ"]).ToString()),
                                nombreequipo = ((reader["EQU"]).ToString()),
                                observacion = ((reader["OBS"]).ToString()),
                                idpaciente = ((reader["idpaciente"]).ToString()),
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                return list;
            }
        }

        public bool EliminarDetalleCita(int numeroDetalle)
        {
            string sql = "delete from EXAMENXCITA WHERE  codigoexamencita ='{0}'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(sql, numeroDetalle), connection);
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxTemporal.Show(ex.Message, "Error", 10, true);
                        return false;
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                    return true;
                }

            }
        }

        public bool UpdateEstadoDetalleCita(int numeroDetalle, string estado,string motivo="",string usuario="")
        {
            string sql = "update EXAMENXCITA set estadoestudio='{1}' {2} WHERE  codigoexamencita ='{0}'";
            string addsql = ", motivo='{0}',fecha_cancela=getdate(), usu_cancela='{1}'";
            if (motivo != "" && usuario != "")
                addsql = string.Format(addsql, motivo, usuario);
            else
                addsql = "";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(sql, numeroDetalle, estado,addsql), connection);
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxTemporal.Show(ex.Message, "Error", 10, true);
                        return false;
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                    return true;
                }

            }
        }
        public bool UpdateEstadoDetalleAtencion(string numeroDetalle, string estado)
        {
            string sql = "update EXAMENXATENCION set estadoestudio='{1}' WHERE  codigo ='{0}'";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(sql, numeroDetalle, estado), connection);
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxTemporal.Show(ex.Message, "Error", 10, true);
                        return false;
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                    return true;
                }

            }
        }
        public int getCodigoDetalleAtencion(string cita, string estudio)
        {
            string sql = "select codigo from EXAMENXATENCION where numerocita ={0} and codigoestudio={1}";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(sql, cita, estudio), connection);
                    connection.Open();
                    SqlDataReader reader =command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            return Convert.ToInt32((reader["codigo"].ToString()));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxTemporal.Show(ex.Message, "Error", 10, true);
                        return 0;
                    }
                    finally
                    {
                        reader.Close();
                        // Always call Close when done reading.
                        connection.Close();
                    }
                    return 0;
                }

            }
        }
        public int getCodigoDetalleCIta(string cita, string estudio)
        {
            string sql = "select codigoexamencita from EXAMENXCITA where numerocita ={0} and codigoestudio={1}";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(sql, cita, estudio), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            return Convert.ToInt32((reader["codigoexamencita"].ToString()));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxTemporal.Show(ex.Message, "Error", 10, true);
                        return 0;
                    }
                    finally
                    {
                        reader.Close();
                        // Always call Close when done reading.
                        connection.Close();
                    }
                    return 0;
                }

            }
        }
    }
}
