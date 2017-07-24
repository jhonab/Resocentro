using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Reporte;
using Encuesta.ViewModels.Tecnologo;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace Encuesta.Controllers.Reporte
{
    [Authorize]
    public class ReporteController : Controller
    {

        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        public ActionResult ReporteTecnologos(DateTime i, DateTime f, string tecno, string equipo, string rev)
        {
            //CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            ReporteViewModel model = new ReporteViewModel();
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);
            //string _user = user.ProviderUserKey.ToString();
            model.isRevisado = false;
            if (fec_ini.ToShortDateString() == fec_fin.ToShortDateString())
            {
                model.rangoFiltro = fec_ini.ToShortDateString();
                if (fec_ini == new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1))
                    model.isRevisado = true;
            }
            else
                model.rangoFiltro = fec_ini.ToShortDateString() + " al " + fec_fin.ToShortDateString();

            List<Data_reporte> lista = new List<Data_reporte>();
            ReporteViewModel data = new ReporteViewModel();
            string sqlPost = @"select ut.siglas,ht.fechaestudio,ea.codigo,pa.apellidos + ' ' + pa.nombres as paciente,es.nombreestudio,eq.nombreequipo,ht.placauso,ea.UrlXero,
case when ea.UrlXero is null then 0 else 1 end as imagenes,ht.procesar,ht.sedacion,ht.contraste,ht.contraste_continuo,ue.ShortName as encuestador,us.ShortName as supervisor, en.fec_ini_tecno,en.fec_fin_tecno,ht.isRevisado, case SUBSTRING(ea.codigoestudio,5,1) when 1 then 'MR' when 2 then 'CT' else '' end as modalidad,case when se.p2 is null then 0 else se.p2 end p2,case when se.p2_1 is null then '' else p2_1 end p2_1 ,case when se.p3 is null then 0 else p3 end p3,case when se.p3_1 is null then '' else p3_1 end p3_1 from Encuesta en 
inner join EXAMENXATENCION ea on en.numeroexamen = ea.codigo 
inner join ESTUDIO es on ea.codigoestudio = es.codigoestudio
inner join  PACIENTE pa on ea.codigopaciente = pa.codigopaciente 
inner join  EQUIPO eq on ea.equipoAsignado = eq.codigoequipo
left join USUARIO us on en.usu_reg_super = us.codigousuario
left join USUARIO ue on en.usu_reg_encu = ue.codigousuario
left join USUARIO ut on en.usu_reg_tecno = ut.codigousuario
inner join HONORARIOTECNOLOGO ht on en.numeroexamen = ht.codigohonorariotecnologo
left join SupervisarEncuesta se on en.numeroexamen = se.numeroexamen
where (en.usu_reg_tecno like '" + tecno + "' and eq.codigoequipo like '" + equipo + "' and ht.isRevisado like '" + rev + "' ) and en.fec_ini_tecno is not null and convert(date,en.fec_ini_encu) between '" + fec_ini.ToShortDateString() + "' and '" + fec_fin.ToShortDateString() + "' and ea.estadoestudio != 'X' and en.estado >=3 order by en.usu_reg_tecno";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(sqlPost, connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new Data_reporte();

                        item.tecno = (reader["siglas"]).ToString();
                        item.fecha = Convert.ToDateTime(reader["fechaestudio"]);
                        item.examen = Convert.ToInt32(reader["codigo"]);
                        item.paciente = (reader["paciente"]).ToString();
                        item.estudio = (reader["nombreestudio"]).ToString();
                        item.equipo = (reader["nombreequipo"]).ToString();
                        item.placas = Convert.ToInt32(reader["placauso"]);
                        item.imagenes = Convert.ToBoolean(reader["imagenes"]);
                        item.postproceso = Convert.ToBoolean(reader["procesar"]);
                        item.sedacion = Convert.ToBoolean(reader["sedacion"]);
                        item.contraste = Convert.ToBoolean(reader["contraste"]);
                        item.continuo = Convert.ToBoolean(reader["contraste_continuo"]);
                        item.encuestador = (reader["encuestador"]).ToString();
                        item.supervisor = (reader["supervisor"]).ToString();
                        item.tec_ini = Convert.ToDateTime(reader["fec_ini_tecno"]);
                        item.tec_fin = Convert.ToDateTime(reader["fec_fin_tecno"]);
                        item.isRevisado = Convert.ToBoolean(reader["isRevisado"]);
                        item.modalidad = (reader["modalidad"]).ToString();
                        item.isError_p2 = Convert.ToBoolean(reader["p2"]);
                        item.Error_p2 = (reader["p2_1"]).ToString();
                        item.isError_p3 = Convert.ToBoolean(reader["p3"]);
                        item.Error_p3 = (reader["p3_1"]).ToString();


                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }


                data.tecnologo = lista;


            }

            return View(data);
        }

        //public ActionResult ReporteTecnologos2(DateTime i, DateTime f, string tecno)
        //{
        //    //CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
        //    ReporteViewModel model = new ReporteViewModel();
        //    DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
        //    DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);
        //    //string _user = user.ProviderUserKey.ToString();
        //    model.isRevisado = false;
        //    if (fec_ini.ToShortDateString() == fec_fin.ToShortDateString())
        //    {
        //        model.rangoFiltro = fec_ini.ToShortDateString();
        //        if (fec_ini == new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1))
        //            model.isRevisado = true;
        //    }
        //    else
        //        model.rangoFiltro = fec_ini.ToShortDateString() + " al " + fec_fin.ToShortDateString();
        //    #region Tecnologo
        //    //if (User.IsInRole("3"))//tecnologo
        //    //{
        //    model.tecnologo = (from en in db.Encuesta
        //                       join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
        //                       join pa in db.PACIENTE on ea.codigopaciente equals pa.codigopaciente
        //                       join eq in db.EQUIPO on ea.equipoAsignado equals eq.codigoequipo
        //                       join us in db.USUARIO on en.usu_reg_super equals us.codigousuario into us_join
        //                       from us in us_join.DefaultIfEmpty()
        //                       join ue in db.USUARIO on en.usu_reg_encu equals ue.codigousuario into ue_join
        //                       from ue in ue_join.DefaultIfEmpty()
        //                       join ut in db.USUARIO on en.usu_reg_tecno equals ut.codigousuario into ut_join
        //                       from ut in ut_join.DefaultIfEmpty()
        //                       join ht in db.HONORARIOTECNOLOGO on en.numeroexamen equals ht.codigohonorariotecnologo
        //                       join se in db.SupervisarEncuesta on en.numeroexamen equals se.numeroexamen into se_join
        //                       from se in se_join.DefaultIfEmpty()
        //                       where
        //                           //SqlMethods.Like(en.usu_reg_tecno,tecno) &&
        //                         en.usu_reg_tecno == tecno &&
        //                         en.fec_ini_tecno != null &&
        //                         (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
        //                         ea.estadoestudio != "X" &&
        //                         en.estado >= 3
        //                       orderby
        //                         ea.codigo
        //                       select new Data_reporte
        //                       {
        //                           tecno = ut.siglas,
        //                           fecha = ht.fechaestudio,
        //                           examen = ea.codigo,
        //                           paciente = pa.apellidos + " " + pa.nombres,
        //                           estudio = ea.ESTUDIO.nombreestudio,
        //                           equipo = eq.nombreequipo,
        //                           placas = ht.placauso,
        //                           imagenes = ea.UrlXero != null ? true : false,
        //                           postproceso = ht.procesar,
        //                           sedacion = ht.sedacion.Value,
        //                           contraste = ht.contraste.Value,
        //                           continuo = ht.contraste_continuo.Value,
        //                           encuestador = ue.ShortName,
        //                           supervisor = us.ShortName,
        //                           tec_ini = en.fec_ini_tecno.Value,
        //                           tec_fin = en.fec_fin_tecno.Value,
        //                           isRevisado = ht.isRevisado,
        //                           modalidad = ea.codigoestudio.Substring(4, 1) == "1" ? "MR" : ea.codigoestudio.Substring(4, 1) == "2" ? "CT" : "",
        //                           isError_p2 = se.p2,
        //                           Error_p2 = se.p2_1,
        //                           isError_p3 = se.p3,
        //                           Error_p3 = se.p3_1,
        //                       }).ToList();


        //    //}
        //    #endregion



        //    return View(model);
        //}

        public ActionResult ListaReporte(DateTime i, DateTime f)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            ReporteViewModel model = new ReporteViewModel();
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);
            string _user = user.ProviderUserKey.ToString();
            model.isRevisado = false;
            if (fec_ini.ToShortDateString() == fec_fin.ToShortDateString())
            {
                model.rangoFiltro = fec_ini.ToShortDateString();
                if (fec_ini == new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1))
                    model.isRevisado = true;
            }
            else
                model.rangoFiltro = fec_ini.ToShortDateString() + " al " + fec_fin.ToShortDateString();

            #region PostProceso
            if (User.IsInRole("8"))//PostProceso
            {

                string sqlPost = @"select p.fecha,ea.codigo,pa.apellidos+' '+pa.nombres paciente,es.nombreestudio,p.descripcion,(ep.duracion*ep.factor) valor from postproceso p inner join examenxatencion ea on p.numeroestudio=ea.codigo  inner join Estudio_PostProceso ep on p.codigoestudio= ep.codigoestudio inner join paciente pa on ea.codigopaciente=pa.codigopaciente inner join estudio es on ea.codigoestudio = es.codigoestudio
where tipo='D' and ea.estadoestudio!='X'and p.codigousuario='" + user.ProviderUserKey.ToString() + "' and convert(date,p.fecha)>='" + fec_ini.ToShortDateString() + "' and convert(date,p.fecha)<='" + fec_fin.ToShortDateString() + "' ";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command_cita = new SqlCommand(sqlPost, connection);
                    connection.Open();
                    SqlDataReader reader = command_cita.ExecuteReader();
                    try
                    {
                        model.postproceso = new List<Data_reporte>();
                        while (reader.Read())
                        {
                            Data_reporte item = new Data_reporte();

                            item.fecha = Convert.ToDateTime(reader["fecha"]);
                            item.examen = Convert.ToInt32(reader["codigo"]);
                            item.paciente = (reader["paciente"]).ToString();
                            item.estudio = (reader["nombreestudio"]).ToString();
                            item.tecnica = (reader["descripcion"]).ToString();
                            item.valorPro = Convert.ToInt32(reader["valor"]);
                            model.postproceso.Add(item);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                }
                //model.postproceso = (from en in db.Encuesta
                //                     join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                //                     join pa in db.PACIENTE on ea.codigopaciente equals pa.codigopaciente
                //                     join us in db.USUARIO on en.usu_reg_super equals us.codigousuario into us_join
                //                     from us in us_join.DefaultIfEmpty()
                //                     join ue in db.USUARIO on en.usu_reg_encu equals ue.codigousuario into ue_join
                //                     from ue in ue_join.DefaultIfEmpty()
                //                     join ht in db.POSTPROCESO on en.numeroexamen equals ht.numeroestudio
                //                     join eq in db.Equipo_PostProceso on ht.codigoequipo equals eq.codigoequipo

                //                     where
                //                       en.usu_reg_proto == _user &&
                //                       en.fec_fin_proto != null &&
                //                       (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                //                       ea.estadoestudio != "X" &&
                //                     en.estado >= 3 &&
                //                        ht.tipo == "D"
                //                     orderby
                //                       ea.codigo
                //                     select new Data_reporte
                //                     {


                //                     }).ToList();


            }
            #endregion

            #region Enfermera
            if (User.IsInRole("4"))//Enfermera
            {

                string sqlPost = @"select c.fecha_inicio,ea.codigo,pa.apellidos+' '+pa.nombres paciente,es.nombreestudio ,dbo.ufn_getInfoContraste(idcontraste) insumos ,dbo.ufn_getInfoContrasteInsumo(idcontraste) nombreInsumo from Contraste c inner join EXAMENXATENCION ea on c.numeroexamen=ea.codigo inner join paciente pa on ea.codigopaciente=pa.codigopaciente  inner join estudio es on ea.codigoestudio = es.codigoestudio where  ea.estadoestudio!='X'and c.usuario='" + user.ProviderUserKey.ToString() + "'and convert(date,c.fecha_inicio)>='" + fec_ini.ToShortDateString() + "'and convert(date,c.fecha_inicio)>='" + fec_fin.ToShortDateString() + "' ";
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command_cita = new SqlCommand(sqlPost, connection);
                    connection.Open();
                    SqlDataReader reader = command_cita.ExecuteReader();
                    try
                    {
                        model.enfermera = new List<Data_reporte>();
                        while (reader.Read())
                        {
                            Data_reporte item = new Data_reporte();

                            item.fecha = Convert.ToDateTime(reader["fecha_inicio"]);
                            item.examen = Convert.ToInt32(reader["codigo"]);
                            item.paciente = (reader["paciente"]).ToString();
                            item.estudio = (reader["nombreestudio"]).ToString();
                            item.insumos = (reader["insumos"]).ToString();
                            item.nombreinsumos = (reader["nombreInsumo"]).ToString();
                            model.enfermera.Add(item);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                }
                //model.postproceso = (from en in db.Encuesta
                //                     join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                //                     join pa in db.PACIENTE on ea.codigopaciente equals pa.codigopaciente
                //                     join us in db.USUARIO on en.usu_reg_super equals us.codigousuario into us_join
                //                     from us in us_join.DefaultIfEmpty()
                //                     join ue in db.USUARIO on en.usu_reg_encu equals ue.codigousuario into ue_join
                //                     from ue in ue_join.DefaultIfEmpty()
                //                     join ht in db.POSTPROCESO on en.numeroexamen equals ht.numeroestudio
                //                     join eq in db.Equipo_PostProceso on ht.codigoequipo equals eq.codigoequipo

                //                     where
                //                       en.usu_reg_proto == _user &&
                //                       en.fec_fin_proto != null &&
                //                       (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                //                       ea.estadoestudio != "X" &&
                //                     en.estado >= 3 &&
                //                        ht.tipo == "D"
                //                     orderby
                //                       ea.codigo
                //                     select new Data_reporte
                //                     {


                //                     }).ToList();


            }
            #endregion

            #region Encuestador
            if (User.IsInRole("1"))//Encuestador
            {
                model.encuestador = (from en in db.Encuesta
                                     join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                                     join pa in db.PACIENTE on ea.codigopaciente equals pa.codigopaciente
                                     join eq in db.EQUIPO on ea.equipoAsignado equals eq.codigoequipo
                                     join us in db.USUARIO on en.usu_reg_super equals us.codigousuario into us_join
                                     from us in us_join.DefaultIfEmpty()
                                     join se in db.SupervisarEncuesta on en.numeroexamen equals se.numeroexamen into se_join
                                     from se in se_join.DefaultIfEmpty()
                                     where
                                       en.usu_reg_encu == _user &&
                                       en.fec_paso3 != null &&
                                       (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                                       ea.estadoestudio != "X" &&
                                       en.estado >= 3
                                     orderby
                                       ea.codigo
                                     select new Data_reporte
                                     {
                                         fecha = ea.ATENCION.fechayhora,
                                         examen = ea.codigo,
                                         paciente = pa.apellidos + " " + pa.nombres,
                                         estudio = ea.ESTUDIO.nombreestudio,
                                         equipo = eq.nombreequipo,
                                         supervisor = us.ShortName,
                                         encu_ini = en.fec_ini_encu,
                                         encu_fin = en.fec_paso3.Value,
                                         claseestudio = ea.ESTUDIO.CLASE.nombreclase,
                                         isError = se.p1,
                                         error = se.p1_1
                                     }).AsParallel().ToList();

                /* model.cali_encuestador = (from en in db.Encuesta
                                           join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                                           join se in db.SupervisarEncuesta on en.numeroexamen equals se.numeroexamen
                                           where ea.estadoestudio != "X"
                                           && (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin)
                                           && en.usu_reg_encu == _user
                                           && en.estado >= 3
                                           //&& se.p1==false
                                           orderby
                                           ea.codigo
                                           select new Data_calificacion
                                           {
                                               fecha = se.fecha_reg,
                                               examen = se.numeroexamen,
                                               paciente = ea.ATENCION.PACIENTE.apellidos + " " + ea.ATENCION.PACIENTE.nombres,
                                               estudio = ea.ESTUDIO.nombreestudio,
                                               supervisor = se.Supervisor.ShortName,
                                               isError = se.p1,
                                               error = se.p1_1
                                           }).AsParallel().ToList();*/


            }
            #endregion

            #region Supervisor
            if (User.IsInRole("2"))//Supervisor
            {
                model.supervisor = (from en in db.Encuesta
                                    join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                                    join pa in db.PACIENTE on ea.codigopaciente equals pa.codigopaciente
                                    join eq in db.EQUIPO on ea.equipoAsignado equals eq.codigoequipo
                                    join us in db.USUARIO on en.usu_reg_encu equals us.codigousuario into us_join
                                    from us in us_join.DefaultIfEmpty()
                                    join se in db.SupervisarEncuesta on en.numeroexamen equals se.numeroexamen into se_join
                                    from se in se_join.DefaultIfEmpty()
                                    where
                                      en.usu_reg_super == _user &&
                                      en.fec_supervisa != null &&
                                      (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                                      ea.estadoestudio != "X" &&
                                      en.estado >= 3
                                    orderby
                                      ea.codigo
                                    select new Data_reporte
                                    {
                                        fecha = ea.ATENCION.fechayhora,
                                        examen = ea.codigo,
                                        paciente = pa.apellidos + " " + pa.nombres,
                                        estudio = ea.ESTUDIO.nombreestudio,
                                        equipo = eq.nombreequipo,
                                        encuestador = us.ShortName,
                                        soli_ini = en.fec_Solicitavalidacion == null ? ea.ATENCION.fechayhora : en.fec_Solicitavalidacion.Value,
                                        ini_super = en.fec_ini_supervisa == null ? ea.ATENCION.fechayhora : en.fec_ini_supervisa.Value,
                                        isError = se.p1_informante,
                                        error = se.p1_1_informante,
                                        isError_p2 = se.p2_informante,
                                        Error_p2 = se.p2_1_informante,
                                        isError_p3 = se.p3_informante,
                                        Error_p3 = se.p3_1_informante,
                                        isError_p4 = se.diagnostico_info,
                                        Error_p4 = se.diagnostico_inf,
                                        med_Calificador = se.Informante.ShortName
                                    }).AsParallel().ToList();

            }
            #endregion

            #region Informante
            model.informante = new List<Data_reporte>();
            /*if (User.IsInRole("18") && !User.IsInRole("3"))//Supervisor
            {
                model.informante = (from en in db.Encuesta
                                    join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                                    join se in db.SupervisarEncuesta on en.numeroexamen equals se.numeroexamen into se_join
                                    from se in se_join.DefaultIfEmpty()
                                    where
                                      se.usu_reg_inf == _user &&
                                      en.fec_supervisa != null &&
                                      (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                                      ea.estadoestudio != "X" &&
                                      en.estado >= 3
                                    orderby
                                      ea.codigo
                                    select new Data_reporte
                                    {
                                        fecha = ea.ATENCION.fechayhora,
                                        examen = ea.codigo,
                                        paciente = ea.ATENCION.PACIENTE.apellidos + " " + ea.ATENCION.PACIENTE.nombres,
                                        estudio = ea.ESTUDIO.nombreestudio,
                                        soli_ini = en.fec_Solicitavalidacion.Value,
                                        ini_super = en.fec_ini_supervisa.Value,
                                        isError = se.p1_validador,
                                        error = se.p1_1_validador,
                                        isError_p2 = se.p2_validador,
                                        Error_p2 = se.p2_1_validador,
                                        isError_p3 = se.p3_validador,
                                        Error_p3 = se.p3_1_validador,
                                        isError_p4 = se.diagnostico_vali,
                                        Error_p4 = se.diagnostico_val,
                                        med_Calificador = se.Validador.ShortName
                                    }).AsParallel().ToList();

                model.contador_inf = (from en in db.Encuesta
                                      join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                                      join se in db.SupervisarEncuesta on en.numeroexamen equals se.numeroexamen into se_join
                                      from se in se_join.DefaultIfEmpty()
                                      join inf in db.INFORMEMEDICO on en.numeroexamen equals inf.numeroinforme into inf_join
                                      from inf in inf_join.DefaultIfEmpty()
                                      where
                                        inf.medicoinforma == _user &&
                                        (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                                        ea.estadoestudio != "X" &&
                                        en.estado >= 3
                                      select new
                                      {
                                          contador = se.usu_reg_inf == null ? (inf.medicoinforma == se.usu_reg ? 1 : 0) : 1
                                      }).Select(x => x.contador).ToList();

                model.contador_val = (from en in db.Encuesta
                                      join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                                      join se in db.SupervisarEncuesta on en.numeroexamen equals se.numeroexamen into se_join
                                      from se in se_join.DefaultIfEmpty()
                                      join inf in db.INFORMEMEDICO on en.numeroexamen equals inf.numeroinforme into inf_join
                                      from inf in inf_join.DefaultIfEmpty()
                                      where
                                        inf.medicorevisa == _user &&
                                        (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                                        ea.estadoestudio != "X" &&
                                        en.estado >= 3
                                      select new
                                      {
                                          contador = se.usu_reg_val == null ? (se.usu_reg_inf == null ? (inf.medicoinforma == se.usu_reg ? 1 : 0) : 1) : 1
                                      }).Select(x => x.contador).ToList();
            
            }*/
            #endregion

            #region Tecnologo
            if (User.IsInRole("3"))//tecnologo
            {
                model.tecnologo = (from en in db.Encuesta
                                   join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                                   join pa in db.PACIENTE on ea.codigopaciente equals pa.codigopaciente
                                   join eq in db.EQUIPO on ea.equipoAsignado equals eq.codigoequipo
                                   join us in db.USUARIO on en.usu_reg_super equals us.codigousuario into us_join
                                   from us in us_join.DefaultIfEmpty()
                                   join ue in db.USUARIO on en.usu_reg_encu equals ue.codigousuario into ue_join
                                   from ue in ue_join.DefaultIfEmpty()
                                   join ht in db.HONORARIOTECNOLOGO on en.numeroexamen equals ht.codigohonorariotecnologo
                                   join se in db.SupervisarEncuesta on en.numeroexamen equals se.numeroexamen into se_join
                                   from se in se_join.DefaultIfEmpty()
                                   where
                                     en.usu_reg_tecno == _user &&
                                     en.fec_ini_tecno != null &&
                                     (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                                     ea.estadoestudio != "X" &&
                                     en.estado >= 3
                                   orderby
                                     ea.codigo
                                   select new Data_reporte
                                   {
                                       fecha = ht.fechaestudio,
                                       examen = ea.codigo,
                                       paciente = pa.apellidos + " " + pa.nombres,
                                       estudio = ea.ESTUDIO.nombreestudio,
                                       equipo = eq.nombreequipo,
                                       placas = ht.placauso,
                                       imagenes = ea.UrlXero != null ? true : false,
                                       postproceso = ht.procesar,
                                       sedacion = ht.sedacion.Value,
                                       contraste = ht.contraste.Value,
                                       continuo = ht.contraste_continuo.Value,
                                       encuestador = ue.ShortName,
                                       supervisor = us.ShortName,
                                       tec_ini = en.fec_ini_tecno.Value,
                                       tec_fin = en.fec_fin_tecno.Value,
                                       isRevisado = ht.isRevisado,
                                       modalidad = ea.codigoestudio.Substring(4, 1) == "1" ? "MR" : ea.codigoestudio.Substring(4, 1) == "2" ? "CT" : "",
                                       isError_p2 = se.p2,
                                       Error_p2 = se.p2_1,
                                       isError_p3 = se.p3,
                                       Error_p3 = se.p3_1,
                                   }).ToList();


            }
            #endregion



            return View(model);
        }

        public ActionResult CerrarTurno()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string _user = user.ProviderUserKey.ToString();
            DateTime fec_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            var tecnologo = (from en in db.Encuesta
                             join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                             join pa in db.PACIENTE on ea.codigopaciente equals pa.codigopaciente
                             join eq in db.EQUIPO on ea.equipoAsignado equals eq.codigoequipo
                             join us in db.USUARIO on en.usu_reg_super equals us.codigousuario into us_join
                             from us in us_join.DefaultIfEmpty()
                             join ue in db.USUARIO on en.usu_reg_encu equals ue.codigousuario into ue_join
                             from ue in ue_join.DefaultIfEmpty()
                             join ht in db.HONORARIOTECNOLOGO on en.numeroexamen equals ht.codigohonorariotecnologo

                             where
                               en.usu_reg_tecno == _user &&
                               en.fec_ini_tecno != null &&
                               (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                               ea.estadoestudio != "X" &&
                               en.estado >= 3
                             orderby
                               ea.codigo
                             select ht).ToList();
            foreach (var item in tecnologo)
            {
                item.isRevisado = true;
                item.fec_revisa = DateTime.Now;
                db.SaveChanges();
            }

            return RedirectToAction("ListaReporte", new { i = DateTime.Now, f = DateTime.Now });
        }

        public ActionResult ReporteValidacionTecnologo(DateTime i, DateTime f)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            ReporteViewModel model = new ReporteViewModel();
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);
            string _user = user.ProviderUserKey.ToString();
            model.isRevisado = false;
            if (fec_ini.ToShortDateString() == fec_fin.ToShortDateString())
            {
                model.rangoFiltro = fec_ini.ToShortDateString();
                if (fec_ini == new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1))
                    model.isRevisado = true;
            }
            else
                model.rangoFiltro = fec_ini.ToShortDateString() + " al " + fec_fin.ToShortDateString();
            #region Tecnologo

            model.tecnologo = (from en in db.Encuesta
                               join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                               join pa in db.PACIENTE on ea.codigopaciente equals pa.codigopaciente
                               join eq in db.EQUIPO on ea.equipoAsignado equals eq.codigoequipo
                               join us in db.USUARIO on en.usu_reg_super equals us.codigousuario into us_join
                               from us in us_join.DefaultIfEmpty()
                               join ue in db.USUARIO on en.usu_reg_encu equals ue.codigousuario into ue_join
                               from ue in ue_join.DefaultIfEmpty()
                               join ht in db.HONORARIOTECNOLOGO on en.numeroexamen equals ht.codigohonorariotecnologo
                               join ut in db.USUARIO on ht.tecnologoturno equals ut.siglas into ut_join
                               from ut in ut_join.DefaultIfEmpty()

                               where
                                 en.fec_ini_tecno != null &&
                                 (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                                 ea.estadoestudio != "X" &&
                                 ht.isRevisado == true
                               orderby
                                 ea.codigo
                               select new Data_reporte
                               {
                                   fecha = ht.fechaestudio,
                                   tecnologo = ut.ShortName,
                                   examen = ea.codigo,
                                   paciente = pa.apellidos + " " + pa.nombres,
                                   estudio = ea.ESTUDIO.nombreestudio,
                                   equipo = eq.nombreequipo,
                                   placas = ht.placauso,
                                   imagenes = ea.UrlXero != null ? true : false,
                                   postproceso = ht.procesar,
                                   sedacion = ht.sedacion.Value,
                                   contraste = ht.contraste.Value,
                                   continuo = ht.contraste_continuo.Value,
                                   encuestador = ue.ShortName,
                                   supervisor = us.ShortName,
                                   tec_ini = en.fec_ini_tecno.Value,
                                   tec_fin = en.fec_fin_tecno.Value,
                                   isRevisado = ht.isRevisado,
                                   modalidad = ea.codigoestudio.Substring(4, 1) == "1" ? "MR" : ea.codigoestudio.Substring(4, 1) == "2" ? "CT" : ""
                               }).ToList();



            #endregion

            return View(model);
        }

        public ActionResult GeneralEncuesta(DateTime i)
        {
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            IEnumerable<ReporteGeneEncuestaViewModel> list = new List<ReporteGeneEncuestaViewModel>();
            //reporte general de encuesta
            list = (from ea in db.EXAMENXATENCION
                    join su in db.SUCURSAL on new { codigounidad = ea.codigoestudio.Substring(0, 1), codigosucursal = ea.codigoestudio.Substring(2, 1) } equals new { codigounidad = SqlFunctions.StringConvert((double)(su.codigounidad)).Trim(), codigosucursal = SqlFunctions.StringConvert((double)(su.codigosucursal)).Trim() }
                    join pa in db.PACIENTE on ea.codigopaciente equals pa.codigopaciente
                    join en in db.Encuesta on ea.codigo equals en.numeroexamen
                    join us_en in db.USUARIO on en.usu_reg_encu equals us_en.codigousuario into left_us_en
                    from us_en in left_us_en.DefaultIfEmpty()
                    join us_su in db.USUARIO on en.usu_reg_super equals us_su.codigousuario into left_us_su
                    from us_su in left_us_su.DefaultIfEmpty()
                    join us_te in db.USUARIO on en.usu_reg_tecno equals us_te.codigousuario into left_us_te
                    from us_te in left_us_te.DefaultIfEmpty()
                    where en.estado >= 3
                    && ea.estadoestudio != "X"
                    && (ea.ATENCION.fechayhora.Year == fec_ini.Year && ea.ATENCION.fechayhora.Month == fec_ini.Month)
                    select new ReporteGeneEncuestaViewModel
                    {
                        num_encuesta = ea.codigo,
                        fecha = ea.ATENCION.fechayhora,
                        estudio = ea.ESTUDIO.nombreestudio,
                        paciente = pa.apellidos + " " + pa.nombres,
                        encuestador = us_en.ShortName == null ? "Sin Encuestador" : us_en.ShortName,
                        supervisor = us_su.ShortName == null ? "Sin Supervisor" : us_su.ShortName,
                        tecnologo = us_te.ShortName == null ? "Sin Tecnologo" : us_te.ShortName,
                        clase = ea.ESTUDIO.CLASE.nombreclase,
                        sede = su.UNIDADNEGOCIO.nombre + " " + su.ShortDesc
                    }).AsParallel().ToList();
            return View(list);

        }
        [Authorize(Roles = "11")]
        public ActionResult GeneralEncuesta_Back(DateTime i)
        {
            DateTime fec_ini = new DateTime(i.Year, i.AddMonths(-1).Month, i.Day, 0, 0, 1);

            //reporte general de encuesta
            var list = (from ea in db.EXAMENXATENCION
                        join en in db.Encuesta on ea.codigo equals en.numeroexamen
                        where en.estado >= 3
                        && ea.estadoestudio != "X"
                        && (ea.ATENCION.fechayhora.Year == fec_ini.Year && ea.ATENCION.fechayhora.Month == fec_ini.Month)
                        select en.numeroexamen).AsParallel().ToList();
            fec_ini = fec_ini.AddMonths(-1);
            //reporte general de encuesta
            var list2 = (from ea in db.EXAMENXATENCION
                         join en in db.Encuesta on ea.codigo equals en.numeroexamen
                         where en.estado >= 3
                         && ea.estadoestudio != "X"
                         && (ea.ATENCION.fechayhora.Year == fec_ini.Year && ea.ATENCION.fechayhora.Month == fec_ini.Month)
                         select en.numeroexamen).AsParallel().ToList();
            fec_ini = fec_ini.AddMonths(-1);
            //reporte general de encuesta
            var list3 = (from ea in db.EXAMENXATENCION
                         join en in db.Encuesta on ea.codigo equals en.numeroexamen
                         where en.estado >= 3
                         && ea.estadoestudio != "X"
                         && (ea.ATENCION.fechayhora.Year == fec_ini.Year && ea.ATENCION.fechayhora.Month == fec_ini.Month)
                         select en.numeroexamen).AsParallel().ToList();
            return Json(new { t1 = list.Count(), t2 = list2.Count(), t3 = list3.Count() });

        }
        [Authorize(Roles = "11")]
        public ActionResult ReporteTiempoEncuesta(DateTime i)
        {
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            Variable _u = new Variable();
            IEnumerable<ReporteGeneEncuestaViewModel> list = new List<ReporteGeneEncuestaViewModel>();
            //reporte general de encuesta
            list = (from ea in db.EXAMENXATENCION
                    join en in db.Encuesta on ea.codigo equals en.numeroexamen
                    join us_en in db.USUARIO on en.usu_reg_encu equals us_en.codigousuario into left_us_en
                    from us_en in left_us_en.DefaultIfEmpty()
                    join us_su in db.USUARIO on en.usu_reg_super equals us_su.codigousuario into left_us_su
                    from us_su in left_us_su.DefaultIfEmpty()
                    join us_te in db.USUARIO on en.usu_reg_tecno equals us_te.codigousuario into left_us_te
                    from us_te in left_us_te.DefaultIfEmpty()
                    where en.estado >= 3
                    && ea.estadoestudio != "X"
                        //&& ea.codigo == 514761
                    && (ea.ATENCION.fechayhora.Year == fec_ini.Year && ea.ATENCION.fechayhora.Month == fec_ini.Month)
                    select new ReporteGeneEncuestaViewModel
                    {
                        num_encuesta = ea.codigo,
                        fecha = ea.ATENCION.fechayhora,
                        estudio = ea.ESTUDIO.nombreestudio,
                        //encuestador
                        encuestador = us_en.ShortName == null ?
                        "Sin Encuestador"
                        : us_en.ShortName,
                        encuesta = en,
                        /*min_res_encu = (Double)SqlFunctions.DateDiff("minute", ea.ATENCION.fechayhora, en.fec_ini_encu),
                        FE_encuestador_Res = Math.Round(((_u.tRespuestaEncuestador * 1.0) / (Double)SqlFunctions.DateDiff("millisecond", ea.ATENCION.fechayhora, en.fec_ini_encu)), 2),
                        */
                        /*min_dur_encu = (Double)SqlFunctions.DateDiff("minute", en.fec_ini_encu, en.fec_paso3),
                        FE_encuestador_Dur = Math.Round(((_u.tDuracionEncuestador * 1.0) / (Double)SqlFunctions.DateDiff("millisecond", en.fec_ini_encu, en.fec_paso3)), 1),*/
                        //supervisor
                        supervisor = us_su.ShortName == null ?
                        "Sin Supervisor"
                        : us_su.ShortName,

                        /* min_res_super = en.fec_supervisa != null ?
                         (Double)SqlFunctions.DateDiff("minute", en.fec_Solicitavalidacion, en.fec_ini_supervisa)
                         : 0.0,
                        */

                        /*FE_supervisor_Res = en.fec_supervisa != null ?
                        Math.Round(((_u.tRespuestaSupervisor * 1.0) / ((Double)SqlFunctions.DateDiff("millisecond", en.fec_Solicitavalidacion, en.fec_ini_supervisa)) + 1), 1)
                        : 0.0,*/

                        //tecnologo
                        tecnologo = us_te.ShortName == null ?
                        "Sin Tecnologo"
                        : us_te.ShortName,

                        /*min_dur_tecno = en.fec_supervisa != null && en.fec_ini_tecno != null && en.fec_fin_tecno != null ?
                        ((Double)SqlFunctions.DateDiff("minute", en.fec_ini_tecno, en.fec_fin_tecno) - (Double)SqlFunctions.DateDiff("minute", en.fec_Solicitavalidacion, en.fec_supervisa))
                        : (Double)SqlFunctions.DateDiff("minute", en.fec_ini_tecno, en.fec_fin_tecno),*/

                        /*FE_tecnologo_Dur = en.fec_supervisa != null && en.fec_ini_tecno != null && en.fec_fin_tecno != null ?
                         Math.Round(((_u.tDuracionTecnologo * 1.0) / ((Double)SqlFunctions.DateDiff("millisecond", en.fec_ini_tecno, en.fec_fin_tecno) - (Double)SqlFunctions.DateDiff("millisecond", en.fec_Solicitavalidacion, en.fec_supervisa))), 1)
                         : Math.Round(((_u.tDuracionTecnologo * 1.0) / (Double)SqlFunctions.DateDiff("millisecond", en.fec_ini_tecno, en.fec_fin_tecno)), 0),*/

                    }).AsParallel().ToList();
            foreach (var item in list)
            {
                TimeSpan tsp;
                TimeSpan tsp1;
                /*ENCUESTADOR*/
                if (item.tecnologo != "Sin Tecnologo")
                {
                    if (item.fecha > item.encuesta.fec_ini_encu)
                    {
                        tsp = item.fecha - item.encuesta.fec_ini_encu;
                        var add = tsp.Seconds;
                        item.fecha = item.fecha.AddSeconds((add + 1) * -1);
                    }
                    tsp = item.encuesta.fec_ini_encu - item.fecha;
                    item.min_res_encu = Math.Round(tsp.TotalMinutes, 0);
                    if (tsp.TotalSeconds > 0)
                        item.FE_encuestador_Res = Math.Round((_u.tRespuestaEncuestador * 1.0) / tsp.TotalSeconds, 2);
                    if (tsp.TotalMinutes > 0)
                        item.FE_encuestador_Res = Math.Round(((_u.tRespuestaEncuestador / 60) * 1.0) / tsp.TotalMinutes, 2);

                    tsp = item.encuesta.fec_paso3.Value - item.encuesta.fec_ini_encu;
                    item.min_dur_encu = Math.Round(tsp.TotalMinutes, 0);
                    if (tsp.TotalSeconds > 0)
                        item.FE_encuestador_Dur = Math.Round((_u.tDuracionEncuestador * 1.0) / tsp.TotalSeconds, 2);
                    if (tsp.TotalMinutes > 0)
                        item.FE_encuestador_Dur = Math.Round(((_u.tDuracionEncuestador / 60) * 1.0) / tsp.TotalMinutes, 2);
                }
                else
                {
                    item.min_res_encu = 0;
                    item.FE_encuestador_Res = 0;
                    item.FE_encuestador_Dur = 0;
                }

                /*SUPERVISOR*/
                if (item.supervisor != "Sin Supervisor")
                {
                    if (item.encuesta.fec_Solicitavalidacion.Value > item.encuesta.fec_ini_supervisa.Value)
                    {
                        tsp = item.encuesta.fec_Solicitavalidacion.Value - item.encuesta.fec_ini_supervisa.Value;
                        var add = tsp.Seconds;
                        item.encuesta.fec_Solicitavalidacion = item.encuesta.fec_Solicitavalidacion.Value.AddSeconds((add + 1) * -1);
                    }
                    tsp = item.encuesta.fec_ini_supervisa.Value - item.encuesta.fec_Solicitavalidacion.Value;
                    item.min_res_super = Math.Round(tsp.TotalMinutes, 0);
                    if (tsp.TotalMilliseconds > 0)
                        item.FE_supervisor_Res = Math.Round((_u.tRespuestaSupervisor * 1.0) / tsp.TotalMilliseconds, 2);
                    if (tsp.TotalSeconds > 0)
                        item.FE_supervisor_Res = Math.Round(((_u.tRespuestaSupervisor / 1000) * 1.0) / tsp.TotalSeconds, 2);
                    if (tsp.TotalMinutes > 0)
                        item.FE_supervisor_Res = Math.Round((((_u.tRespuestaSupervisor / 1000) / 60) * 1.0) / tsp.TotalMinutes, 2);
                }
                else
                {
                    item.min_res_super = 0;
                    item.FE_supervisor_Res = 0;
                }

                /*TECNÓLOGO*/
                if (item.tecnologo != "Sin Tecnologo")
                {
                    if (item.encuesta.fec_fin_tecno.Value > item.encuesta.fec_ini_tecno.Value)
                    {
                        tsp = item.encuesta.fec_fin_tecno.Value - item.encuesta.fec_ini_tecno.Value;
                        var add = tsp.Seconds;
                        item.encuesta.fec_fin_tecno = item.encuesta.fec_fin_tecno.Value.AddSeconds((add + 1) * -1);
                    }
                    tsp = item.encuesta.fec_fin_tecno.Value - item.encuesta.fec_ini_tecno.Value;
                    double _restaminutossupervisor = 0;
                    if (item.supervisor != "Sin Supervisor")
                    {
                        tsp1 = item.encuesta.fec_supervisa.Value - item.encuesta.fec_Solicitavalidacion.Value;
                        _restaminutossupervisor = tsp1.TotalMinutes;
                    }
                    item.min_dur_tecno = Math.Round(tsp.TotalMinutes - _restaminutossupervisor, 0);
                    if (tsp.TotalMilliseconds > 0)
                        item.FE_tecnologo_Dur = Math.Round((_u.tDuracionTecnologo * 1.0) / tsp.TotalMilliseconds, 2);
                    if (tsp.TotalSeconds > 0)
                        item.FE_tecnologo_Dur = Math.Round(((_u.tDuracionTecnologo / 1000) * 1.0) / tsp.TotalSeconds, 2);
                    if (tsp.TotalMinutes > 0)
                        item.FE_tecnologo_Dur = Math.Round((((_u.tDuracionTecnologo / 1000) / 60) * 1.0) / tsp.TotalMinutes, 2);
                }
                else
                {
                    /*min_dur_tecno = en.fec_supervisa != null && en.fec_ini_tecno != null && en.fec_fin_tecno != null ?
                       ((Double)SqlFunctions.DateDiff("minute", en.fec_ini_tecno, en.fec_fin_tecno) - (Double)SqlFunctions.DateDiff("minute", en.fec_Solicitavalidacion, en.fec_supervisa))
                       : (Double)SqlFunctions.DateDiff("minute", en.fec_ini_tecno, en.fec_fin_tecno),*/

                    /*FE_tecnologo_Dur = en.fec_supervisa != null && en.fec_ini_tecno != null && en.fec_fin_tecno != null ?
                     Math.Round(((_u.tDuracionTecnologo * 1.0) / ((Double)SqlFunctions.DateDiff("millisecond", en.fec_ini_tecno, en.fec_fin_tecno) - (Double)SqlFunctions.DateDiff("millisecond", en.fec_Solicitavalidacion, en.fec_supervisa))), 1)
                     : Math.Round(((_u.tDuracionTecnologo * 1.0) / (Double)SqlFunctions.DateDiff("millisecond", en.fec_ini_tecno, en.fec_fin_tecno)), 0),*/

                    item.min_dur_tecno = 0;
                    item.FE_tecnologo_Dur = 0;
                }
            }


            return View(list);
        }
        [Authorize(Roles = "11,20")]
        public ActionResult ReporteMedico(DateTime i, DateTime f)
        {
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);

            var lst = (from en in db.Encuesta
                       join se in db.SupervisarEncuesta on en.numeroexamen equals se.numeroexamen into se_join
                       from se in se_join.DefaultIfEmpty()
                       join inf in db.INFORMEMEDICO on en.numeroexamen equals inf.numeroinforme
                       join ea in db.EXAMENXATENCION on en.numeroexamen equals ea.codigo
                       where
                         (en.fec_ini_encu >= fec_ini && en.fec_ini_encu <= fec_fin) &&
                         ea.estadoestudio != "X" //&&
                       //en.estado >= 3
                       select new
                       {
                           usuI = inf.medicoinforma,
                           cantI = se.usu_reg_inf == null ? (inf.medicoinforma == se.usu_reg ? 1 : 0) : 1,
                           usuV = inf.medicorevisa,
                           cantV = se.usu_reg_val == null ? (se.usu_reg_inf == null ? (inf.medicoinforma == se.usu_reg ? 1 : 0) : 1) : 1
                       }).ToList();

            xxx lista = new xxx();
            lista.informante = new List<xxx_x>();
            lista.validador = new List<xxx_x>();
            foreach (var medico in lst.GroupBy(x => x.usuI))
            {
                if (medico.Key != null)
                {
                    xxx_x _med_i = new xxx_x();
                    _med_i.medico = db.USUARIO.SingleOrDefault(x => x.codigousuario == medico.Key).ShortName;
                    var total = lst.Where(x => x.usuI == medico.Key).ToList().Count();
                    var falta = total - lst.Where(x => x.usuI == medico.Key).Select(x => x.cantI).Sum();
                    _med_i.cant = ((falta * 100.00) / total).ToString("0.##") + " %";
                    lista.informante.Add(_med_i);
                }
            }

            foreach (var medico in lst.GroupBy(x => x.usuV))
            {
                if (medico.Key != null)
                {
                    xxx_x _med_v = new xxx_x();
                    _med_v.medico = db.USUARIO.SingleOrDefault(x => x.codigousuario == medico.Key).ShortName;
                    var total = lst.Where(x => x.usuV == medico.Key).ToList().Count();
                    var falta = total - lst.Where(x => x.usuV == medico.Key).Select(x => x.cantV).Sum();
                    _med_v.cant = ((falta * 100.00) / total).ToString("0.##") + " %";
                    lista.validador.Add(_med_v);
                }
            }
            return View(lista);
        }


        /**************************************/
        public ActionResult PrintReporteContraste(DateTime i, DateTime f, String lstsede, String lstproducto)
        {
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);
            String sede = "%";
            String producto = "%";

            if (lstsede == null || lstproducto == null)
            {
                sede = "%";
                producto = "%";
            }
            else
            {
                sede = lstsede.ToString();
                producto = lstproducto.ToString();
            }

            List<Reporte_Contraste> lista = new List<Reporte_Contraste>();
            DataReporteContraste data = new DataReporteContraste();
            string sqlPost = @"select a.fechayhora,c.fecha_fin, s.ShortDesc,
            e.codigo,p.nombres + ' ' + p.apellidos paciente,a.edad,a.peso,es.nombreestudio, e.estadoestudio ,(select eq.ShortDesc from EQUIPO eq where eq.codigoequipo = e.equipoAsignado) equipoAsignado,(select US.ShortName from USUARIO us where us.siglas= h.tecnologoturno) tecnologoturno,(select US.ShortName from USUARIO us where us.codigousuario = c.usuario) enfermera,c.tipo_aplicacion_contraste,
            case when dc.Al_InsumId is not null then  (	select i.Al_InsumId from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=dc.Al_InsumId) else '-1' end  Al_InsumId, case when dc.id_insumo is null then  (	select i.Nombre from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=dc.Al_InsumId) else (select i.producto from Insumo_Cum_Sunasa i where i.idInsumo=dc.id_insumo)  end producto, dc.cantidad, case when dc.isaplicado = '1' then 'SI' else 'NO' end aplicado, dc.lote 
            from Contraste c  inner join Detalle_Contraste dc on c.idcontraste = dc.id_contraste inner join EXAMENXATENCION e on c.numeroexamen = e.codigo inner join ATENCION a on e.numeroatencion = a.numeroatencion inner join PACIENTE p on e.codigopaciente = p.codigopaciente inner join ESTUDIO es on e.codigoestudio=es.codigoestudio inner join SUCURSAL s on SUBSTRING(e.codigoestudio,3,1) = s.codigosucursal and s.codigounidad = '1' 
            inner join HONORARIOTECNOLOGO h on e.codigo = h.codigohonorariotecnologo
left join AL_Stock_Item si on si.AL_Stock_ItemID = dc.Al_InsumId
			left join AL_Insum ai on si.AL_InsumId = ai.Al_InsumId
            where convert(Date,a.fechayhora) between '{0}' and '{1}' and ai.IN_SubClaseId !=3 order by a.fechayhora, s.ShortDesc, e.codigo asc";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, fec_ini.ToShortDateString(), fec_fin.ToShortDateString(), sede.ToString(), producto.ToString()), connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new Reporte_Contraste();



                        //item.fecha = fechaF.Day + "/" + fechaF.Month + "/" + fechaF.Year;
                        item.fechaB = Convert.ToDateTime(reader["fechayhora"]).ToString("dd/MM/yy hh:mm tt");
                        item.fec_aplica = Convert.ToDateTime(reader["fecha_fin"]);
                        item.sede = (reader["ShortDesc"]).ToString();
                        item.codigo = Convert.ToInt32(reader["codigo"]);
                        item.paciente = (reader["paciente"]).ToString();
                        item.edad = Convert.ToInt32(reader["edad"]);
                        item.peso = Convert.ToInt32(reader["peso"]);
                        item.edad = Convert.ToInt32(reader["edad"]);
                        item.peso = Convert.ToInt32(reader["peso"]);
                        item.nombreestudio = (reader["nombreestudio"]).ToString();
                        item.estadoestudio = (reader["estadoestudio"]).ToString();
                        item.equipo = (reader["equipoAsignado"]).ToString();
                        item.tecnologo = (reader["tecnologoturno"]).ToString();
                        item.enfermera = (reader["enfermera"]).ToString();
                        item.tipo_aplicacion = (reader["tipo_aplicacion_contraste"]).ToString();
                        item.producto = (reader["producto"]).ToString();
                        item.cantidad = Convert.ToInt32(reader["cantidad"]);
                        item.aplicado = (reader["aplicado"]).ToString();
                        item.lote = (reader["lote"]).ToString();
                        item.id_insumo = Convert.ToInt32(reader["Al_InsumId"]);
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }

                List<Resumen_Reporte_Contraste> lstresumen = new List<Resumen_Reporte_Contraste>();

                foreach (var item in lista.Select(x => new { x.id_insumo, x.producto }).Distinct())
                {
                    double total = 0, volumen = 0;


                    string sqlResumen = @"
			select count(*) total,i.Volumen from AL_Stock_Consumo so inner join AL_Stock_Item si on so.AL_Stock_ConsumoID = si.AL_Stock_ConsumoID inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId  where si.Al_InsumId = '{0}' and convert(date,so.FecSalida) between '{1}' and '{2}' group by i.Volumen";
                    SqlCommand command_resumen = new SqlCommand(string.Format(sqlResumen, item.id_insumo, fec_ini.ToShortDateString(), fec_fin.ToShortDateString()), connection);
                    connection.Open();

                    SqlDataReader reader2 = command_resumen.ExecuteReader();

                    try
                    {

                        while (reader2.Read())
                        {
                            total = Convert.ToDouble(reader2["total"].ToString());
                            volumen = Convert.ToDouble(reader2["volumen"].ToString());
                        }
                    }
                    finally
                    {
                        reader2.Close();
                        connection.Close();
                    }


                    Resumen_Reporte_Contraste det = new Resumen_Reporte_Contraste();
                    det.insumo = item.producto;
                    det.frascoSalida = total;
                    det.umedidaSalida = det.frascoSalida * volumen;
                    det.umedidaUsado = lista.Where(x => x.id_insumo == item.id_insumo).Sum(x => x.cantidad);
                    det.frascoUsado = det.umedidaUsado / volumen;
                    det.umedidaSaldo = det.umedidaSalida - det.umedidaUsado;
                    det.frascoSaldo = det.umedidaSaldo / volumen;
                    lstresumen.Add(det);
                }

                data.detalle = lista;
                data.resumen = lstresumen;

            }
            return View(data);
        }

        public ActionResult ReporteContrasteDiario(DateTime fecha)
        {

            List<Reporte_Contraste> lista = new List<Reporte_Contraste>();
            string sqlPost = @"select  s.ShortDesc,i.Nombre, si.Correlativo,i.Volumen,sum(dc.cantidad) usado from Contraste c 
inner join Detalle_Contraste dc on c.idcontraste=dc.id_contraste
inner join EXAMENXATENCION ea on c.numeroexamen=ea.codigo
inner join SUCURSAL s on convert(int,substring(ea.codigoestudio,2,2))=s.codigosucursal and s.codigounidad=1
inner join AL_Stock_Item si on dc.Al_InsumId=si.AL_Stock_ItemID
inner join AL_Insum i on si.AL_InsumId=i.Al_InsumId
where convert(date,c.fecha_inicio)='{0}'
group by s.ShortDesc,i.Nombre,si.Correlativo,i.Volumen
order by s.ShortDesc,i.Nombre,si.Correlativo
";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, fecha.ToShortDateString()), connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        var item = new Reporte_Contraste();

                        item.sede = (reader["ShortDesc"]).ToString();
                        item.producto = (reader["Nombre"]).ToString();
                        item.usado = Convert.ToDouble(reader["usado"].ToString());
                        item.lote = (reader["Correlativo"]).ToString();
                        item.merma = Convert.ToDouble(reader["Volumen"].ToString()) - item.usado;
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }
            }
            return View(lista);
        }
        public ActionResult ReporteContrasteStock()
        {

            List<Reporte_Contraste> lista = new List<Reporte_Contraste>();
            string sqlPost = @"select a.almacen,a.insumo,a.Correlativo,a.Volumen,a.usado , a.Volumen-a.usado queda from(
			select al.Nombre almacen,i.Nombre insumo,i.Volumen,si.AL_Stock_ItemID,si.Correlativo,so.FecSalida,si.AL_InsumId,
			 (select isnull(sum(dc.cantidad),0) from Detalle_Contraste dc where dc.lote = convert(varchar,si.Correlativo)and dc.Al_InsumId = si.AL_Stock_ItemID) usado,(select ii.volumen from AL_Stock_Item sii inner join AL_Insum ii on sii.Al_InsumId = ii.Al_InsumId where sii.AL_Stock_ItemID = si.AL_Stock_ItemID) total
            from AL_Stock_Item si 
            inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId 
            inner join IN_UnidadMedida u on i.VolumenUM = u.IN_UnidadMedidaId
            inner join AL_Enfermeria e on i.Al_InsumId = e.Al_InsumoId
            inner join IN_UnidadMedida um on i.ConcentracionUM = um.IN_UnidadMedidaId
            inner join IN_SubClase sc on  i.IN_SubClaseId = sc.IN_SubClaseId
            inner join Al_Stock_Consumo so on si.Al_Stock_ConsumoID=so.Al_Stock_ConsumoID
            inner join AL_Almacen al on si.AL_AlmacenId = al.AL_AlmacenId
             where e.isVisibleEnfermeria = '1' and
            (so.AL_TipoConsumoId is null or so.AL_TipoConsumoId = 1)
			--and si.Al_InsumId = '{0}' and al.CodigoEquivalente ='{1}' 
			and i.IN_SubClaseId in (1,2)
			and convert(date,so.FecSalida) > '15/01/2016' 
			)as A where 

