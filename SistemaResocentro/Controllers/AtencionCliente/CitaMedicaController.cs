using SistemaResocentro.Member;
using SistemaResocentro.Models;
using SistemaResocentro.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaResocentro.Controllers.AtencionCliente
{
    [Authorize]
    public class CitaMedicaController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: CitaMedica
        public ActionResult BusquedaGeneral(string filtro)
        {

            BusquedaGeneralViewModel item = new BusquedaGeneralViewModel();
            item.lstPacientes = new List<PACIENTE>();
            item.lstCita = new List<ListaCitas_BG>();
            item.lstAdmision = new List<ListaAdmision_BG>();
            item.lstcarta = new List<ListaCartas_BG>();
            item.lstAdquisicion = new List<ListaAdquisicion_BG>();
            item.lstdocumento = new List<ListaDocumento_BG>();
            if (filtro != null)
            {
                filtro = filtro.Trim();
                var _pacientes = getPaciente(0, filtro, "");
                item.lstPacientes = _pacientes.lstPacientes;
                if (item.lstPacientes.Count == 1)
                {
                    var codigpaciente = item.lstPacientes.First().codigopaciente;
                    var _item = getInfoPaciente(codigpaciente);
                    item.lstCita = _item.lstCita;
                    item.lstAdmision = _item.lstAdmision;
                    item.lstcarta = _item.lstcarta;
                    item.lstAdquisicion = _item.lstAdquisicion;
                    item.lstdocumento = _item.lstdocumento;
                }


            }

            if (item.lstPacientes.Count == 0)
                if (filtro != null)
                    ViewBag.Error = "No se encontro ninguna coincidencia";

            return View(item);
        }
        public BusquedaGeneralViewModel getInfoPaciente(int codigpaciente)
        {
            #region sql
            string sqlCita = @"
select c.citavip,ec.estadoestudio E,ec.numerocita cita,sedacion S,convert(varchar(20),c.fechareserva,103)+' '+convert(varchar(20),ec.horacita,108) Fecha,
es.nombreestudio,case when ec.codigomoneda='1' then 'S/.'+convert(varchar(200),ec.precioestudio) else '$.'+ convert(varchar(200),ec.precioestudio) end precio,convert(varchar(200),ISNULL(cg.cobertura,'0.0'))+'%' cob,eq.nombreequipo equipo,REPLACE(REPLACE(REPLACE(c.observacion,CHAR(10),' ') ,CHAR(13),' ') ,'  ',' ') observacion ,us.ShortName,ISNULL(cg.codigocartagarantia,'')carta,ch.razonsocial
 from EXAMENXCITA ec
inner join CITA c on ec.numerocita=c.numerocita
inner join ESTUDIO es on ec.codigoestudio=es.codigoestudio
inner join EQUIPO eq on ec.codigoequipo=eq.codigoequipo
inner join USUARIO us on c.codigousuario=us.codigousuario
inner join CLINICAHOSPITAL ch on c.codigoclinica=ch.codigoclinica
left join CARTAGARANTIA cg on c.codigocartagarantia=cg.codigocartagarantia
where ec.codigopaciente='{0}' ORDER BY ec.numerocita
";
            string sqlAdmin = @"
select ea.estadoestudio AS E,case when ci.estadocargo='I' then 'Informes' when ci.estadocargo='E' then 'Counter'  when ci.estadocargo='O'  then 'Paciente' else ''end 'ubicacion',a.fechayhora fecha,ea.numeroatencion atencion,ea.codigo examen,es.nombreestudio estudio,cs.descripcion aseguradora,ea.prioridad,me.apellidos+' '+me.nombres medico,us.Shortname,ea.turnomedico,ea.numerocita from EXAMENXATENCION ea 
inner join ATENCION a on ea.numeroatencion=a.numeroatencion
inner join ESTUDIO es on ea.codigoestudio=es.codigoestudio
inner join COMPANIASEGURO cs on ea.codigocompaniaseguro=cs.codigocompaniaseguro
inner join MEDICOEXTERNO me on a.cmp=me.cmp
inner join USUARIO us on a.codigousuario=us.codigousuario
left join CARGOINFORMES ci on ea.codigo=ci.numeroestudio
where a.codigopaciente='{0}' order by ea.codigo
";
            string sqlCarta = @"select  isnull(c.numerocita,'') cita,cg.estadocarta,cg.fechatramite fecha,cg.codigocartagarantia id,ISNULL(cg.codigocartagarantia2,'') id2,cs.descripcion,convert(varchar(100),cg.cobertura)+'%' cobertura,cg.codigodocadjunto adjunto,cg.codigopaciente paciente  from CARTAGARANTIA cg 
inner join COMPANIASEGURO cs on cg.codigocompaniaseguro=cs.codigocompaniaseguro
left join CITA c on cg.codigocartagarantia=c.codigocartagarantia
where cg.codigopaciente='{0}' order by cg.fechatramite";
            string sqldocumento = @" select d.estado,u.nombre empresa,d.fechaemitio emision,d.fechadepago pago,CASE WHEN d.tipodocumento='01' THEN 'Factura' WHEN d.tipodocumento='03' THEN 'Boleta'  WHEN d.tipodocumento='07' THEN 'Nota Credito' else '' END AS documento,d.numerodocumento numero,case when d.tipomoneda='1' then 'S/.'+convert(varchar(200),d.total) else '$.'+ convert(varchar(200),d.total) end total ,ISNULL(lc.numerocita,'')cita from DOCUMENTO d  
 inner join UNIDADNEGOCIO u on d.codigounidad=u.codigounidad
 left join LIBROCAJA lc on lc.numerodocumento=SUBSTRING(d.numerodocumento,5,7) AND lc.codigopaciente=d.codigopaciente
 where d.codigopaciente='{0}'
 and  lc.codigopaciente='{0}'
  ORDER BY lc.fechacobranza;";

            string sqladquisicion = @" select ea.codigo,es.nombreestudio,ht.placauso, ISNULL(cv.tiposedacion,'') AS Sedacion,ISNULL(cv.tipocontraste,'') AS Contraste,eq.ShortDesc equipo,CONVERT(CHAR(10),ht.fechaestudio,103)+' '+CONVERT(CHAR(5),ht.horaestudio,108) fecha, us.ShortName tecnologo,ea.numerocita cita from HONORARIOTECNOLOGO ht
 inner join EXAMENXATENCION ea on ht.codigohonorariotecnologo=ea.codigo
 inner join equipo eq on ht.codigoequipo=eq.codigoequipo
 inner join ESTUDIO es on ea.codigoestudio=es.codigoestudio
 left join CONTROLDEVALIDACION cv on ea.codigo=cv.numeroestudio
 inner join USUARIO us on ht.tecnologoturno=us.siglas
 where ea.codigopaciente='{0}' order by ht.codigohonorariotecnologo";
            #endregion
            BusquedaGeneralViewModel item = new BusquedaGeneralViewModel();
            item.lstCita = new List<ListaCitas_BG>();
            item.lstAdmision = new List<ListaAdmision_BG>();
            item.lstcarta = new List<ListaCartas_BG>();
            item.lstAdquisicion = new List<ListaAdquisicion_BG>();
            item.lstdocumento = new List<ListaDocumento_BG>();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_cita = new SqlCommand(string.Format(sqlCita, codigpaciente.ToString()), connection);
                connection.Open();
                SqlDataReader reader = command_cita.ExecuteReader();
                try
                {
                    //CITA
                    while (reader.Read())
                    {
                        item.lstCita.Add(new ListaCitas_BG
                        {
                            estado = (reader["E"]).ToString(),
                            vip = Convert.ToBoolean(reader["citavip"]),
                            sedacion = Convert.ToBoolean(reader["s"]),
                            num_cita = Convert.ToInt32((reader["cita"]).ToString()),
                            fecha = Convert.ToDateTime((reader["Fecha"]).ToString()),
                            estudio = (reader["nombreestudio"]).ToString(),
                            precio = (reader["precio"]).ToString(),
                            cobertura = (reader["cob"]).ToString(),
                            equipo = (reader["equipo"]).ToString(),
                            obs = (reader["observacion"]).ToString(),
                            usuario = (reader["ShortName"]).ToString(),
                            clinica = (reader["razonsocial"]).ToString(),
                            carta = (reader["carta"]).ToString(),
                        });
                    }
                    //ADMISION
                    SqlCommand command_admi = new SqlCommand(string.Format(sqlAdmin, codigpaciente.ToString()), connection);
                    reader = command_admi.ExecuteReader();
                    while (reader.Read())
                    {
                        item.lstAdmision.Add(new ListaAdmision_BG
                        {
                            estado = (reader["E"]).ToString(),
                            ubicacion = (reader["ubicacion"]).ToString(),
                            fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
                            atencion = Convert.ToInt32((reader["atencion"]).ToString()),
                            examen = Convert.ToInt32((reader["examen"]).ToString()),
                            estudio = (reader["estudio"]).ToString(),
                            aseguradora = (reader["aseguradora"]).ToString(),
                            prioridad = (reader["prioridad"]).ToString(),
                            medico = (reader["medico"]).ToString(),
                            turno = (reader["turnomedico"]).ToString(),
                            usuario = (reader["ShortName"]).ToString(),
                            cita = Convert.ToInt32((reader["numerocita"]).ToString()),
                        });
                    }
                    //CARTA
                    SqlCommand command_carta = new SqlCommand(string.Format(sqlCarta, codigpaciente.ToString()), connection);
                    reader = command_carta.ExecuteReader();
                    while (reader.Read())
                    {
                        item.lstcarta.Add(new ListaCartas_BG
                        {
                            estado = (reader["estadocarta"]).ToString(),
                            fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
                            id = (reader["id"]).ToString(),
                            id2 = (reader["id2"]).ToString(),
                            aseguradora = (reader["descripcion"]).ToString(),
                            cobertura = (reader["cobertura"]).ToString(),
                            cita = Convert.ToInt32((reader["cita"]).ToString()),
                            isadjunto = (reader["adjunto"]).ToString(),
                            idpaciente = (reader["paciente"]).ToString(),
                        });
                    }
                    //ADQUISICION
                    SqlCommand command_adquisicion = new SqlCommand(string.Format(sqladquisicion, codigpaciente.ToString()), connection);
                    reader = command_adquisicion.ExecuteReader();
                    while (reader.Read())
                    {
                        item.lstAdquisicion.Add(new ListaAdquisicion_BG
                        {
                            examen = Convert.ToInt32((reader["codigo"]).ToString()),
                            estudio = (reader["nombreestudio"]).ToString(),
                            placas = Convert.ToInt32((reader["placauso"]).ToString()),
                            sedacion = (reader["Sedacion"]).ToString(),
                            contraste = (reader["Contraste"]).ToString(),
                            fecha = Convert.ToDateTime((reader["fecha"]).ToString()),
                            tecnologo = (reader["tecnologo"]).ToString(),
                            equipo = (reader["equipo"]).ToString(),
                            cita = (reader["cita"]).ToString(),
                        });
                    }
                    //DOCUMENTO
                    SqlCommand command_documento = new SqlCommand(string.Format(sqldocumento, codigpaciente.ToString()), connection);
                    reader = command_documento.ExecuteReader();
                    while (reader.Read())
                    {
                        item.lstdocumento.Add(new ListaDocumento_BG
                        {
                            estado = (reader["estado"]).ToString(),
                            empresa = (reader["empresa"]).ToString(),
                            emision = Convert.ToDateTime((reader["emision"]).ToString()),
                            pago = Convert.ToDateTime((reader["pago"]).ToString()),
                            documento = (reader["documento"]).ToString(),
                            numero = (reader["numero"]).ToString(),
                            total = (reader["total"]).ToString(),
                            cita = (reader["cita"]).ToString(),
                        });
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return item;
        }
        public BusquedaGeneralViewModel getPaciente(int t, string p1, string p2)
        {
            string queryString = @"
select codigopaciente,ISNULL(IsProtocolo,0) IsProtocolo,apellidos,nombres,dni,telefono+'   '+celular telefono from PACIENTE 
where apellidos like '%{0}%' 
or nombres like '%{0}%' 
or dni like '%{0}%'
";
            string queryApelliNombre = @"
select codigopaciente,ISNULL(IsProtocolo,0) IsProtocolo,apellidos,nombres,dni,telefono+'   '+celular telefono from PACIENTE 
where apellidos like '%{0}%' 
and  nombres like '%{1}%' 
";
            BusquedaGeneralViewModel item = new BusquedaGeneralViewModel();
            item.lstPacientes = new List<PACIENTE>();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command;
                if (t == 0)

                    command = new SqlCommand(string.Format(queryString, p1), connection);
                else
                    command = new SqlCommand(string.Format(queryApelliNombre, p1, p2), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        item.lstPacientes.Add(new PACIENTE
                        {
                            codigopaciente = Convert.ToInt32((reader["codigopaciente"]).ToString()),
                            IsProtocolo = Convert.ToBoolean(reader["IsProtocolo"]),
                            apellidos = (reader["apellidos"]).ToString(),
                            nombres = (reader["nombres"]).ToString(),
                            dni = (reader["dni"]).ToString(),
                            telefono = (reader["telefono"]).ToString()
                        });
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return item;
        }
        public ActionResult getDataPaciente(int codigopaciente)
        {
            var item = getInfoPaciente(codigopaciente);
            var resultado = new
            {
                cita = (from x in item.lstCita select new[] { x.estado, x.vip.ToString(), x.sedacion.ToString(), x.num_cita.ToString(), x.estudio, x.fecha.ToString("dd/MM/yyyy hh:mm tt"), x.precio,x.clinica , x.cobertura, x.equipo, x.obs, x.usuario}),
                admin = (from x in item.lstAdmision select new[] { x.estado, x.ubicacion, x.fecha.ToString("dd/MM/yyyy hh:mm tt"), x.atencion.ToString(), x.examen.ToString(), x.estudio, x.aseguradora, x.prioridad, x.medico, x.turno, x.usuario, x.cita.ToString() }),
                carta = (from x in item.lstcarta select new[] { x.estado, x.fecha.ToString("dd/MM/yyyy hh:mm tt"), x.id, x.id2, x.aseguradora, x.cobertura, x.cita.ToString(),x.isadjunto.ToString(),x.idpaciente.ToString() }),
                pagos = (from x in item.lstdocumento select new[] { x.estado, x.empresa, x.emision.ToString("dd/MM/yyyy hh:mm tt"), x.pago.ToString("dd/MM/yyyy hh:mm tt"), x.documento, x.numero, x.total, x.cita }),
                adquisicion = (from x in item.lstAdquisicion select new[] { x.examen.ToString(), x.estudio, x.placas.ToString(), x.sedacion, x.contraste, x.equipo, x.fecha.ToString("dd/MM/yyyy hh:mm tt"), x.tecnologo, x.cita }),
            };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getlistaPaciente(int t, string p1, string p2)
        {
            var item = getPaciente(t, p1, p2);
            var resultado = (from x in item.lstPacientes select new[] { x.codigopaciente.ToString(), x.IsProtocolo.ToString(), x.apellidos, x.nombres, x.dni, x.telefono + "  " + x.telefono });
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListaTrabajo()
        {
            ViewBag.EstadoCarta = new SelectList(new Variable().getEstadoCarta(), "nombre", "nombre", "APROBADA");
            return View();
        }
        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {
            //JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var listacarta = getCartasxCitar();
            IEnumerable<CartasxCitarViewModel> listaCartasFilter;
            param.sSearch = (Request["search[value]"]);
            param.iDisplayStart = Convert.ToInt32(Request["start"]);
            param.iDisplayLength = Convert.ToInt32(Request["length"]);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                listaCartasFilter = listacarta
                     .Where(c => c.paciente.ToLower().Contains(param.sSearch.ToLower())
                         || c.estado.ToLower().Contains(param.sSearch.ToLower())
                     );
            }
            else
            {
                listaCartasFilter = listacarta;
            }

            var isPacienteSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
            var isInicioSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
            var isCoberturaSortable = Convert.ToBoolean(Request["columns[2][orderable]"]);
            var isTelefonoSortable = Convert.ToBoolean(Request["columns[3][orderable]"]);
            var isLlamadasSortable = Convert.ToBoolean(Request["columns[4][orderable]"]);
            var isAseguradoraSortable = Convert.ToBoolean(Request["columns[5][orderable]"]);
            var isestadoSortable = Convert.ToBoolean(Request["columns[6][orderable]"]);

            var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            Func<CartasxCitarViewModel, string> orderingFunction = (
                c => sortColumnIndex == 0 && isPacienteSortable ? c.paciente.ToString() :
                    sortColumnIndex == 1 && isInicioSortable ? c.inicio.ToString() :
                    sortColumnIndex == 2 && isCoberturaSortable ? c.cobertura.ToString() :
                    sortColumnIndex == 3 && isTelefonoSortable ? c.telefono.ToString() :
                    sortColumnIndex == 4 && isLlamadasSortable ? c.llamadas.ToString() :
                    sortColumnIndex == 5 && isAseguradoraSortable ? c.aseguradora.ToString() :
                    sortColumnIndex == 6 && isestadoSortable ? c.estado.ToString() :
                    "");

            var sortDirection = Request["order[0][dir]"]; // asc or desc
            if (sortDirection == "asc")
                listaCartasFilter = listaCartasFilter.OrderBy(orderingFunction);
            else
                listaCartasFilter = listaCartasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaCartasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = (from c in displayedCompanies
                          select new[]{ 
                   (c.isrevisada==true?"<i class='fa fa-check-circle fa-2x' style='color:#007d3d'></i>&nbsp;&nbsp;":"")+c.paciente,
                   c.inicio.ToString("dd/MM/yy"),
                   c.cobertura +(c.adjunto!=""?"&nbsp;&nbsp;&nbsp;<a class='btn btn-default btn-xs' onclick='downloadAdjunto(\""+c.adjunto+"\")' ><i class='fa  fa-download'></i></a>":""),
                   (c.telefono!="."&&c.telefono!=""&&c.telefono!="0"?"<a class='btn btn-info btn-xs' onclick='registrarLlamada(\""+c.codigocarta+"\",\""+c.idpaciente.ToString()+"\",\""+c.telefono.ToString()+"\")' ><i class='fa fa-phone'></i>&nbsp;&nbsp;"+c.telefono+"</a>":"")+"&nbsp;&nbsp;&nbsp;"+(
                   c.celular!="."&&c.celular!=""&&c.celular!="0"?"<a class='btn btn-info btn-xs' onclick='registrarLlamada(\""+c.codigocarta+"\",\""+c.idpaciente.ToString()+"\",\""+c.celular.ToString()+"\")' ><i class='fa fa-phone'></i>&nbsp;&nbsp;"+c.celular+"</a>":""),
                   c.llamadas.ToString(),
                   c.aseguradora.ToString(),
                   c.estado.ToString()
                   
               });

            return Json(new
            {
                draw = Request["draw"],
                recordsTotal = listacarta.Count(),
                recordsFiltered = listaCartasFilter.Count(),
                data = result
            },
                        JsonRequestBehavior.AllowGet);


        }
        public ActionResult GetAdjuntosxCartaGarantia(string adjunto)
        {
            string fileName;
            var _a = (from dca in db.DOCESCANEADO
                      where dca.codigodocadjunto == adjunto
                      select dca).SingleOrDefault();
            if (_a != null)
            {
                byte[] archivo = _a.cuerpoarchivo;
                fileName = _a.nombrearchivo;

                return File(archivo, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return View();
        }
        private IList<CartasxCitarViewModel> getCartasxCitar()
        {
            List<CartasxCitarViewModel> listaCarta = new List<CartasxCitarViewModel>();
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
cg.cobertura Cob,
cs.descripcion Aseguradora,
cg.estadocarta estadoCarta,
case when cg.codigodocadjunto is null then 
''
else
cg.codigodocadjunto
end Adjunto
FROM CARTAGARANTIA cg 
INNER JOIN PACIENTE p ON cg.codigopaciente=p.codigopaciente
INNER JOIN  COMPANIASEGURO cs ON cg.codigocompaniaseguro=cs.codigocompaniaseguro
WHERE cg.isCaducada is null
and cg.estadocarta !='CITADA'
and (select top(1)ce.codigoestudio from ESTUDIO_CARTAGAR ce where ce.codigopaciente=cg.codigopaciente and ce.codigocartagarantia=cg.codigocartagarantia) like '1%'
";
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        listaCarta.Add(new CartasxCitarViewModel
                        {
                            codigocarta = (reader["codigo"]).ToString(),
                            idpaciente = Convert.ToInt32(reader["idpaciente"]),
                            paciente = (reader["Paciente"]).ToString(),
                            inicio = Convert.ToDateTime(reader["Inicio"]),
                            aprobacion = Convert.ToDateTime(reader["Aprobacion"]),
                            telefono = (reader["Telefono"]).ToString(),
                            celular = (reader["Celular"]).ToString(),
                            llamadas = Convert.ToInt32(reader["Llamadas"]),
                            cobertura = (reader["Cob"]).ToString() + "%",
                            aseguradora = (reader["Aseguradora"]).ToString(),
                            estado = (reader["estadoCarta"]).ToString(),
                            adjunto = (reader["Adjunto"]).ToString(),
                            isrevisada = Convert.ToBoolean(reader["Revisada"]),
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

        public ActionResult RealizarLLamadaCita(string idcarta, int idpaciente, string telefono)
        {
            var carta = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == idcarta && x.codigopaciente == idpaciente);
            var _auditCarta = db.AUDITORIA_CARTAS_GARANTIA
                .Where(x => x.numerodecarta == idcarta && x.codigopaciente == idpaciente)
                .Select(x => new DetalleRealizarLlamada
                {
                    fecha = x.fecha.Value,
                    usuario = x.USUARIO.ShortName,
                    mensaje = x.mensaje,
                    telefono = x.tel_registro
                }).ToList();
            RealizarLlamadaViewModel registroLlamada = new RealizarLlamadaViewModel();
            if (carta != null)
            {
                registroLlamada.paciente = carta.PACIENTE;
                registroLlamada.carta = carta;
                registroLlamada.llamadas = _auditCarta;
                registroLlamada.telefono = telefono;
            }
            return View(registroLlamada);
        }
        public ActionResult GuardarLlamadaCarta(string cadena)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            AUDITORIA_CARTAS_GARANTIA item = new AUDITORIA_CARTAS_GARANTIA();
            item = Newtonsoft.Json.JsonConvert.DeserializeObject<AUDITORIA_CARTAS_GARANTIA>(cadena);
            item.codigousuario = user.ProviderUserKey.ToString();
            item.fecha = DateTime.Now;
            db.AUDITORIA_CARTAS_GARANTIA.Add(item);
            db.SaveChanges();
            return Json(true);
        }

        public ActionResult CitarCarta(string idcarta, int idpaciente)
        {
            var _carta = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == idcarta && x.codigopaciente == idpaciente);

            CitarCartaViewModel item = new CitarCartaViewModel();
            if (_carta != null)
            {
                item.carta = _carta;
                item.cita = new CITA();
                var _paciente = _carta.PACIENTE;
                var _clinica = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == _carta.codigoclinica);
                var _medico = db.MEDICOEXTERNO.SingleOrDefault(x => x.cmp == _carta.cmp);
                var _aseguradora = db.COMPANIASEGURO.SingleOrDefault(x => x.codigocompaniaseguro == _carta.codigocompaniaseguro);
                var _estudios = db.ESTUDIO_CARTAGAR.Where(x => x.codigocartagarantia == idcarta && x.codigopaciente == idpaciente).ToList();
                item.cita.codigopaciente = _paciente.codigopaciente;
                item.cita.PACIENTE = _paciente;
                if (_clinica != null)
                {
                    item.cita.codigoclinica = _clinica.codigoclinica;
                    item.cita.codigozona = _clinica.codigozona;
                    item.cita.CLINICAHOSPITAL = _clinica;
                }
                else
                {
                    ViewBag.Clinica = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).AsParallel().ToList(), "codigoclinica", "razonsocial");
                }
                item.cita.citavip = false;
                item.cita.sedacion = false;
                item.cita.claustrofobico = false;
                item.cita.ampliatorio = false;
                item.cita.observacion = _carta.seguimiento;
                item.medico = _medico;
                item.cita.codigocompaniaseguro = _aseguradora.codigocompaniaseguro;
                item.cita.COMPANIASEGURO = _aseguradora;
                item.cita.codigomodalidad = int.Parse((_estudios.FirstOrDefault()).codigoestudio.Substring(3, 2).ToString());
                item.cita.codigounidad = int.Parse(_estudios.FirstOrDefault().codigoestudio.Substring(0, 1).ToString());
                var _sucursal = int.Parse(_estudios.FirstOrDefault().codigoestudio.Substring(1, 2).ToString());
                ViewBag.UnidadNegocio = new SelectList(db.SUCURSAL.OrderBy(x => x.codigounidad).Where(x => x.codigounidad == 1).Select(x => new { codigounidad = (x.codigounidad * 100) + x.codigosucursal, nombre = x.UNIDADNEGOCIO.nombre + " - " + x.ShortDesc }).ToList(), "codigounidad", "nombre");
                ViewBag.Categoria = new SelectList(new Variable().getCategoriaCita(), "nombre", "nombre");
                item.cita.categoria = "AMBULATORIO";
                item.idsede = ((item.cita.codigounidad * 100) + _sucursal);
                item.estudios = new List<EXAMENXCITA>();

                foreach (var estu in _estudios)
                {
                    var _precio = db.ESTUDIO_COMPANIA.SingleOrDefault(x => x.codigocompaniaseguro == estu.codigocompaniaseguro && x.codigoestudio == estu.codigoestudio);

                    EXAMENXCITA est = new EXAMENXCITA();
                    est.codigoestudio = estu.codigoestudio;
                    est.ESTUDIO = _precio.ESTUDIO;
                    est.codigoclase = estu.codigoclase;
                    est.codigomoneda = _precio.codigomoneda;
                    est.precioestudio = _precio.preciobruto;
                    item.estudios.Add(est);
                }

            }
            return View(item);
        }

        public ActionResult VerificarTurnoCita(DateTime fecha, int idequipo)
        {
            var isLibre = db.HORARIO.SingleOrDefault(x => x.fecha.Year == fecha.Year && x.fecha.Month == fecha.Month && x.fecha.Day == fecha.Day && x.hora.Hour == fecha.Hour && x.hora.Minute == fecha.Minute && x.codigoequipo == idequipo);
            if (isLibre != null)
                return Json(isLibre.numerocita == null, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegistrarCitaCarta(string p1, string p2)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            CITA cita = new CITA();
            cita = Newtonsoft.Json.JsonConvert.DeserializeObject<CITA>(p1);
            cita.EXAMENXCITA = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EXAMENXCITA>>(p2);
            cita.numerocita = db.CITA.Max(x => x.numerocita) + 1;
            if (cita.codigozona == null)
            {
                var _clinica = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == cita.codigoclinica);
                cita.codigozona = _clinica.codigozona;
            }
            cita.fechacreacion = DateTime.Now;
            cita.nombrepc = "WEB";
            cita.nombrepc = "AMBULATORIO";
            cita.codigousuario = user.ProviderUserKey.ToString();
            foreach (var item in cita.EXAMENXCITA)
            {
                if (int.Parse(item.codigoestudio.Substring(6, 1)) < 9)
                {
                    var _horario = db.HORARIO.SingleOrDefault(x => x.codigohorario == item.numerocita);
                    _horario.numerocita = cita.numerocita;
                    _horario.codigopaciente = cita.codigopaciente;
                    _horario.codigoestudio = item.codigoestudio;
                }
                item.numerocita = cita.numerocita;
                db.EXAMENXCITA.Add(item);
            }
            db.CITA.Add(cita);
            //actualizando carta
            var _carta = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == cita.codigocartagarantia && x.codigopaciente == cita.codigopaciente);
            _carta.estadocarta = "CITADA";

            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaTurnos(string codigoestudio, DateTime fecha, string idsede)
        {
            int unidad = int.Parse(idsede.Substring(0, 1).ToString()),
                sucursal = int.Parse(idsede.Substring(1, 2).ToString()),
                modalidad = int.Parse(codigoestudio.Substring(3, 2).ToString()),
                dia = fecha.Day,
                mes = fecha.Month,
                ano = fecha.Year;
            ViewBag.Fecha = fecha.ToShortDateString();
            var lista = db.listarturnoparacita(unidad, sucursal, modalidad, dia, mes, ano);
            return View(lista);
        }
    }
}