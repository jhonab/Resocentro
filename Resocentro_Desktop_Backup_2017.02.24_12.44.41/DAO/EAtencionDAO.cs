using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.DAO
{
    class EAtencionDAO
    {
        public EXAMENXATENCION getEAtencionxCodigo(int codigo)
        {
            EXAMENXATENCION item = new EXAMENXATENCION();
            string query = "select codigo,numeroatencion,codigopaciente,numerocita,codigocompaniaseguro,ruc,codigoequipo,horaatencion,codigoestudio,codigoclase,codigomodalidad,codigounidad,estadoestudio,turnomedico,prioridad from EXAMENXATENCION where codigo={0}";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, codigo), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            item.codigo = Convert.ToInt32((reader["codigo"]).ToString());
                            item.numeroatencion = Convert.ToInt32((reader["numeroatencion"]).ToString());
                            item.codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString());
                            item.numerocita = Convert.ToInt32((reader["numerocita"]).ToString());
                            item.codigocompaniaseguro = Convert.ToInt32((reader["codigocompaniaseguro"]).ToString());
                            item.ruc = ((reader["ruc"]).ToString());
                            item.codigoequipo = Convert.ToInt32((reader["codigoequipo"]).ToString());
                            item.horaatencion = Convert.ToDateTime((reader["horaatencion"]).ToString());
                            item.codigoestudio = ((reader["codigoestudio"]).ToString());
                            item.codigoclase = Convert.ToInt32((reader["codigoclase"]).ToString());
                            item.codigomodalidad = Convert.ToInt32((reader["codigomodalidad"]).ToString());
                            item.codigounidad = Convert.ToInt32((reader["codigounidad"]).ToString());
                            item.estadoestudio = ((reader["estadoestudio"]).ToString());
                            item.turnomedico = ((reader["turnomedico"]).ToString());
                            item.prioridad = ((reader["prioridad"]).ToString());

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
        public EXAMENXATENCION getEAtencionxAtencion(int numeroatencion)
        {
            EXAMENXATENCION item = new EXAMENXATENCION();
            string query = "select top(1)codigo,numeroatencion,codigopaciente,numerocita,codigocompaniaseguro,ruc,codigoequipo,horaatencion,codigoestudio,codigoclase,codigomodalidad,codigounidad,estadoestudio,turnomedico,prioridad from EXAMENXATENCION where numeroatencion={0}";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(query, numeroatencion), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            item.codigo = Convert.ToInt32((reader["codigo"]).ToString());
                            item.numeroatencion = Convert.ToInt32((reader["numeroatencion"]).ToString());
                            item.codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString());
                            item.numerocita = Convert.ToInt32((reader["numerocita"]).ToString());
                            item.codigocompaniaseguro = Convert.ToInt32((reader["codigocompaniaseguro"]).ToString());
                            item.ruc = ((reader["ruc"]).ToString());
                            item.codigoequipo = Convert.ToInt32((reader["codigoequipo"]).ToString());
                            item.horaatencion = Convert.ToDateTime((reader["horaatencion"]).ToString());
                            item.codigoestudio = ((reader["codigoestudio"]).ToString());
                            item.codigoclase = Convert.ToInt32((reader["codigoclase"]).ToString());
                            item.codigomodalidad = Convert.ToInt32((reader["codigomodalidad"]).ToString());
                            item.codigounidad = Convert.ToInt32((reader["codigounidad"]).ToString());
                            item.estadoestudio = ((reader["estadoestudio"]).ToString());
                            item.turnomedico = ((reader["turnomedico"]).ToString());
                            item.prioridad = ((reader["prioridad"]).ToString());

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

        //        public HistoriaClinicaPaciente getHclinicaPaciente(int codigpaciente)
        //        {
        //            #region sql
        //            string sqlCita = @"
        //select c.citavip,ec.estadoestudio E,ec.numerocita cita,sedacion S,convert(varchar(20),c.fechareserva,103)+' '+convert(varchar(20),ec.horacita,108) Fecha,c.fechacreacion,
        //es.nombreestudio,case when ec.codigomoneda='1' then 'S/.'+convert(varchar(200),ec.precioestudio) else '$.'+ convert(varchar(200),ec.precioestudio) end precio,convert(varchar(200),ISNULL(ecg.cobertura_det,'0.0'))+'%' cob,eq.nombreequipo equipo,REPLACE(REPLACE(REPLACE(c.observacion,CHAR(10),' ') ,CHAR(13),' ') ,'  ',' ') observacion ,us.ShortName,ISNULL(c.codigocartagarantia,'')carta,ch.razonsocial,es.codigoestudio,ec.codigoexamencita 
        // from EXAMENXCITA ec
        //inner join CITA c on ec.numerocita=c.numerocita
        //inner join ESTUDIO es on ec.codigoestudio=es.codigoestudio
        //inner join EQUIPO eq on ec.codigoequipo=eq.codigoequipo
        //inner join USUARIO us on c.codigousuario=us.codigousuario
        //inner join CLINICAHOSPITAL ch on c.codigoclinica=ch.codigoclinica
        //left join ESTUDIO_CARTAGAR ecg on c.codigocartagarantia=ecg.codigocartagarantia and c.codigopaciente=ecg.codigopaciente and substring(ec.codigoestudio,4,10)=substring(ecg.codigoestudio,4,10)
        //where ec.codigopaciente='{0}' ORDER BY ec.numerocita
        //";
        //            string sqlAdmin = @"
        //select ea.estadoestudio AS E,case when ci.estadocargo='I' then 'Informes' when ci.estadocargo='E' then 'Counter'  when ci.estadocargo='O'  then 'Paciente' else ''end 'ubicacion',a.fechayhora fecha,ea.numeroatencion atencion,ea.codigo examen,es.nombreestudio estudio,cs.descripcion aseguradora,ea.prioridad,me.apellidos+' '+me.nombres medico,us.Shortname,ea.turnomedico,ea.numerocita,es.codigoestudio from EXAMENXATENCION ea 
        //inner join ATENCION a on ea.numeroatencion=a.numeroatencion
        //inner join ESTUDIO es on ea.codigoestudio=es.codigoestudio
        //inner join COMPANIASEGURO cs on ea.codigocompaniaseguro=cs.codigocompaniaseguro
        //inner join MEDICOEXTERNO me on a.cmp=me.cmp
        //inner join USUARIO us on a.codigousuario=us.codigousuario
        //left join CARGOINFORMES ci on ea.codigo=ci.numeroestudio
        //where a.codigopaciente='{0}' order by ea.codigo
        //";
        //            string sqlCarta = @"select  isnull(c.numerocita,'') cita,cg.estadocarta,cg.fechatramite fecha,cg.codigocartagarantia id,ISNULL(cg.codigocartagarantia2,'') id2,cs.descripcion,convert(varchar(100),cg.cobertura)+'%' cobertura,cg.codigopaciente,isnull(cg.isRevisada,'0')isrevisada from CARTAGARANTIA cg 
        //inner join COMPANIASEGURO cs on cg.codigocompaniaseguro=cs.codigocompaniaseguro
        //left join CITA c on cg.codigocartagarantia=c.codigocartagarantia
        //where cg.codigopaciente='{0}' order by cg.fechatramite";
        //            string sqldocumento = @" select d.estado,u.nombre empresa,d.fechaemitio emision,d.fechadepago pago,CASE WHEN d.tipodocumento='01' THEN 'Factura' WHEN d.tipodocumento='03' THEN 'Boleta'  WHEN d.tipodocumento='07' THEN 'Nota Credito' else '' END AS documento,d.numerodocumento numero,case when d.tipomoneda='1' then 'S/.'+convert(varchar(200),d.total) else '$.'+ convert(varchar(200),d.total) end total ,ISNULL(lc.numerocita,'')cita,d.codigopaciente,d.ruc from DOCUMENTO d  
        // inner join UNIDADNEGOCIO u on d.codigounidad=u.codigounidad
        // left join LIBROCAJA lc on lc.numerodocumento=SUBSTRING(d.numerodocumento,5,7) AND lc.codigopaciente=d.codigopaciente
        // where d.codigopaciente='{0}'
        // --and  lc.codigopaciente='{0}'
        //  ORDER BY lc.fechacobranza;";

        //            string sqladquisicion = @"select ea.codigo,es.nombreestudio,ht.placauso, ISNULL(ht.sedacion,'') AS Sedacion,ISNULL(ht.contraste,'') AS Contraste,eq.ShortDesc equipo,CONVERT(CHAR(10),ht.fechaestudio,103)+' '+CONVERT(CHAR(5),ht.horaestudio,108) fecha, us.ShortName tecnologo,ea.numerocita cita,isnull(mi.shortName,'-') informa,isnull(mr.shortName,'-') revisa from HONORARIOTECNOLOGO ht
        // inner join EXAMENXATENCION ea on ht.codigohonorariotecnologo=ea.codigo
        // inner join equipo eq on ht.codigoequipo=eq.codigoequipo
        // inner join ESTUDIO es on ea.codigoestudio=es.codigoestudio
        // inner join USUARIO us on ht.tecnologoturno=us.siglas
        // left join INFORMEMEDICO i on ea.codigo=i.numeroinforme 
        // left join USUARIO mi on i.medicoinforma=mi.codigousuario
        // left join USUARIO mr on i.medicorevisa=mr.codigousuario
        // where ea.codigopaciente='{0}' order by ht.codigohonorariotecnologo";
        //            #endregion
        //            HistoriaClinicaPaciente item = new HistoriaClinicaPaciente();
        //            item.lstCita = new List<HCCita>();
        //            item.lstAdmision = new List<HCAdmision>();
        //            item.lstCarta = new List<HCCarta>();
        //            item.lstAdquisicion = new List<HCAdquisicion>();
        //            item.lstPagos = new List<HCPagos>();
        //            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
        //            {
        //                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
        //                {
        //                    SqlCommand command_cita = new SqlCommand(string.Format(sqlCita, codigpaciente.ToString()), connection);
        //                    connection.Open();
        //                    SqlDataReader reader = command_cita.ExecuteReader();
        //                    try
        //                    {
        //                        //CITA
        //                        while (reader.Read())
        //                        {
        //                            item.lstCita.Add(new HCCita()
        //                            {
        //                                estado = (reader["E"]).ToString(),
        //                                vip = Convert.ToBoolean(reader["citavip"]),
        //                                sedacion = Convert.ToBoolean(reader["s"]),
        //                                num_cita = Convert.ToInt32((reader["cita"]).ToString()),
        //                                fecha = Convert.ToDateTime((reader["Fecha"]).ToString()),
        //                                fechacreacion = Convert.ToDateTime((reader["fechacreacion"]).ToString()),
        //                                estudio = (reader["nombreestudio"]).ToString(),
        //                                codigoestudio = (reader["codigoestudio"]).ToString(),
        //                                precio = (reader["precio"]).ToString(),
        //                                codigoexamencita = (reader["codigoexamencita"]).ToString(),
        //                                cobertura = (reader["cob"]).ToString(),
        //                                equipo = (reader["equipo"]).ToString(),
        //                                obs = (reader["observacion"]).ToString(),
        //                                usuario = (reader["ShortName"]).ToString(),
        //                                clinica = (reader["razonsocial"]).ToString(),
        //                                carta = (reader["carta"]).ToString(),
        //                                isSeleccionado = false
        //                            });
        //                        }
        //                        //ADMISION
        //                        command_cita = new SqlCommand(string.Format(sqlAdmin, codigpaciente.ToString()), connection);
        //                        reader = command_cita.ExecuteReader();
        //                        while (reader.Read())
        //                        {
        //                            item.lstAdmision.Add(new HCAdmision
        //                            {
        //                                estado = (reader["E"]).ToString(),
        //                                ubicacion = (reader["ubicacion"]).ToString(),
        //                                fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
        //                                atencion = Convert.ToInt32((reader["atencion"]).ToString()),
        //                                examen = Convert.ToInt32((reader["examen"]).ToString()),
        //                                estudio = (reader["estudio"]).ToString(),
        //                                codigoestudio = (reader["codigoestudio"]).ToString(),
        //                                aseguradora = (reader["aseguradora"]).ToString(),
        //                                prioridad = (reader["prioridad"]).ToString(),
        //                                medico = (reader["medico"]).ToString(),
        //                                turno = (reader["turnomedico"]).ToString(),
        //                                usuario = (reader["ShortName"]).ToString(),
        //                                cita = Convert.ToInt32((reader["numerocita"]).ToString()),
        //                                isSeleccionado = false
        //                            });
        //                        }
        //                        //CARTA
        //                        command_cita = new SqlCommand(string.Format(sqlCarta, codigpaciente.ToString()), connection);
        //                        reader = command_cita.ExecuteReader();
        //                        while (reader.Read())
        //                        {
        //                            item.lstCarta.Add(new HCCarta
        //                            {
        //                                estado = (reader["estadocarta"]).ToString(),
        //                                fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
        //                                id = (reader["id"]).ToString(),
        //                                id2 = (reader["id2"]).ToString(),
        //                                aseguradora = (reader["descripcion"]).ToString(),
        //                                cobertura = (reader["cobertura"]).ToString(),
        //                                cita = Convert.ToInt32((reader["cita"]).ToString()),
        //                                codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString()),
        //                                isRevisada = Convert.ToBoolean(reader["isrevisada"]),
        //                                isSeleccionado = false
        //                            });
        //                        }
        //                        //ADQUISICION
        //                        command_cita = new SqlCommand(string.Format(sqladquisicion, codigpaciente.ToString()), connection);
        //                        reader = command_cita.ExecuteReader();
        //                        while (reader.Read())
        //                        {
        //                            item.lstAdquisicion.Add(new HCAdquisicion
        //                            {
        //                                examen = Convert.ToInt32((reader["codigo"]).ToString()),
        //                                estudio = (reader["nombreestudio"]).ToString(),
        //                                placas = Convert.ToInt32((reader["placauso"]).ToString()),
        //                                sedacion = Convert.ToBoolean((reader["Sedacion"]).ToString()),
        //                                contraste = Convert.ToBoolean((reader["Contraste"]).ToString()),
        //                                fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
        //                                tecnologo = (reader["tecnologo"]).ToString(),
        //                                equipo = (reader["equipo"]).ToString(),
        //                                cita = Convert.ToInt32((reader["cita"]).ToString()),
        //                                informa = (reader["informa"]).ToString(),
        //                                revisa = (reader["revisa"]).ToString(),
        //                                isSeleccionado = false
        //                            });
        //                        }
        //                        //DOCUMENTO
        //                        command_cita = new SqlCommand(string.Format(sqldocumento, codigpaciente.ToString()), connection);
        //                        reader = command_cita.ExecuteReader();
        //                        while (reader.Read())
        //                        {
        //                            item.lstPagos.Add(new HCPagos
        //                            {
        //                                estado = (reader["estado"]).ToString(),
        //                                empresa = (reader["empresa"]).ToString(),
        //                                emision = Convert.ToDateTime((reader["emision"]).ToString()),
        //                                pago = Convert.ToDateTime((reader["pago"]).ToString()),
        //                                documento = (reader["documento"]).ToString(),
        //                                numero = (reader["numero"]).ToString(),
        //                                total = (reader["total"]).ToString(),
        //                                ruc = (reader["ruc"]).ToString(),
        //                                cita = Convert.ToInt32((reader["cita"]).ToString()),
        //                                isSeleccionado = false,
        //                                codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString()),
        //                            });
        //                        }
        //                    }
        //                    finally
        //                    {
        //                        reader.Close();
        //                    }
        //                }
        //            }
        //            return item;
        //        }
        public async Task<List<HCCita>> getHclinicaPacienteCita(int codigpaciente)
        {
            string sqlCita = @"exec getCitasPaciente {0}";

            var lstCita = new List<HCCita>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command_cita = new SqlCommand(string.Format(sqlCita, codigpaciente.ToString()), connection);
                    connection.Open();
                    command_cita.Prepare();

                    try
                    {
                        //CITA
                        using (var reader = await command_cita.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                lstCita.Add(new HCCita()
                                {
                                    estado = (reader["E"]).ToString(),
                                    vip = Convert.ToBoolean(reader["citavip"]),
                                    sedacion = Convert.ToBoolean(reader["s"]),
                                    num_cita = Convert.ToInt32((reader["cita"]).ToString()),
                                    fecha = Convert.ToDateTime((reader["Fecha"]).ToString()),
                                    fechacreacion = Convert.ToDateTime((reader["fechacreacion"]).ToString()),
                                    estudio = (reader["nombreestudio"]).ToString(),
                                    codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString()),
                                    codigoestudio = (reader["codigoestudio"]).ToString(),
                                    precio = (reader["precio"]).ToString(),
                                    codigoexamencita = (reader["codigoexamencita"]).ToString(),
                                    cobertura = (reader["cob"]).ToString(),
                                    equipo = (reader["equipo"]).ToString(),
                                    obs = (reader["observacion"]).ToString(),
                                    usuario = (reader["ShortName"]).ToString(),
                                    clinica = (reader["razonsocial"]).ToString(),
                                    empresa = (reader["empresa"]).ToString(),
                                    carta = (reader["carta"]).ToString(),
                                    isSeleccionado = false
                                });
                            }
                        }

                    }
                    finally
                    {
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return lstCita;
        }
        public async Task<List<HCAdmision>> getHclinicaPacienteAdmision(int codigpaciente)
        {
            string sqlAdmin = @"exec getAtencionesPaciente {0}";

            var lstAdmision = new List<HCAdmision>();

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command_cita = new SqlCommand(string.Format(sqlAdmin, codigpaciente.ToString()), connection);
                    connection.Open();
                    command_cita.Prepare();
                    try
                    {
                        using (var reader = await command_cita.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                lstAdmision.Add(new HCAdmision
                                {
                                    estado = (reader["E"]).ToString(),
                                    ubicacion = (reader["ubicacion"]).ToString(),
                                    fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
                                    atencion = Convert.ToInt32((reader["atencion"]).ToString()),
                                    examen = Convert.ToInt32((reader["examen"]).ToString()),
                                    estudio = (reader["estudio"]).ToString(),
                                    codigoestudio = (reader["codigoestudio"]).ToString(),
                                    codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString()),
                                    aseguradora = (reader["aseguradora"]).ToString(),
                                    prioridad = (reader["prioridad"]).ToString(),
                                    medico = (reader["medico"]).ToString(),
                                    turno = (reader["turnomedico"]).ToString(),
                                    usuario = (reader["ShortName"]).ToString(),
                                    empresa = (reader["empresa"]).ToString(),
                                    cita = Convert.ToInt32((reader["numerocita"]).ToString()),
                                    isSeleccionado = false
                                });
                            }
                        }
                    }
                    finally
                    {
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return lstAdmision;
        }
        public async Task<List<HCCarta>> getHclinicaPacienteCarta(int codigpaciente)
        {
            string sqlCarta = @"exec getCartasPaciente {0}";
            var lstCarta = new List<HCCarta>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command_cita = new SqlCommand(string.Format(sqlCarta, codigpaciente.ToString()), connection);
                    connection.Open();
                    command_cita.Prepare();
                    try
                    {
                        using (var reader = await command_cita.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                lstCarta.Add(new HCCarta
                                {
                                    estado = (reader["estadocarta"]).ToString(),
                                    fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
                                    id = (reader["id"]).ToString(),
                                    id2 = (reader["id2"]).ToString(),
                                    aseguradora = (reader["descripcion"]).ToString(),
                                    cobertura = (reader["cobertura"]).ToString(),
                                    cita = Convert.ToInt32((reader["cita"]).ToString()),
                                    codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString()),
                                    isRevisada = Convert.ToBoolean(reader["isrevisada"]),
                                    obs_revision = (reader["obs_revision"].ToString()),
                                    empresa = (reader["empresa"]).ToString(),
                                    isSeleccionado = false
                                });
                            }
                        }
                    }
                    finally
                    {
                        connection.Dispose();
                        connection.Close();
                    }

                }
            }
            return lstCarta;
        }
        public async Task<List<HCPagos>> getHclinicaPacienteDocumento(int codigpaciente)
        {
            string sqldocumento = @" exec getDocumentosPaciente {0}";

            var lstPagos = new List<HCPagos>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command_cita = new SqlCommand(string.Format(sqldocumento, codigpaciente.ToString()), connection);
                    connection.Open();
                    command_cita.Prepare();

                    try
                    {
                        using (var reader = await command_cita.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                lstPagos.Add(new HCPagos
                                {
                                    estado = (reader["estado"]).ToString(),
                                    empresa = (reader["empresa"]).ToString(),
                                    codigoempresa = (reader["codigounidad"]).ToString(),
                                    codigosede = (reader["codigosucursal"]).ToString(),
                                    emision = Convert.ToDateTime((reader["emision"]).ToString()),
                                    pago = Convert.ToDateTime((reader["pago"]).ToString()),
                                    documento = (reader["documento"]).ToString(),
                                    numero = (reader["numero"]).ToString(),
                                    total = (reader["total"]).ToString(),
                                    ruc = (reader["ruc"]).ToString(),
                                    pathFile = (reader["pathFile"]).ToString(),
                                    cita = Convert.ToInt32((reader["cita"]).ToString()),
                                    isSeleccionado = false,
                                    codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString()),
                                });
                            }
                        }
                    }
                    finally
                    {
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return lstPagos;
        }
        public async Task<List<HCAdquisicion>> getHclinicaPacienteTecnologo(int codigpaciente)
        {
            string sqladquisicion = @"exec getAdquisicionesPaciente {0}";

            var lstAdquisicion = new List<HCAdquisicion>();

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command_cita = new SqlCommand(string.Format(sqladquisicion, codigpaciente.ToString()), connection);
                    connection.Open();
                    command_cita.Prepare();
                    try
                    {
                        using (var reader = await command_cita.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                lstAdquisicion.Add(new HCAdquisicion
                                {
                                    examen = Convert.ToInt32((reader["codigo"]).ToString()),
                                    estudio = (reader["nombreestudio"]).ToString(),
                                    placas = Convert.ToInt32((reader["placauso"]).ToString()),
                                    sedacion = Convert.ToBoolean((reader["Sedacion"]).ToString()),
                                    contraste = Convert.ToBoolean((reader["Contraste"]).ToString()),
                                    fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
                                    tecnologo = (reader["tecnologo"]).ToString(),
                                    equipo = (reader["equipo"]).ToString(),
                                    cita = Convert.ToInt32((reader["cita"]).ToString()),
                                    informa = (reader["informa"]).ToString(),
                                    empresa = (reader["empresa"]).ToString(),
                                    revisa = (reader["revisa"]).ToString(),
                                    isSeleccionado = false
                                });
                            }
                        }
                    }
                    finally
                    {
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return lstAdquisicion;
        }

        public DetalleHCAdquisicion getDetalleAdquisicion(int examen)
        {
            DetalleHCAdquisicion item = new DetalleHCAdquisicion();

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string sqlquery = "select u.ShortName,e.nombreequipo,ht.placauso from HONORARIOTECNOLOGO ht inner join EQUIPO e on ht.codequiporealizo=e.codigoequipo inner join USUARIO u on ht.tecnologoturno=u.siglas where ht.codigohonorariotecnologo='{0}'";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    try
                    {
                        //Tecnologo
                        SqlCommand command_cita = new SqlCommand(string.Format(sqlquery, examen.ToString()), connection);
                        connection.Open();
                        command_cita.Prepare();
                        SqlDataReader reader = command_cita.ExecuteReader();

                        while (reader.Read())
                        {
                            item.tecnologoName = reader["ShortName"].ToString();
                            item.equipo = reader["nombreequipo"].ToString();
                            item.placasuso = reader["placauso"].ToString();
                        }
                        //Tecnicas
                        reader.Close();
                        sqlquery = "select convert(varchar(200),(substring(seriesdate,7,2)+'/'+substring(seriesdate,5,2)+'/'+substring(seriesdate,1,4)+' '+substring(seriestime,1,2)+':'+substring(seriestime,3,2)+':'+substring(seriestime,5,2))) fecha,StudyDescription,SeriesDescription,SeriesNumber,numberOfSeriesRelatedInstances from INTEGRACION_SERIES where AccessionNumber='{0}'";
                        command_cita.CommandText = string.Format(sqlquery, examen.ToString());
                        command_cita.Prepare();
                        reader = command_cita.ExecuteReader();
                        item.tecnicasTecnologo = new List<DetalleHCAdquision_Tecnica>();
                        while (reader.Read())
                        {
                            item.tecnicasTecnologo.Add(new DetalleHCAdquision_Tecnica
                            {
                                Fecha = (reader["fecha"].ToString()),
                                Estudio = reader["StudyDescription"].ToString(),
                                Serie = reader["SeriesDescription"].ToString(),
                                Cant = Convert.ToInt32(reader["numberOfSeriesRelatedInstances"].ToString()),
                            });
                        }
                        //Enfermera
                        reader.Close();
                        sqlquery = @"select 
u.ShortName,
e.nombreestudio,
a.Nombre,
dc.cantidad,
dc.lote,
dc.isaplicado,
a.CorrelativoPref
from Contraste c 
inner join Detalle_Contraste dc on c.idcontraste=dc.id_contraste 
inner join AL_Stock_Item ai on dc.Al_InsumId=ai.AL_Stock_ItemID 
inner join AL_Insum a on ai.AL_InsumId= a.Al_InsumId
inner join ESTUDIO e on c.insumo_medico=e.codigoestudio
inner join USUARIO u on c.usuario=u.codigousuario
where c.numeroexamen='{0}'";
                        command_cita.CommandText = string.Format(sqlquery, examen.ToString());
                        command_cita.Prepare();
                        reader = command_cita.ExecuteReader();
                        item.insumoEnfermera = new List<DetalleHCAdquision_Insumo>();
                        while (reader.Read())
                        {
                            item.enfermeraName = (reader["shortName"].ToString());
                            item.contraste = reader["nombreestudio"].ToString();

                            item.insumoEnfermera.Add(new DetalleHCAdquision_Insumo
                            {
                                Insumo = (reader["Nombre"].ToString()),
                                Cant = (reader["cantidad"].ToString()),
                                Frasco = string.Format(reader["CorrelativoPref"].ToString(), reader["lote"].ToString()),
                                Aplicado = Convert.ToBoolean(reader["isaplicado"].ToString()),
                            });

                        }
                        //Anestesiologo
                        reader.Close();
                        sqlquery = "select u.ShortName,isnull(tipo_sedacion,'')tipo,isnull(case when s.motivo_sedacion='Otros'then s.motivo_otros else s.motivo_sedacion end,'') motivo from Sedacion s inner join USUARIO u on s.usuario=u.codigousuario where numeroexamen='{0}'";
                        command_cita.CommandText = string.Format(sqlquery, examen.ToString());
                        command_cita.Prepare();
                        reader = command_cita.ExecuteReader();
                        while (reader.Read())
                        {
                            item.anestesiologoName = (reader["shortname"].ToString());
                            item.tipoSedacion = (reader["tipo"].ToString());
                            item.motivoSedacion = reader["motivo"].ToString();
                        }
                        //Postprocesador
                        reader.Close();
                        sqlquery = "select convert(varchar(20),p.fecha,113)fecha,u.ShortName,p.descripcion,p.cantidadplaca,e.nombreequipo from POSTPROCESO p inner join Equipo_PostProceso e on p.codigoequipo=e.codigoequipo inner join USUARIO u on p.codigousuario=u.codigousuario where p.tipo='D' and p.estado='R' and p.numeroestudio='{0}'";
                        command_cita.CommandText = string.Format(sqlquery, examen.ToString());
                        command_cita.Prepare();
                        reader = command_cita.ExecuteReader();
                        item.tecnicasPostproceso = new List<DetalleHCAdquision_PostProceso>();
                        while (reader.Read())
                        {
                            item.tecnicasPostproceso.Add(new DetalleHCAdquision_PostProceso
                            {
                                Fecha = Convert.ToDateTime(reader["fecha"].ToString()).ToShortDateString(),
                                Tecnologo = (reader["shortname"].ToString()),
                                Equipo = reader["nombreequipo"].ToString(),
                                Tecnica = (reader["descripcion"].ToString()),
                                Placa = (reader["cantidadplaca"].ToString()),
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxTemporal.Show(ex.Message, "Error", 10, true);
                    }
                    finally
                    {
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
            return item;
        }
    }
}