--si esta cerrado
((a.usado=0 ) or 

--si esta abierto y tiene menos de 24 horas
((a.total > a.usado )and (convert(date,(select min(fecha_inicio) from Contraste c inner join Detalle_Contraste dc on c.idcontraste=dc.id_contraste where dc.Al_InsumId=a.AL_Stock_ItemID))

between convert(date,DATEADD(day,-1,GETDATE())) and  convert(date,GETDATE()))))-- and  convert(date,GETDATE())))) DATEADD(day,-1,GETDATE()) and  GETDATE())))

or
--conray no tiene fecha de vencimiento
(((a.total > a.usado ) and a.AL_InsumId = '1020' ))

order by a.almacen,a.insumo,a.Correlativo

";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(sqlPost, connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        var item = new Reporte_Contraste();

                        item.sede = (reader["almacen"]).ToString();
                        item.producto = (reader["insumo"]).ToString();
                        item.usado = Convert.ToDouble(reader["usado"].ToString());
                        item.lote = (reader["Correlativo"]).ToString();
                        item.merma = Convert.ToDouble(reader["queda"].ToString());
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }
            }
            return View(lista);
        }
        public ActionResult ReporteContraste(DateTime i, DateTime f)
        {
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);

            List<Reporte_Contraste> lista = new List<Reporte_Contraste>();
            DataReporteContraste data = new DataReporteContraste();
            string sqlPost = @"select a.fechayhora,c.fecha_fin, s.ShortDesc,
            e.codigo,p.nombres + ' ' + p.apellidos paciente,a.edad,a.peso,es.nombreestudio, e.estadoestudio ,(select eq.ShortDesc from EQUIPO eq where eq.codigoequipo = e.equipoAsignado) equipoAsignado,(select US.ShortName from USUARIO us where us.siglas= h.tecnologoturno) tecnologoturno,(select US.ShortName from USUARIO us where us.codigousuario = c.usuario) enfermera,c.tipo_aplicacion_contraste,
            case when dc.Al_InsumId is not null then  (	select i.Al_InsumId from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=dc.Al_InsumId) else '-1' end  Al_InsumId, case when dc.id_insumo is null then  (	select i.Nombre from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=dc.Al_InsumId) else (select i.producto from Insumo_Cum_Sunasa i where i.idInsumo=dc.id_insumo)  end producto, dc.cantidad, case when dc.isaplicado = '1' then 'SI' else 'NO' end aplicado, dc.lote 
            from Contraste c  inner join Detalle_Contraste dc on c.idcontraste = dc.id_contraste inner join EXAMENXATENCION e on c.numeroexamen = e.codigo inner join ATENCION a on e.numeroatencion = a.numeroatencion inner join PACIENTE p on e.codigopaciente = p.codigopaciente inner join ESTUDIO es on e.codigoestudio=es.codigoestudio inner join SUCURSAL s on SUBSTRING(e.codigoestudio,3,1) = s.codigosucursal and s.codigounidad = '1' 
            inner join HONORARIOTECNOLOGO h on e.codigo = h.codigohonorariotecnologo
left join AL_Stock_Item si on si.AL_Stock_ItemID = dc.Al_InsumId
			left join AL_Insum ai on si.AL_InsumId = ai.Al_InsumId
            where convert(Date,a.fechayhora) between '{0}' and '{1}' and ai.IN_SubClaseId !=3 order by a.fechayhora, s.ShortDesc, e.codigo asc";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, fec_ini.ToShortDateString(), fec_fin.ToShortDateString()), connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new Reporte_Contraste();

                        item.fecha = Convert.ToDateTime(reader["fechayhora"]);
                        item.fec_aplica = Convert.ToDateTime(reader["fecha_fin"]);
                        item.sede = (reader["ShortDesc"]).ToString();
                        item.codigo = Convert.ToInt32(reader["codigo"]);
                        item.paciente = (reader["paciente"]).ToString();
                        item.edad = Convert.ToInt32(reader["edad"]);
                        item.peso = Convert.ToInt32(reader["peso"]);
                        item.nombreestudio = (reader["nombreestudio"]).ToString();
                        item.estadoestudio = (reader["estadoestudio"]).ToString();
                        item.equipo = (reader["equipoAsignado"]).ToString();
                        item.tecnologo = (reader["tecnologoturno"]).ToString();
                        item.enfermera = (reader["enfermera"]).ToString();
                        item.tipo_aplicacion = (reader["tipo_aplicacion_contraste"]).ToString();
                        item.producto = (reader["producto"]).ToString();
                        item.cantidad = Convert.ToInt32(reader["cantidad"]);
                        item.aplicado = (reader["aplicado"]).ToString();
                        item.lote = (reader["lote"]).ToString();
                        item.id_insumo = Convert.ToInt32(reader["Al_InsumId"]);
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }

                List<Resumen_Reporte_Contraste> lstresumen = new List<Resumen_Reporte_Contraste>();

                foreach (var item in lista.Select(x => new { x.id_insumo, x.producto }).Distinct())
                {
                    double total = 0, volumen = 0;


                    string sqlResumen = @"
			select count(*) total,i.Volumen from AL_Stock_Consumo so inner join AL_Stock_Item si on so.AL_Stock_ConsumoID = si.AL_Stock_ConsumoID inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId  where si.Al_InsumId = '{0}' and convert(date,so.FecSalida) between '{1}' and '{2}' group by i.Volumen";
                    SqlCommand command_resumen = new SqlCommand(string.Format(sqlResumen, item.id_insumo, fec_ini.ToShortDateString(), fec_fin.ToShortDateString()), connection);
                    connection.Open();

                    SqlDataReader reader2 = command_resumen.ExecuteReader();

                    try
                    {

                        while (reader2.Read())
                        {
                            total = Convert.ToDouble(reader2["total"].ToString());
                            volumen = Convert.ToDouble(reader2["volumen"].ToString());
                        }
                    }
                    finally
                    {
                        reader2.Close();
                        connection.Close();
                    }


                    Resumen_Reporte_Contraste det = new Resumen_Reporte_Contraste();
                    det.insumo = item.producto;
                    det.frascoSalida = total;
                    det.umedidaSalida = det.frascoSalida * volumen;
                    det.umedidaUsado = lista.Where(x => x.id_insumo == item.id_insumo).Sum(x => x.cantidad);
                    det.frascoUsado = det.umedidaUsado / volumen;
                    det.umedidaSaldo = det.umedidaSalida - det.umedidaUsado;
                    det.frascoSaldo = det.umedidaSaldo / volumen;
                    lstresumen.Add(det);
                }

                data.detalle = lista;
                data.resumen = lstresumen;

            }
            return View(data);
        }

        public ActionResult ReporteMensualContrastesML(string date, int lstsedes)
        {
            string fecha = Convert.ToDateTime(date + "-01").ToString("MM-yyyy");
            //int mes = i.Month;
            var sede = 100 + lstsedes;

           /* if (lstsedes == 1) sede = 101;
            if (lstsedes == 6) sede = 106;
            if (lstsedes == 3) sede = 103;
            if (lstsedes == 5) sede = 105;
            if (lstsedes == 7) sede = 107;
            if (lstsedes == 8) sede = 108;
            if (lstsedes == 4) sede = 104;
            if (lstsedes == 9) sede = 109;
            */


            List<Reporte_Mensual_ML_Contraste> lista = new List<Reporte_Mensual_ML_Contraste>();

            DataReporteMensualMLContraste data = new DataReporteMensualMLContraste();
            string sqlPost = @"declare @fecha varchar(20)

set @fecha='{0}'
select A.dia,A.Nombre,SUM(a.cantidad) cant from (select day(c.fecha_inicio)dia,ai.nombre, dc.cantidad,a.Nombre sede from Contraste c
inner join Detalle_Contraste dc on c.idcontraste=dc.id_contraste
inner join AL_Stock_Item si on si.AL_Stock_ItemID = dc.Al_InsumId
inner join AL_Insum ai on si.AL_InsumId=ai.Al_InsumId
inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
where
convert(date,c.fecha_inicio) between '01-'+@fecha and DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,'01-'+@fecha)+1,0))
and a.CodigoEquivalente={1}
) as A
group by a.dia,a.Nombre
order by a.Nombre
";
            #region sqlrenato
 /*@" select Nombre, (select top 1 datename(Month,c.fecha_fin) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '{0}' ) as 'Mes' ,
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}' 
 and day(c.fecha_fin) = 1 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '1',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 2 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '2',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}' 
 and day(c.fecha_fin) = 3 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '3',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 4 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '4',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 5 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '5',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 6 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '6',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 7 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '7',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 8 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '8',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 9 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '9',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 10 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '10',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 11 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '11',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 12 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '12',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 13 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '13',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 14 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '14',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 15 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '15',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 16 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '16',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 17 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '17',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 18 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '18',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 19 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '19',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 20 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '20',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 21 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '21',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 22 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '22',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 23 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '23',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 24 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '24',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 25 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '25',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 26 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '26',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 27 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '27',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 28 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '28',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 29 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '29',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 30 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '30',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad),2) is null then 0 else Convert(Decimal(15,2),sum(dc.cantidad),2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 31 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '31'

from AL_Insum ai where ai.IsActive = 1 and ai.Correlativo != 0 and ai.IN_SubClaseId not in (3,4) ";
  * */
#endregion

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, fecha, sede.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {
                        var item = new Reporte_Mensual_ML_Contraste();
                        while (reader.Read())
                        {
                            if (item.nom_contraste != reader["nombre"].ToString())
                            {
                                if (item.nom_contraste != null)
                                    lista.Add(item);
                                item = new Reporte_Mensual_ML_Contraste();
                                item.nom_contraste = reader["nombre"].ToString();
                                item.mes = Convert.ToDateTime("01-" + fecha).ToString("MMMM");
                            }
                            double cantidad = Convert.ToDouble(reader["cant"].ToString());

                            switch (Convert.ToInt32(reader["dia"].ToString()))
                            {
                                case 1:
                                    item.dia1 = cantidad;
                                    break;
                                case 2:
                                    item.dia2 = cantidad;
                                    break;
                                case 3:
                                    item.dia3 = cantidad;
                                    break;
                                case 4:
                                    item.dia4 = cantidad;
                                    break;
                                case 5:
                                    item.dia5 = cantidad;
                                    break;
                                case 6:
                                    item.dia6 = cantidad;
                                    break;
                                case 7:
                                    item.dia7 = cantidad;
                                    break;
                                case 8:
                                    item.dia8 = cantidad;
                                    break;
                                case 9:
                                    item.dia9 = cantidad;
                                    break;
                                case 10:
                                    item.dia10 = cantidad;
                                    break;
                                case 11:
                                    item.dia11 = cantidad;
                                    break;
                                case 12:
                                    item.dia12 = cantidad;
                                    break;
                                case 13:
                                    item.dia13 = cantidad;
                                    break;
                                case 14:
                                    item.dia14 = cantidad;
                                    break;
                                case 15:
                                    item.dia15 = cantidad;
                                    break;
                                case 16:
                                    item.dia16 = cantidad;
                                    break;
                                case 17:
                                    item.dia17 = cantidad;
                                    break;
                                case 18:
                                    item.dia18 = cantidad;
                                    break;
                                case 19:
                                    item.dia19 = cantidad;
                                    break;
                                case 20:
                                    item.dia20 = cantidad;
                                    break;
                                case 21:
                                    item.dia21 = cantidad;
                                    break;
                                case 22:
                                    item.dia22 = cantidad;
                                    break;
                                case 23:
                                    item.dia23 = cantidad;
                                    break;
                                case 24:
                                    item.dia24 = cantidad;
                                    break;
                                case 25:
                                    item.dia25 = cantidad;
                                    break;
                                case 26:
                                    item.dia26 = cantidad;
                                    break;
                                case 27:
                                    item.dia27 = cantidad;
                                    break;
                                case 28:
                                    item.dia28 = cantidad;
                                    break;
                                case 29:
                                    item.dia29 = cantidad;
                                    break;
                                case 30:
                                    item.dia30 = cantidad;
                                    break;
                                case 31:
                                    item.dia31 = cantidad;
                                    break;
                                default:
                                    break;
                            }
                            
                        }
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }


                    data.mensual = lista;


                }
                return View(data);
            }
        }

        public ActionResult ReporteMensualContrastes(string date, int lstsedes)
        {
            string fecha=Convert.ToDateTime(date+"-01").ToString("MM-yyyy");
            //int mes = i.Month;
            var sede = 100 + lstsedes;

            /*if (lstsedes == 1) sede = 101;
            if (lstsedes == 6) sede = 106;
            if (lstsedes == 3) sede = 103;
            if (lstsedes == 5) sede = 105;
            if (lstsedes == 7) sede = 107;
            if (lstsedes == 8) sede = 108;
            if (lstsedes == 4) sede = 104;
            if (lstsedes == 9) sede = 109;
            */


            List<Reporte_Mensual_F_Contraste> lista = new List<Reporte_Mensual_F_Contraste>();

            DataReporteMensualFContraste data = new DataReporteMensualFContraste();
            string sqlPost = @"declare @fecha varchar(20)

set @fecha='{0}'
select A.dia,A.Nombre,COUNT(a.Correlativo) cant from (select distinct day(c.fecha_inicio)dia,ai.nombre, si.Correlativo,a.Nombre sede from Contraste c
inner join Detalle_Contraste dc on c.idcontraste=dc.id_contraste
inner join AL_Stock_Item si on si.AL_Stock_ItemID = dc.Al_InsumId
inner join AL_Insum ai on si.AL_InsumId=ai.Al_InsumId
inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
where
convert(date,c.fecha_inicio) between '01-'+@fecha and DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,'01-'+@fecha)+1,0))
and a.CodigoEquivalente={1}
) as A
group by a.dia,a.Nombre
order by a.Nombre
";
            #region SQL RENATO
            /*
@" select Nombre, (select top 1 datename(Month,c.fecha_fin) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '{0}' ) as 'Mes' ,
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}' 
 and day(c.fecha_fin) = 1 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '1',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 2 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '2',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}' 
 and day(c.fecha_fin) = 3 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '3',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 4 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '4',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 5 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '5',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 6 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '6',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 7 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '7',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 8 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '8',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 9 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '9',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 10 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '10',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 11 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '11',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 12 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '12',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 13 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '13',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 14 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '14',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 15 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '15',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 16 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '16',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 17 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '17',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 18 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '18',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 19 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '19',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 20 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '20',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 21 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '21',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 22 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '22',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 23 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '23',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 24 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '24',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 25 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '25',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 26 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '26',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 27 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '27',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 28 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '28',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 29 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '29',
 (select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 30 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '30',
(select case when ai.Al_InsumId in ('1027') then case when Convert(Decimal(15,2),sum(dc.cantidad)/5,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/5,2) end
 when ai.Al_InsumId in ('1005') then case when Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/7.50,2) end
 when ai.Al_InsumId in ('1024','1025') then case when Convert(Decimal(15,2),sum(dc.cantidad)/10,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/10,2) end
 when ai.Al_InsumId in ('1001','1003') then case when Convert(Decimal(15,2),sum(dc.cantidad)/15,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/15,2) end
 when ai.Al_InsumId in ('1012','1028') then case when Convert(Decimal(15,2),sum(dc.cantidad)/20,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/20,2) end
 when ai.Al_InsumId in ('1029','1013','1014','1015','1004') then case when Convert(Decimal(15,2),sum(dc.cantidad)/50,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/50,2) end
 when ai.Al_InsumId in ('1002') then case when Convert(Decimal(15,2),sum(dc.cantidad)/60,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/60,2) end
 when ai.Al_InsumId in ('1006','1007') then case when Convert(Decimal(15,2),sum(dc.cantidad)/75,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/75,2) end
 when ai.Al_InsumId in ('1008','1009','1016','1017','1021','1022') then case when Convert(Decimal(15,2),sum(dc.cantidad)/100,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/100,2) end
 when ai.Al_InsumId in ('1010','1011') then case when Convert(Decimal(15,2),sum(dc.cantidad)/125,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/125,2) end
 when ai.Al_InsumId in ('1020') then case when Convert(Decimal(15,2),sum(dc.cantidad)/150,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/150,2) end
 when ai.Al_InsumId in ('1031') then case when Convert(Decimal(15,2),sum(dc.cantidad)/250,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/250,2) end
 when ai.Al_InsumId in ('1018','1019') then case when Convert(Decimal(15,2),sum(dc.cantidad)/500,2) is null then 0.00 else Convert(Decimal(15,2),sum(dc.cantidad)/500,2) end
 end
 from AL_Stock_Item si 
 inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID 
 inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId 
 inner join Contraste c on dc.id_contraste = c.idcontraste 
 inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId 
 where month(c.fecha_fin) = '{0}'  
 and day(c.fecha_fin) = 31 
 and si.AL_InsumId = ai.Al_InsumId
 and a.CodigoEquivalente = '{1}') as '31'

from AL_Insum ai where ai.IsActive = 1 and ai.Correlativo != 0 and ai.IN_SubClaseId not in (3,4) "*/
            #endregion

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, fecha, sede.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {
                        if (reader.HasRows)
                        {
                            var item = new Reporte_Mensual_F_Contraste();
                            while (reader.Read())
                            {
                                if (item.nom_contraste != reader["nombre"].ToString())
                                {
                                    if (item.nom_contraste!=null)
                                        lista.Add(item);
                                    item = new Reporte_Mensual_F_Contraste();
                                    item.nom_contraste = reader["nombre"].ToString();
                                    item.mes = Convert.ToDateTime("01-" + fecha).ToString("MMMM");
                                }
                                int cantidad = Convert.ToInt32(reader["cant"].ToString());
                                switch (Convert.ToInt32(reader["dia"].ToString()))
                                {
                                    case 1:
                                        item.dia1 = cantidad;
                                        break;
                                    case 2:
                                        item.dia2 = cantidad;
                                        break;
                                    case 3:
                                        item.dia3 = cantidad;
                                        break;
                                    case 4:
                                        item.dia4 = cantidad;
                                        break;
                                    case 5:
                                        item.dia5 = cantidad;
                                        break;
                                    case 6:
                                        item.dia6 = cantidad;
                                        break;
                                    case 7:
                                        item.dia7 = cantidad;
                                        break;
                                    case 8:
                                        item.dia8 = cantidad;
                                        break;
                                    case 9:
                                        item.dia9 = cantidad;
                                        break;
                                    case 10:
                                        item.dia10 = cantidad;
                                        break;
                                    case 11:
                                        item.dia11 = cantidad;
                                        break;
                                    case 12:
                                        item.dia12 = cantidad;
                                        break;
                                    case 13:
                                        item.dia13 = cantidad;
                                        break;
                                    case 14:
                                        item.dia14 = cantidad;
                                        break;
                                    case 15:
                                        item.dia15 = cantidad;
                                        break;
                                    case 16:
                                        item.dia16 = cantidad;
                                        break;
                                    case 17:
                                        item.dia17 = cantidad;
                                        break;
                                    case 18:
                                        item.dia18 = cantidad;
                                        break;
                                    case 19:
                                        item.dia19 = cantidad;
                                        break;
                                    case 20:
                                        item.dia20 = cantidad;
                                        break;
                                    case 21:
                                        item.dia21 = cantidad;
                                        break;
                                    case 22:
                                        item.dia22 = cantidad;
                                        break;
                                    case 23:
                                        item.dia23 = cantidad;
                                        break;
                                    case 24:
                                        item.dia24 = cantidad;
                                        break;
                                    case 25:
                                        item.dia25 = cantidad;
                                        break;
                                    case 26:
                                        item.dia26 = cantidad;
                                        break;
                                    case 27:
                                        item.dia27 = cantidad;
                                        break;
                                    case 28:
                                        item.dia28 = cantidad;
                                        break;
                                    case 29:
                                        item.dia29 = cantidad;
                                        break;
                                    case 30:
                                        item.dia30 = cantidad;
                                        break;
                                    case 31:
                                        item.dia31 = cantidad;
                                        break;
                                    default:
                                        break;
                                }
                                /* 
                                 item.nom_contraste = (reader["Nombre"]).ToString();
                                 item.mes = (reader["Mes"]).ToString();
                                 if (reader["1"] != DBNull.Value)
                                     item.dia1 = Convert.ToDouble(reader["1"]);
                                 if (reader["2"] != DBNull.Value)
                                     item.dia2 = Convert.ToDouble(reader["2"]);
                                 if (reader["3"] != DBNull.Value)
                                     item.dia3 = Convert.ToDouble(reader["3"]);
                                 if (reader["4"] != DBNull.Value)
                                     item.dia4 = Convert.ToDouble(reader["4"]);
                                 if (reader["5"] != DBNull.Value)
                                     item.dia5 = Convert.ToDouble(reader["5"]);
                                 if (reader["6"] != DBNull.Value)
                                     item.dia6 = Convert.ToDouble(reader["6"]);
                                 if (reader["7"] != DBNull.Value)
                                     item.dia7 = Convert.ToDouble(reader["7"]);
                                 if (reader["8"] != DBNull.Value)
                                     item.dia8 = Convert.ToDouble(reader["8"]);
                                 if (reader["9"] != DBNull.Value)
                                     item.dia9 = Convert.ToDouble(reader["9"]);
                                 if (reader["10"] != DBNull.Value)
                                     item.dia10 = Convert.ToDouble(reader["10"]);
                                 if (reader["11"] != DBNull.Value)
                                     item.dia11 = Convert.ToDouble(reader["11"]);
                                 if (reader["12"] != DBNull.Value)
                                     item.dia12 = Convert.ToDouble(reader["12"]);
                                 if (reader["13"] != DBNull.Value)
                                     item.dia13 = Convert.ToDouble(reader["13"]);
                                 if (reader["14"] != DBNull.Value)
                                     item.dia14 = Convert.ToDouble(reader["14"]);
                                 if (reader["15"] != DBNull.Value)
                                     item.dia15 = Convert.ToDouble(reader["15"]);
                                 if (reader["16"] != DBNull.Value)
                                     item.dia16 = Convert.ToDouble(reader["16"]);
                                 if (reader["17"] != DBNull.Value)
                                     item.dia17 = Convert.ToDouble(reader["17"]);
                                 if (reader["18"] != DBNull.Value)
                                     item.dia18 = Convert.ToDouble(reader["18"]);
                                 if (reader["19"] != DBNull.Value)
                                     item.dia19 = Convert.ToDouble(reader["19"]);
                                 if (reader["20"] != DBNull.Value)
                                     item.dia20 = Convert.ToDouble(reader["20"]);
                                 if (reader["21"] != DBNull.Value)
                                     item.dia21 = Convert.ToDouble(reader["21"]);
                                 if (reader["22"] != DBNull.Value)
                                     item.dia22 = Convert.ToDouble(reader["22"]);
                                 if (reader["23"] != DBNull.Value)
                                     item.dia23 = Convert.ToDouble(reader["23"]);
                                 if (reader["24"] != DBNull.Value)
                                     item.dia24 = Convert.ToDouble(reader["24"]);
                                 if (reader["25"] != DBNull.Value)
                                     item.dia25 = Convert.ToDouble(reader["25"]);
                                 if (reader["26"] != DBNull.Value)
                                     item.dia26 = Convert.ToDouble(reader["26"]);
                                 if (reader["27"] != DBNull.Value)
                                     item.dia27 = Convert.ToDouble(reader["27"]);
                                 if (reader["28"] != DBNull.Value)
                                     item.dia28 = Convert.ToDouble(reader["28"]);
                                 if (reader["29"] != DBNull.Value)
                                     item.dia29 = Convert.ToDouble(reader["29"]);
                                 if (reader["30"] != DBNull.Value)
                                     item.dia30 = Convert.ToDouble(reader["30"]);
                                 if (reader["31"] != DBNull.Value)
                                     item.dia31 = Convert.ToDouble(reader["31"]);
                                */



                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }


                    data.mensual = lista;


                }
                return View(data);
            }
        }
        public ActionResult ReporteMensualExamenesContrastados(int i, int lstsedes)
        {
            //int mes = i.Month;
            var sede = 0;

            if (lstsedes == 1) sede = 101;
            if (lstsedes == 6) sede = 106;
            if (lstsedes == 3) sede = 103;
            if (lstsedes == 5) sede = 105;
            if (lstsedes == 7) sede = 107;
            if (lstsedes == 8) sede = 108;
            if (lstsedes == 4) sede = 104;
            if (lstsedes == 9) sede = 109;



            List<Reporte_Mensual_Contraste> lista = new List<Reporte_Mensual_Contraste>();

            DataReporteMensualContraste data = new DataReporteMensualContraste();
            string sqlPost = @" select Nombre, (select top 1 datename(Month,c.fecha_fin) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' ) as 'Mes' ,(select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 1 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '1', (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 2 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '2' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 3 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '3' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 4 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '4' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 5 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '5' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 6 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '6' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 7 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente ='" + sede + "') as '7' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 8 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '8' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 9 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '9' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 10 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '10' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 11 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '11' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 12 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '12' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 13 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '13' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 14 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '14' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 15 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '15' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 16 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '16' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 17 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '17' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 18 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '18' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 19 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '19' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 20 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '20' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 21 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente ='" + sede + "') as '21' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 22 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '22' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 23 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '23' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 24 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '24' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 25 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '25' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 26 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '26' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 27 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '27' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 28 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '28' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 29 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '29' , (select count(*) from AL_Stock_Item si  inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 30 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '30' , (select count(*) from AL_Stock_Item si inner join AL_Stock_Consumo sc on si.AL_Stock_ConsumoID = sc.AL_Stock_ConsumoID inner join Detalle_Contraste dc on si.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Almacen a on si.AL_AlmacenId = a.AL_AlmacenId where month(c.fecha_fin) = '" + i + "' and day(c.fecha_fin) = 31 and si.AL_InsumId = ai.Al_InsumId and a.CodigoEquivalente = '" + sede + "') as '31' from AL_Insum ai where ai.IsActive = 1 and ai.IN_SubClaseId !=3";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(sqlPost, connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new Reporte_Mensual_Contraste();
                        item.nom_contraste = (reader["Nombre"]).ToString();
                        item.mes = (reader["Mes"]).ToString();
                        item.dia1 = Convert.ToInt32(reader["1"]);
                        item.dia2 = Convert.ToInt32(reader["2"]);
                        item.dia3 = Convert.ToInt32(reader["3"]);
                        item.dia4 = Convert.ToInt32(reader["4"]);
                        item.dia5 = Convert.ToInt32(reader["5"]);
                        item.dia6 = Convert.ToInt32(reader["6"]);
                        item.dia7 = Convert.ToInt32(reader["7"]);
                        item.dia8 = Convert.ToInt32(reader["8"]);
                        item.dia9 = Convert.ToInt32(reader["9"]);
                        item.dia10 = Convert.ToInt32(reader["10"]);
                        item.dia11 = Convert.ToInt32(reader["11"]);
                        item.dia12 = Convert.ToInt32(reader["12"]);
                        item.dia13 = Convert.ToInt32(reader["13"]);
                        item.dia14 = Convert.ToInt32(reader["14"]);
                        item.dia15 = Convert.ToInt32(reader["15"]);
                        item.dia16 = Convert.ToInt32(reader["16"]);
                        item.dia17 = Convert.ToInt32(reader["17"]);
                        item.dia18 = Convert.ToInt32(reader["18"]);
                        item.dia19 = Convert.ToInt32(reader["19"]);
                        item.dia20 = Convert.ToInt32(reader["20"]);
                        item.dia21 = Convert.ToInt32(reader["21"]);
                        item.dia22 = Convert.ToInt32(reader["22"]);
                        item.dia23 = Convert.ToInt32(reader["23"]);
                        item.dia24 = Convert.ToInt32(reader["24"]);
                        item.dia25 = Convert.ToInt32(reader["25"]);
                        item.dia26 = Convert.ToInt32(reader["26"]);
                        item.dia27 = Convert.ToInt32(reader["27"]);
                        item.dia28 = Convert.ToInt32(reader["28"]);
                        item.dia29 = Convert.ToInt32(reader["29"]);
                        item.dia30 = Convert.ToInt32(reader["30"]);
                        item.dia31 = Convert.ToInt32(reader["31"]);
                        item.total = item.dia1 + item.dia2 + item.dia3 + item.dia4 + item.dia5 + item.dia6 + item.dia7 + item.dia8 + item.dia9 + item.dia10 + item.dia11 + item.dia12 + item.dia13 + item.dia14 + item.dia15 + item.dia16 + item.dia17 + item.dia18 + item.dia19 + item.dia20 + item.dia21 + item.dia22 + item.dia23 + item.dia24 + item.dia25 + item.dia26 + item.dia27 + item.dia28 + item.dia29 + item.dia30 + item.dia31;
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }


                data.mensual = lista;


            }
            return View(data);
        }

        public ActionResult ReporteFrasco(int lstsedes, string lstproducto)
        {
            //DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);

            var sede = 0;
            var cantidad = 0.00;

            if (lstsedes == 1) sede = 1000;
            else if (lstsedes == 6) sede = 1003;
            else if (lstsedes == 3) sede = 1004;
            else if (lstsedes == 5) sede = 1005;
            else if (lstsedes == 7) sede = 1006;
            else if (lstsedes == 8) sede = 1007;
            else if (lstsedes == 4) sede = 1008;
            else if (lstsedes == 9) sede = 1009;

            if (lstproducto == "1001" || lstproducto == "1003") cantidad = 15;
            else if (lstproducto == "1002") cantidad = 60;
            else if (lstproducto == "1004" || lstproducto == "1013" || lstproducto == "1014" || lstproducto == "1015" || lstproducto == "1029") cantidad = 50;
            else if (lstproducto == "1005") cantidad = 7.50;
            else if (lstproducto == "1006" || lstproducto == "1007") cantidad = 75;
            else if (lstproducto == "1008" || lstproducto == "1009" || lstproducto == "1016" || lstproducto == "1017" || lstproducto == "1021" || lstproducto == "1022") cantidad = 100;
            else if (lstproducto == "1010" || lstproducto == "1011") cantidad = 125;
            else if (lstproducto == "1012" || lstproducto == "1028") cantidad = 20;
            else if (lstproducto == "1018" || lstproducto == "1019") cantidad = 500;
            else if (lstproducto == "1027") cantidad = 5;
            else if (lstproducto == "1024" || lstproducto == "1025") cantidad = 10;
            else if (lstproducto == "1020") cantidad = 150;
            else if (lstproducto == "1031") cantidad = 250;

            List<Reporte_Frasco> lista = new List<Reporte_Frasco>();
            DataReporteFrasco data = new DataReporteFrasco();
            string sqlPost = @"select a.Nombre,a.Correlativo,a.total,a.usado,a.final from(select f.Nombre,f.Correlativo,f.Volumen, f.AL_Stock_ItemID, (select ii.volumen from AL_Stock_Item sii inner join AL_Insum ii on sii.Al_InsumId = ii.Al_InsumId where sii.AL_Stock_ItemID = f.AL_Stock_ItemID) total,(select isnull(sum(dc.cantidad),0) from Detalle_Contraste dc where dc.lote = convert(varchar,f.Correlativo) and dc.Al_InsumId = f.AL_Stock_ItemID) usado,(select ii.volumen from AL_Stock_Item sii inner join AL_Insum ii on sii.Al_InsumId = ii.Al_InsumId where sii.AL_Stock_ItemID = f.AL_Stock_ItemID) - (select isnull(sum(dc.cantidad),0) from Detalle_Contraste dc where dc.lote = convert(varchar,f.Correlativo) and dc.Al_InsumId = f.AL_Stock_ItemID) final, f.fecSalida from (select i.Nombre,si.Correlativo, i.Volumen, round((sum(dc.cantidad)/'" + cantidad + "'),2) as final, si.AL_Stock_ItemID,convert(date,so.FecSalida) as fecSalida from Detalle_Contraste dc right join AL_Stock_Item si on dc.Al_InsumId = si.AL_Stock_ItemID inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId inner join Al_Stock_Consumo so on si.AL_Stock_ConsumoID = so.AL_Stock_ConsumoID where si.AL_AlmacenId = '" + sede + "' and si.AL_InsumId = '" + lstproducto.ToString() + "' and (dc.cantidad is null or dc.cantidad < '" + cantidad + "') and si.AL_Stock_ConsumoID is not null and convert(date,so.FecSalida) > '15/01/2016' group by si.Correlativo,i.Volumen,si.AL_Stock_ItemID,convert(date,so.FecSalida), i.Nombre having convert(int,round((sum(dc.cantidad)/'" + cantidad + "'),2)) != 1) as f) as a inner join Detalle_Contraste dc on a.AL_Stock_ItemID = dc.Al_InsumId inner join Contraste c on dc.id_contraste = c.idcontraste inner join AL_Stock_Item si on dc.Al_InsumId = si.AL_Stock_ItemID where ((a.total > a.usado )and (convert(date,(c.fecha_fin)) between convert(date,DATEADD(day,-1,GETDATE())) and  convert(date,GETDATE()))) or ((a.total > a.usado ) and si.AL_InsumId = '1020' and (convert(date,a.FecSalida) between convert(date,DATEADD(MONTH,-1,GETDATE())) and  convert(date,GETDATE()))) or ((a.total > a.usado ) and (convert(date,a.FecSalida) between convert(date,DATEADD(day,-1,GETDATE())) and  convert(date,GETDATE()))) group by a.Correlativo,a.total,a.usado,a.final, a.Nombre union all select  b.Nombre, b.Correlativo,b.total,b.usado,b.final from (select g.Nombre, g.Correlativo,g.Volumen, g.AL_Stock_ItemID, (select ii.volumen from AL_Stock_Item sii inner join AL_Insum ii on sii.Al_InsumId = ii.Al_InsumId where sii.AL_Stock_ItemID = g.AL_Stock_ItemID) total, (select isnull(sum(dc.cantidad),0) from Detalle_Contraste dc where dc.lote = convert(varchar,g.Correlativo) and dc.Al_InsumId = g.AL_Stock_ItemID) usado, ((select ii.volumen from AL_Stock_Item sii inner join AL_Insum ii on sii.Al_InsumId = ii.Al_InsumId where sii.AL_Stock_ItemID = g.AL_Stock_ItemID) - (select isnull(sum(dc.cantidad),0) from Detalle_Contraste dc where dc.lote = convert(varchar,g.Correlativo) and dc.Al_InsumId = g.AL_Stock_ItemID)) final from (select i.Nombre, si.Correlativo, i.Volumen, round((sum(dc.cantidad)/'" + cantidad + "'),2) as final, si.AL_Stock_ItemID from Detalle_Contraste dc right join AL_Stock_Item si on dc.Al_InsumId = si.AL_Stock_ItemID  inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId inner join Al_Stock_Consumo so on si.AL_Stock_ConsumoID = so.AL_Stock_ConsumoID where si.AL_AlmacenId = '" + sede + "' and si.AL_InsumId = '" + lstproducto.ToString() + "' and (dc.cantidad is null) and si.AL_Stock_ConsumoID is not null and convert(date,so.FecSalida) > '15/01/2016' group by si.Correlativo,i.Volumen,si.AL_Stock_ItemID, i.Nombre) as g) as b left join Detalle_Contraste dc on b.AL_Stock_ItemID = dc.Al_InsumId left join Contraste c on dc.id_contraste = c.idcontraste order by Correlativo";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(sqlPost, connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new Reporte_Frasco();
                        //string fechaF = Convert.ToDateTime(reader["fecha"]).ToString("dd/MM/yyyy");
                        item.nombre = (reader["nombre"]).ToString();
                        item.frasco = Convert.ToInt32(reader["Correlativo"]);
                        item.inicial = Convert.ToDouble(reader["total"]);
                        item.usado = Convert.ToDouble(reader["usado"]);
                        item.final = Convert.ToDouble(reader["final"]);
                        //item.fecha = fechaF;
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }


                data.frasco = lista;


            }
            return View(data);
        }


        public ActionResult ReporteMerma(int lstsedes, string lstproducto, int i)
        {
            //DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);

            var sede = 0;
            //var cantidad = 0.00;

            if (lstsedes == 1) sede = 1000;
            else if (lstsedes == 6) sede = 1003;
            else if (lstsedes == 3) sede = 1004;
            else if (lstsedes == 5) sede = 1005;
            else if (lstsedes == 7) sede = 1006;
            else if (lstsedes == 8) sede = 1007;
            else if (lstsedes == 4) sede = 1008;
            else if (lstsedes == 9) sede = 1009;

            //if (lstproducto == "1001" || lstproducto == "1003") cantidad = 15;
            //else if (lstproducto == "1002") cantidad = 60;
            //else if (lstproducto == "1004" || lstproducto == "1013" || lstproducto == "1014" || lstproducto == "1015" || lstproducto == "1029") cantidad = 50;
            //else if (lstproducto == "1005") cantidad = 7.50;
            //else if (lstproducto == "1006" || lstproducto == "1007") cantidad = 75;
            //else if (lstproducto == "1008" || lstproducto == "1009" || lstproducto == "1016" || lstproducto == "1017" || lstproducto == "1021" || lstproducto == "1022") cantidad = 100;
            //else if (lstproducto == "1010" || lstproducto == "1011") cantidad = 125;
            //else if (lstproducto == "1012" || lstproducto == "1028") cantidad = 20;
            //else if (lstproducto == "1018" || lstproducto == "1019") cantidad = 500;
            //else if (lstproducto == "1027") cantidad = 5;
            //else if (lstproducto == "1024" || lstproducto == "1025") cantidad = 10;
            //else if (lstproducto == "1020") cantidad = 150;
            //else if (lstproducto == "1031") cantidad = 250;

            List<Reporte_Merma> lista = new List<Reporte_Merma>();
            DataReporteMerma data = new DataReporteMerma();
            string sqlPost = @"select i.Nombre,si.Correlativo, i.Volumen, round((sum(dc.cantidad)),2) as usado,i.Volumen - round((sum(dc.cantidad)),2)  as queda, dc.Al_InsumId,convert(date,so.FecSalida) as fecha
from Detalle_Contraste dc
inner join Contraste c on dc.id_contraste = c.idcontraste 
right join AL_Stock_Item si on dc.Al_InsumId = si.AL_Stock_ItemID 
inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId 
inner join Al_Stock_Consumo so on si.AL_Stock_ConsumoID = so.AL_Stock_ConsumoID
where si.AL_InsumId = '{0}' 
and si.AL_AlmacenId = '{1}' 
and si.AL_Stock_ConsumoID is not null
and month(so.FecSalida) = '{2}'
group by i.Nombre,si.Correlativo,i.Volumen,si.AL_Stock_ItemID,dc.Al_InsumId,convert(date,so.FecSalida)
having  i.Volumen - round((sum(dc.cantidad)),2) > 0";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, lstproducto.ToString(), sede.ToString(), i.ToString()), connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new Reporte_Merma();
                        string fechaF = Convert.ToDateTime(reader["fecha"]).ToString("dd/MM/yyyy");
                        item.nombre = (reader["nombre"]).ToString();
                        item.frasco = Convert.ToInt32(reader["Correlativo"]);
                        item.inicial = Convert.ToDouble(reader["Volumen"]);
                        item.usado = Convert.ToDouble(reader["usado"]);
                        item.final = Convert.ToDouble(reader["queda"]);
                        item.fecha = fechaF;
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }


                data.merma = lista;


            }
            return View(data);
        }

        public ActionResult ReportePrueba(DateTime i, DateTime f, String lstsede, String lstproducto)
        {
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);
            String sede = "%";
            String producto = "%";

            if (lstsede == null || lstproducto == null)
            {
                sede = "%";
                producto = "%";
            }
            else
            {
                sede = lstsede.ToString();
                producto = lstproducto.ToString();
            }

            List<Reporte_Contraste> lista = new List<Reporte_Contraste>();
            DataReporteContraste data = new DataReporteContraste();
            string sqlPost = @"select a.fechayhora, s.ShortDesc,
            e.codigo,p.nombres + ' ' + p.apellidos paciente,a.edad,a.peso, SUBSTRING(es.nombreestudio,1,11) nombreestudio, e.estadoestudio ,(select eq.ShortDesc from EQUIPO eq where eq.codigoequipo = e.equipoAsignado) equipoAsignado,(select US.siglas from USUARIO us where us.siglas= h.tecnologoturno) tecnologoturno,(select US.siglas from USUARIO us where us.codigousuario = c.usuario) enfermera,c.tipo_aplicacion_contraste,
            case when dc.Al_InsumId is not null then  (	select i.Al_InsumId from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=dc.Al_InsumId) else '-1' end  Al_InsumId, case when dc.id_insumo is null then  (	select i.Nombre from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=dc.Al_InsumId) else (select i.producto from Insumo_Cum_Sunasa i where i.idInsumo=dc.id_insumo)  end producto, dc.cantidad, case when dc.isaplicado = '1' then 'SI' else 'NO' end aplicado, dc.lote 
            from Contraste c  inner join Detalle_Contraste dc on c.idcontraste = dc.id_contraste inner join EXAMENXATENCION e on c.numeroexamen = e.codigo inner join ATENCION a on e.numeroatencion = a.numeroatencion inner join PACIENTE p on e.codigopaciente = p.codigopaciente inner join ESTUDIO es on e.codigoestudio=es.codigoestudio inner join SUCURSAL s on SUBSTRING(e.codigoestudio,3,1) = s.codigosucursal and s.codigounidad = '1' 
            inner join HONORARIOTECNOLOGO h on e.codigo = h.codigohonorariotecnologo
            where convert(Date,a.fechayhora) between '{0}' and '{1}' and s.codigosucursal like '{2}' and (case when dc.Al_InsumId is not null then  (	select i.Al_InsumId from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=dc.Al_InsumId) else '-1' end) like '{3}' order by a.fechayhora, s.ShortDesc, e.codigo asc";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, fec_ini.ToShortDateString(), fec_fin.ToShortDateString(), sede.ToString(), producto.ToString()), connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new Reporte_Contraste();
                        string fechaF = Convert.ToDateTime(reader["fechayhora"]).ToString("dd/MM/yyyy");



                        //item.fecha = fechaF.Day + "/" + fechaF.Month + "/" + fechaF.Year;
                        item.fechaB = fechaF;
                        item.sede = (reader["ShortDesc"]).ToString();
                        item.codigo = Convert.ToInt32(reader["codigo"]);
                        item.paciente = (reader["paciente"]).ToString();
                        item.edad = Convert.ToInt32(reader["edad"]);
                        item.peso = Convert.ToInt32(reader["peso"]);
                        item.nombreestudio = (reader["nombreestudio"]).ToString();
                        item.estadoestudio = (reader["estadoestudio"]).ToString();
                        item.equipo = (reader["equipoAsignado"]).ToString();
                        item.tecnologo = (reader["tecnologoturno"]).ToString();
                        item.enfermera = (reader["enfermera"]).ToString();
                        item.tipo_aplicacion = (reader["tipo_aplicacion_contraste"]).ToString();
                        item.producto = (reader["producto"]).ToString();
                        item.cantidad = Convert.ToInt32(reader["cantidad"]);
                        item.aplicado = (reader["aplicado"]).ToString();
                        item.lote = (reader["lote"]).ToString();
                        item.id_insumo = Convert.ToInt32(reader["Al_InsumId"]);
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }

                List<Resumen_Reporte_Contraste> lstresumen = new List<Resumen_Reporte_Contraste>();

                foreach (var item in lista.Select(x => new { x.id_insumo, x.producto }).Distinct())
                {
                    double total = 0, volumen = 0;


                    string sqlResumen = @"
			select count(*) total,i.Volumen from AL_Stock_Consumo so inner join AL_Stock_Item si on so.AL_Stock_ConsumoID = si.AL_Stock_ConsumoID inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId  where si.Al_InsumId = '{0}' and convert(date,so.FecSalida) between '{1}' and '{2}' group by i.Volumen";
                    SqlCommand command_resumen = new SqlCommand(string.Format(sqlResumen, item.id_insumo, fec_ini.ToShortDateString(), fec_fin.ToShortDateString()), connection);
                    connection.Open();

                    SqlDataReader reader2 = command_resumen.ExecuteReader();

                    try
                    {

                        while (reader2.Read())
                        {
                            total = Convert.ToDouble(reader2["total"].ToString());
                            volumen = Convert.ToDouble(reader2["volumen"].ToString());
                        }
                    }
                    finally
                    {
                        reader2.Close();
                        connection.Close();
                    }


                    Resumen_Reporte_Contraste det = new Resumen_Reporte_Contraste();
                    det.insumo = item.producto;
                    det.frascoSalida = total;
                    det.umedidaSalida = det.frascoSalida * volumen;
                    det.umedidaUsado = lista.Where(x => x.id_insumo == item.id_insumo).Sum(x => x.cantidad);
                    det.frascoUsado = det.umedidaUsado / volumen;
                    det.umedidaSaldo = det.umedidaSalida - det.umedidaUsado;
                    det.frascoSaldo = det.umedidaSaldo / volumen;
                    lstresumen.Add(det);
                }

                data.detalle = lista;
                data.resumen = lstresumen;

            }
            return View(data);
        }

        /**********************************************/
        public ActionResult ReporteSedacion(DateTime i, DateTime f)
        {
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);

            List<Reporte_Sedacion> lista = new List<Reporte_Sedacion>();
            DataReporteSedacion data = new DataReporteSedacion();
            string sqlPost = @"select a.fechayhora, s.fecha_fin, su.ShortDesc, ea.codigo, p.nombres + ' ' + p.apellidos paciente, a.edad, a.peso, es.nombreestudio, ea.estadoestudio, (select eq.ShortDesc from EQUIPO eq where eq.codigoequipo = (case when ea.equipoAsignado is null then ea.codigoequipo else ea.equipoAsignado end)) equipoAsignado, (select US.ShortName from USUARIO us where us.codigousuario = s.usuario) anestesiologo, s.tipo_sedacion, s.motivo_sedacion,(case when s.motivo_otros is null then ' ' else s.motivo_otros end) motivo_otros,  
case when ds.Al_InsumId is not null then  (	select i.Al_InsumId from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=ds.Al_InsumId) else '-1' end  Al_InsumId, case when ds.id_insumo is null then  (	select i.Nombre from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=ds.Al_InsumId) else (select i.producto from Insumo_Cum_Sunasa i where i.idInsumo=ds.id_insumo)  end producto, ds.cantidad, case when ds.isaplicado = '1' then 'SI' else 'NO' end aplicado, ds.lote
from Sedacion s inner join Detalle_Sedacion ds on s.idsedacion = ds.id_sedacion inner join EXAMENXATENCION ea on s.numeroexamen = ea.codigo inner join ATENCION a on ea.numeroatencion = a.numeroatencion inner join PACIENTE p on ea.codigopaciente = p.codigopaciente inner join ESTUDIO es on ea.codigoestudio = es.codigoestudio inner join SUCURSAL su on SUBSTRING(ea.codigoestudio,3,1) = su.codigosucursal and su.codigounidad = '1' left join AL_Stock_Item si on si.AL_Stock_ItemID = ds.Al_InsumId left join AL_Insum ai on si.AL_InsumId = ai.Al_InsumId where convert(Date,a.fechayhora) between '{0}' and '{1}' and ai.IN_SubClaseId in (3,4) and s.estado=1 order by a.fechayhora, su.ShortDesc, ea.codigo asc";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, fec_ini.ToShortDateString(), fec_fin.ToShortDateString()), connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new Reporte_Sedacion();

                        item.fecha = Convert.ToDateTime(reader["fechayhora"]);
                        item.fec_aplica = Convert.ToDateTime(reader["fecha_fin"]);
                        item.sede = (reader["ShortDesc"]).ToString();
                        item.codigo = Convert.ToInt32(reader["codigo"]);
                        item.paciente = (reader["paciente"]).ToString();
                        item.edad = Convert.ToInt32(reader["edad"]);
                        item.peso = Convert.ToInt32(reader["peso"]);
                        item.nombreestudio = (reader["nombreestudio"]).ToString();
                        item.estadoestudio = (reader["estadoestudio"]).ToString();
                        item.equipo = (reader["equipoAsignado"]).ToString();
                        item.anestecia = (reader["anestesiologo"]).ToString();
                        item.tipo_seda = (reader["tipo_sedacion"]).ToString();
                        item.motivo_seda = (reader["motivo_sedacion"]).ToString();
                        item.otros_seda = (reader["motivo_otros"]).ToString();
                        item.producto = (reader["producto"]).ToString();
                        item.cantidad = Convert.ToInt32(reader["cantidad"]);
                        item.aplicado = (reader["aplicado"]).ToString();
                        item.lote = (reader["lote"]).ToString();
                        item.id_insumo = Convert.ToInt32(reader["Al_InsumId"]);
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }

                List<Resumen_Reporte_Sedacion> lstresumen = new List<Resumen_Reporte_Sedacion>();

                foreach (var item in lista.Select(x => new { x.id_insumo, x.producto }).Distinct())
                {
                    double total = 0, volumen = 0;


                    string sqlResumen = @"
			select count(*) total,i.Volumen from AL_Stock_Consumo so inner join AL_Stock_Item si on so.AL_Stock_ConsumoID = si.AL_Stock_ConsumoID inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId  where si.Al_InsumId = '{0}' and convert(date,so.FecSalida) between '{1}' and '{2}' group by i.Volumen";
                    SqlCommand command_resumen = new SqlCommand(string.Format(sqlResumen, item.id_insumo, fec_ini.ToShortDateString(), fec_fin.ToShortDateString()), connection);
                    connection.Open();

                    SqlDataReader reader2 = command_resumen.ExecuteReader();

                    try
                    {

                        while (reader2.Read())
                        {
                            total = Convert.ToDouble(reader2["total"].ToString());
                            volumen = Convert.ToDouble(reader2["volumen"].ToString());
                        }
                    }
                    finally
                    {
                        reader2.Close();
                        connection.Close();
                    }


                    Resumen_Reporte_Sedacion det = new Resumen_Reporte_Sedacion();
                    det.insumo = item.producto;
                    det.frascoSalida = total;
                    det.umedidaSalida = det.frascoSalida * volumen;
                    det.umedidaUsado = lista.Where(x => x.id_insumo == item.id_insumo).Sum(x => x.cantidad);
                    det.frascoUsado = det.umedidaUsado / volumen;
                    det.umedidaSaldo = det.umedidaSalida - det.umedidaUsado;
                    det.frascoSaldo = det.umedidaSaldo / volumen;
                    lstresumen.Add(det);
                }

                data.detalle = lista;
                data.resumen = lstresumen;

            }
            return View(data);
        }


        public ActionResult ReporteEventosImg(DateTime i, DateTime f)
        {
            DateTime fec_ini = new DateTime(i.Year, i.Month, i.Day, 0, 0, 1);
            DateTime fec_fin = new DateTime(f.Year, f.Month, f.Day, 23, 59, 59);

            List<Reporte_Sedacion> lista = new List<Reporte_Sedacion>();
            DataReporteSedacion data = new DataReporteSedacion();
            string sqlPost = @"select a.fechayhora, s.fecha_fin, su.ShortDesc, ea.codigo, p.nombres + ' ' + p.apellidos paciente, a.edad, a.peso, es.nombreestudio, ea.estadoestudio, (select eq.ShortDesc from EQUIPO eq where eq.codigoequipo = (case when ea.equipoAsignado is null then ea.codigoequipo else ea.equipoAsignado end)) equipoAsignado, (select US.ShortName from USUARIO us where us.codigousuario = s.usuario) anestesiologo, s.tipo_sedacion, s.motivo_sedacion,(case when s.motivo_otros is null then ' ' else s.motivo_otros end) motivo_otros,  
case when ds.Al_InsumId is not null then  (	select i.Al_InsumId from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=ds.Al_InsumId) else '-1' end  Al_InsumId, case when ds.id_insumo is null then  (	select i.Nombre from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=ds.Al_InsumId) else (select i.producto from Insumo_Cum_Sunasa i where i.idInsumo=ds.id_insumo)  end producto, ds.cantidad, case when ds.isaplicado = '1' then 'SI' else 'NO' end aplicado, ds.lote
from Sedacion s inner join Detalle_Sedacion ds on s.idsedacion = ds.id_sedacion inner join EXAMENXATENCION ea on s.numeroexamen = ea.codigo inner join ATENCION a on ea.numeroatencion = a.numeroatencion inner join PACIENTE p on ea.codigopaciente = p.codigopaciente inner join ESTUDIO es on ea.codigoestudio = es.codigoestudio inner join SUCURSAL su on SUBSTRING(ea.codigoestudio,3,1) = su.codigosucursal and su.codigounidad = '1' left join AL_Stock_Item si on si.AL_Stock_ItemID = ds.Al_InsumId left join AL_Insum ai on si.AL_InsumId = ai.Al_InsumId where convert(Date,a.fechayhora) between '{0}' and '{1}' and ai.IN_SubClaseId in (3,4) order by a.fechayhora, su.ShortDesc, ea.codigo asc";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, fec_ini.ToShortDateString(), fec_fin.ToShortDateString()), connection);
                connection.Open();
                SqlDataReader reader = command_contraste.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new Reporte_Sedacion();

                        item.fecha = Convert.ToDateTime(reader["fechayhora"]);
                        item.fec_aplica = Convert.ToDateTime(reader["fecha_fin"]);
                        item.sede = (reader["ShortDesc"]).ToString();
                        item.codigo = Convert.ToInt32(reader["codigo"]);
                        item.paciente = (reader["paciente"]).ToString();
                        item.edad = Convert.ToInt32(reader["edad"]);
                        item.peso = Convert.ToInt32(reader["peso"]);
                        item.nombreestudio = (reader["nombreestudio"]).ToString();
                        item.estadoestudio = (reader["estadoestudio"]).ToString();
                        item.equipo = (reader["equipoAsignado"]).ToString();
                        item.anestecia = (reader["anestesiologo"]).ToString();
                        item.tipo_seda = (reader["tipo_sedacion"]).ToString();
                        item.motivo_seda = (reader["motivo_sedacion"]).ToString();
                        item.otros_seda = (reader["motivo_otros"]).ToString();
                        item.producto = (reader["producto"]).ToString();
                        item.cantidad = Convert.ToInt32(reader["cantidad"]);
                        item.aplicado = (reader["aplicado"]).ToString();
                        item.lote = (reader["lote"]).ToString();
                        item.id_insumo = Convert.ToInt32(reader["Al_InsumId"]);
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }

                List<Resumen_Reporte_Sedacion> lstresumen = new List<Resumen_Reporte_Sedacion>();

                foreach (var item in lista.Select(x => new { x.id_insumo, x.producto }).Distinct())
                {
                    double total = 0, volumen = 0;


                    string sqlResumen = @"
			select count(*) total,i.Volumen from AL_Stock_Consumo so inner join AL_Stock_Item si on so.AL_Stock_ConsumoID = si.AL_Stock_ConsumoID inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId  where si.Al_InsumId = '{0}' and convert(date,so.FecSalida) between '{1}' and '{2}' group by i.Volumen";
                    SqlCommand command_resumen = new SqlCommand(string.Format(sqlResumen, item.id_insumo, fec_ini.ToShortDateString(), fec_fin.ToShortDateString()), connection);
                    connection.Open();

                    SqlDataReader reader2 = command_resumen.ExecuteReader();

                    try
                    {

                        while (reader2.Read())
                        {
                            total = Convert.ToDouble(reader2["total"].ToString());
                            volumen = Convert.ToDouble(reader2["volumen"].ToString());
                        }
                    }
                    finally
                    {
                        reader2.Close();
                        connection.Close();
                    }


                    Resumen_Reporte_Sedacion det = new Resumen_Reporte_Sedacion();
                    det.insumo = item.producto;
                    det.frascoSalida = total;
                    det.umedidaSalida = det.frascoSalida * volumen;
                    det.umedidaUsado = lista.Where(x => x.id_insumo == item.id_insumo).Sum(x => x.cantidad);
                    det.frascoUsado = det.umedidaUsado / volumen;
                    det.umedidaSaldo = det.umedidaSalida - det.umedidaUsado;
                    det.frascoSaldo = det.umedidaSaldo / volumen;
                    lstresumen.Add(det);
                }

                data.detalle = lista;
                data.resumen = lstresumen;

            }
            return View(data);
        }

        //lista de equipos asignados por sucursal
        public JsonResult getsedes(string tipo)
        {

            var query = from s in db.SUCURSAL
                        where s.codigounidad == 1
                        select new { s.codigosucursal, s.descripcion };

            return Json(query, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getproductos()
        {
            var query = from a in db.AL_Insum
                        where a.IN_SubClaseId != 3
                        select new { a.Al_InsumId, a.Nombre };

            return Json(query, JsonRequestBehavior.AllowGet);

        }

        class lstenfermera
        {
            public string cod_usuario { get; set; }

            public string nombre { get; set; }
        }

        public JsonResult getenfermeras()
        {

            var query = from a in db.SegUsuarioXRol
                        join u in db.USUARIO on a.codigousuario equals u.codigousuario
                        where a.SegRolId == 7
                        orderby u.ShortName
                        select new { a.codigousuario, u.ShortName };

            //var query = @"select se.codigousuario as 'Enf.Codigo', u.ShortName as 'Enf.Nombre' from SegUsuarioXRol se inner join  USUARIO u on se.codigousuario = u.codigousuario where SegRolId = 7 order by u.ShortName for JSON path";

            return Json(query, JsonRequestBehavior.AllowGet);

        }

        public JsonResult gettecnologos()
        {

            var query = from a in db.EMPLEADO
                        join u in db.USUARIO on a.dni equals u.dni
                        where a.codigocargo == 7
                        orderby u.ShortName
                        select new { u.codigousuario, u.ShortName };

            //var query = @"select se.codigousuario as 'Enf.Codigo', u.ShortName as 'Enf.Nombre' from SegUsuarioXRol se inner join  USUARIO u on se.codigousuario = u.codigousuario where SegRolId = 7 order by u.ShortName for JSON path";

            return Json(query, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getequipos()
        {

            var query = from a in db.EQUIPO
                        where
                        a.codigounidad2 == 1 &&
                        a.estado == "1"
                        orderby a.nombreequipo
                        select new { a.codigoequipo, a.nombreequipo };

            //var query = @"select se.codigousuario as 'Enf.Codigo', u.ShortName as 'Enf.Nombre' from SegUsuarioXRol se inner join  USUARIO u on se.codigousuario = u.codigousuario where SegRolId = 7 order by u.ShortName for JSON path";

            return Json(query, JsonRequestBehavior.AllowGet);

        }

    }

}

public class xxx
{
    public List<xxx_x> informante { get; set; }
    public List<xxx_x> validador { get; set; }
}

public class xxx_x
{
    public string medico { get; set; }
    public string cant { get; set; }
}