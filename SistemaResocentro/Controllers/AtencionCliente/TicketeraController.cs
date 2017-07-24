using SistemaResocentro.Hubs;
using SistemaResocentro.Member;
using SistemaResocentro.Models;
using SistemaResocentro.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaResocentro.Controllers
{

    public class TicketeraController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        //
        // GET: /Ticketera/
        [Authorize]
        public ActionResult CambioEstadoSession(int idcounter, bool estado)
        {
            bool result = true;
            string msj = "";
            var counter = db.TKT_Counter.Where(x => x.idcounter == idcounter).SingleOrDefault();
            if (counter != null)
            {
                TKT_Historial historial = new TKT_Historial();
                historial.idcounter = idcounter;
                if (estado)
                    historial.tipo = (int)Tipo_Historial.OnLine;
                else
                    historial.tipo = (int)Tipo_Historial.OffLine;
                historial.fecha = DateTime.Now;
                db.TKT_Historial.Add(historial);
                counter.isActivo = estado;
                db.SaveChanges();
                result = estado;
            }
            else
                result = false;

            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }
        [Authorize]
        public ActionResult VerificarEstadoSession(int idcounter)
        {
            bool result = true;
            string msj = "";
            var counter = db.TKT_Counter.Where(x => x.idcounter == idcounter).SingleOrDefault();
            result = counter.isActivo;
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        [Authorize]
        public ActionResult Counter()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            TKT_Counter _counter = db.TKT_Counter.Where(x => x.nombre == cod_usu).SingleOrDefault();

            return View(_counter);
        }


        //lista de examenes pendiestes
        [Authorize]
        public ActionResult CambioEstado(int idticket, int estado)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            bool result = true;
            string msj = "";
            var ticket = db.TKT_Ticketera.Where(x => x.id_Ticket == idticket).SingleOrDefault();
            if (ticket != null)
            {
                if (estado == (int)Estado_Ticket.Llamada && ticket.estado == (int)Estado_Ticket.Llamada)
                {
                    ticket.isRellamada = true;
                }
                //guardando fechas
                else if (estado == (int)Estado_Ticket.Llamada)
                {
                    ticket.fec_llamada = DateTime.Now;
                    ticket.usu_llamada = user.ProviderUserKey.ToString();
                    ticket.isRellamada = true;
                }
                else if (estado == (int)Estado_Ticket.Cancelado)
                {
                    ticket.fec_cancela = DateTime.Now;
                    ticket.usu_cancela = user.ProviderUserKey.ToString();
                }
                else if (estado == (int)Estado_Ticket.Terminado)
                {
                    ticket.fec_termino = DateTime.Now;
                    ticket.usu_termino = user.ProviderUserKey.ToString();
                }
                else { }
                ticket.estado = estado;
                db.SaveChanges();
                result = true;
            }


            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }


        //lista de examenes pendiestes
        [Authorize]
        public ActionResult GetListaTicket(JQueryDataTableParamModel param)
        {
            var allCompanies = getExamenes();
            var result = from c in allCompanies
                         select new[] { 
                             Convert.ToString("<cite class='strong'>"+c.nombre.ToUpper()+"</cite>"),
                             Convert.ToString(c.tipo.ToLower()),
                             Convert.ToString("<strong style='color:red;'>"+c.minutos+"  </strong><br/><strong style='color:green;'>"+c.min_aten+" </strong>"),
                             "",//botones
                             Convert.ToString(c.id_tiket),
                         Convert.ToString(c.estado),
                         Convert.ToString(c.isPreferencial),
                         Convert.ToString(c.isRecojo),
                         Convert.ToString(c.isAtendiendo),
                         Convert.ToString(c.isEmergencia),};

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = allCompanies.Count(),
                iTotalDisplayRecords = allCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }

        //metodo que obtiene la data
        [Authorize]
        private IList<List_TicketeraViewModels> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            var counter = db.TKT_Counter.Where(x => x.nombre == cod_usu).SingleOrDefault();

            string sql = "select t.id_Ticket,t.nombre,t.id_tipo_aten,ta.nombre tipo,CONVERT(VARCHAR(8), GETDATE() - CAST(CONVERT(VARCHAR(8), GETDATE(), 112) + ' ' + CONVERT(VARCHAR(8), t.fec_registro, 108) AS DATETIME), 108) min_esp ,t.estado,t.isPreferencial,t.isrecojo,t.isEmergencia,isnull(CONVERT(VARCHAR(8), GETDATE() - CAST(CONVERT(VARCHAR(8), GETDATE(), 112) + ' ' + CONVERT(VARCHAR(8), t.fec_llamada, 108) AS DATETIME), 108),'') min_ate from TKT_Ticketera t inner join TKT_Tipo_Atencion ta on t.id_tipo_aten=ta.id_tipo_aten where t.idempresa=" + counter.idempresa + " and t.idSede=" + counter.idSede + " and t.estado<" + ((int)Estado_Ticket.Cancelado).ToString() + "and t.id_counter=" + counter.idcounter + " order by t.estado desc,t.isPreferencial desc,t.fec_registro";
            List<List_TicketeraViewModels> allCompanies = new List<List_TicketeraViewModels>();

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        List_TicketeraViewModels item = new List_TicketeraViewModels();
                        item.id_tiket = Convert.ToInt32(reader["id_Ticket"].ToString());
                        item.nombre = (reader["nombre"].ToString());
                        item.tipo = (reader["tipo"].ToString());
                        item.minutos = (reader["min_esp"].ToString());
                        item.estado = Convert.ToInt32(reader["estado"].ToString());
                        item.isPreferencial = Convert.ToBoolean(reader["isPreferencial"].ToString());
                        item.isRecojo = Convert.ToBoolean(reader["isrecojo"].ToString());
                        item.isAtendiendo = item.estado == (int)Estado_Ticket.Llamada ? true : false;
                        item.isEmergencia = Convert.ToBoolean(reader["isEmergencia"].ToString());
                        item.min_aten = (reader["min_ate"].ToString());
                        allCompanies.Add(item);
                    }

                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Close();
                }
            }

            /*
             * IList<List_TicketeraViewModels> allCompanies = (from x in db.TKT_Ticketera
                                                                        where x.isEliminado == false
                                                                        && x.estado < (int)Estado_Ticket.Cancelado
                                                                        && x.id_counter == counter.idcounter
                                                                        && x.idSede == counter.idSede
                                                                        && x.idempresa == counter.idempresa
                                                                        orderby x.estado descending, x.isPreferencial, x.fec_registro ascending
                                                                        select new List_TicketeraViewModels
                                                                        {
                                                                            id_tiket = x.id_Ticket,
                                                                            nombre = x.nombre,
                                                                            tipo = x.TKT_Tipo_Atencion.nombre,
                                                                            minutos = SqlFunctions.DateDiff("month", x.fec_registro, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", x.fec_registro, SqlFunctions.GetDate())) + " mes(es)") :
                                                SqlFunctions.DateDiff("day", x.fec_registro, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", x.fec_registro, SqlFunctions.GetDate())) + " día(s)") : SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", x.fec_registro, SqlFunctions.GetDate())),
                                                                            estado = x.estado,
                                                                            isPreferencial = x.isPreferencial,
                                                                            isRecojo = x.isrecojo,
                                                                            isAtendiendo = x.estado == (int)Estado_Ticket.Llamada ? true : false,
                                                                            isEmergencia = x.isEmergencia,
                                                                            min_aten = x.fec_llamada != null ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", x.fec_llamada, SqlFunctions.GetDate()))) : "0"

                                                                        }
                                                                      ).ToList();
                        */

            return allCompanies;
        }


        //Visor de Pacientes

        public ActionResult VisorPacientes()
        {
            return View();
        }
        //Visor de Pacientes2
        public ActionResult VisorPacientes2()
        {
            return View();
        }

        //Visor de Pacientes
        public ActionResult AsyncVisorPaciente(int sede, int empresa)
        {
            bool result = true;
            string preferencial = "", resocentro = "", emetac = "";
            string msj = "";
            /*string planitlla = "    <div class='col-md-3' style='font-size:medium;'>" +
"       <div class='panel panel-primary'>" +
"            <div class='panel-heading'>" +
"                <h1 class='panel-title'>{0}</h1>" +
"            </div>" +
"            <div class='panesl-body'>" +
"                <div class='row'>" +
"                    <div class='col-lg-12'>" +
"                        <div class='table-responsive'>" +
"                            <table class='table table-striped table-bordered table-hover'>" +
"                                <thead>" +
"                                    <tr>" +
"                                        <th>Paciente</th>" +
"                                </thead>" +
"                                <tbody>" +
"                                   {1}" +
"                                <tbody>" +
"                            </table>" +
"                        </div>" +
"                    </div>" +
"                </div>" +
"            </div>" +
"        </div>" +
"   c";*/
            string planitlla = @"    
<div class='col-md-4' style='font-size:medium;'>
<div class='panel panel-primary' style='margin-bottom:10px;'>
    <div class='panel-heading'>
        <h1 class='panel-title' style='font-size: 20px;'>{0}</h1>
    </div>
    <div class='triangulo_inf'></div>
</div>
<table class='table  table-bordered '>
    <tbody> 
        {1}
    <tbody>
</table>
</div>
";

            List<List_PacienteViewModels> lista = new List<List_PacienteViewModels>();
            List<TKT_Ticketera> lst = new List<TKT_Ticketera>();
            /*(from x in db.TKT_Ticketera
                                         where x.isEliminado == false
                                         && x.estado < (int)Estado_Ticket.Cancelado
                                         && x.idSede == sede
                                         && x.idempresa == empresa
                                         orderby x.estado descending, x.fec_registro ascending
                                         select x);*/
            string sql = "select t.id_Ticket,t.nombre,t.id_tipo_aten,t.estado,t.fec_registro,t.id_counter,'' as 'counter',t.isRellamada,t.isPreferencial,t.isEmergencia  from TKT_Ticketera t  where t.idempresa=" + empresa.ToString() + " and t.idSede=" + sede.ToString() + "  and t.isEliminado=0 and t.estado<" + ((int)Estado_Ticket.Cancelado).ToString() + " order by t.estado desc,t.isPreferencial desc,t.fec_registro; update TKT_Ticketera set isRellamada=0   where idempresa=" + empresa.ToString() + " and idSede=" + sede.ToString() + " and isRellamada=1;";
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        TKT_Ticketera item = new TKT_Ticketera();
                        item.id_Ticket = Convert.ToInt32(reader["id_Ticket"].ToString());
                        item.nombre = (reader["nombre"].ToString());
                        item.id_tipo_aten = Convert.ToInt32(reader["id_tipo_aten"].ToString());
                        item.estado = Convert.ToInt32(reader["estado"].ToString());
                        item.isRellamada = Convert.ToBoolean(reader["isRellamada"].ToString());
                        item.isPreferencial = Convert.ToBoolean(reader["isPreferencial"].ToString());
                        item.isEmergencia = Convert.ToBoolean(reader["isEmergencia"].ToString());
                        item.fec_registro = Convert.ToDateTime(reader["fec_registro"].ToString());
                        item.TKT_Counter = new TKT_Counter();
                        //item.id_counter = Convert.ToInt32(reader["id_counter"].ToString());
                        item.TKT_Counter.nombre = (reader["counter"].ToString());
                        lst.Add(item);
                    }

                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Close();
                }
            }

           /* string items = "";
            int count = 0;
            foreach (var counter in lst.Select(x => x.id_counter).Distinct().AsParallel())
            {
                string nombre_counter = "";
                foreach (var item in lst.Where(x => x.id_counter == counter).ToList().AsParallel())
                {
                    string nombre = "";
                    string[] nom = item.nombre.Split(' ');
                    if (nom.Count() >= 2)
                        nombre = nom[0] + ".";
                    else
                        nombre = nom[0];
                    if (nombre.Length > 20)
                        nombre = nombre.Substring(0, 16) + ".";

                    if (item.estado == (int)Estado_Ticket.Llamada)
                        items += "<tr class='odd'><td class='bg-verde  estado_" + item.estado + " " + (item.isRellamada ? "resaltar" : "") + "'><span>" + nombre + "</span><i class='fa fa-users  pull-right' style='color:#7FD193;margin-top: 5px;'></i></td></tr>";
                    else if (count == 0)
                    {
                        items += "<tr class='odd'><td class='bg-celeste  estado_" + item.estado + "'><span>" + nombre + "</span><i class='fa fa-user  pull-right' style='color:#81DBFF;margin-top: 5px;'></i></td></tr>";
                        count++;
                    }
                    else if (count == 1)
                        items += "<tr class='odd'><td class='bg-celeste  estado_" + item.estado + "'><span>" + nombre + "</span><i class='fa fa-user  pull-right' style='color:#81DBFF;margin-top: 5px;'></i></td></tr>";
                    else if (count == 2)
                        items += "<tr class='odd'><td class='bg-plomo  estado_" + item.estado + "'><span>" + nombre + "</span><i class='fa fa-clock-o  pull-right' style='color:#C1C2C4;margin-top: 5px;'></i></td></tr>";
                    else
                        items += "<tr class='odd '><td class='bg-oscuro estado_" + item.estado + "'><span>" + nombre + "</span></td></tr>";
                    count++;
                    nombre_counter = item.TKT_Counter.nombre;

                }

               
                items = "";
                count = 0;
            }*/

            int[] atencion_reso = { 1, 2 };
            int[] atencion_emetac = { 3, 4, 5, 8, 9 };
            int countpre = 1, countreso = 1, countemetac = 1;
            foreach (var item in lst)
            {
                string nombre = "";
                string[] nom = item.nombre.Split(' ');
                if (nom.Count() >= 2)
                    nombre = nom[0] + ".";
                else
                    nombre = nom[0];
                if (nombre.Length > 20)
                    nombre = nombre.Substring(0, 16) + ".";



                if (item.isPreferencial)
                {
                    preferencial += "<tr class='odd'><td class='  " + (item.estado == (int)Estado_Ticket.Llamada ? "bg-blue" : "") + "   estado_" + item.estado + " " + (item.isRellamada ? "resaltar" : "") + "'><gf class='correlativo'> " + countpre.ToString() + ". </gf><span>" + nombre + "</span>" + ((item.estado == (int)Estado_Ticket.Llamada) ? "<i class='fa fa-user  pull-right' ></i> " : "") + "</td></tr>";
                    countpre++;
                }
                else if (atencion_emetac.Contains(item.id_tipo_aten.Value))
                {
                    emetac += "<tr class='odd'><td class='" + (item.isRellamada ? "bg-green" : "") + " estado_" + item.estado + " " + (item.isRellamada ? "resaltar" : "") + "'><gf class='correlativo'> " + countemetac.ToString() + ". </gf><span>" + nombre + "</span>" + (emetac != "" ? ((item.isRellamada) ? "<i class='fa fa-user  pull-right' ></i> " : "") : "") + "</td></tr>";
                    countemetac++;
                }
                else
                {
                    resocentro += "<tr class='odd'><td class='" + (item.estado == (int)Estado_Ticket.Llamada ? "bg-celeste" : "") + " estado_" + item.estado + " " + (item.isRellamada ? "resaltar" : "") + "'><gf class='correlativo'> " + countreso.ToString() + ". </gf><span>" + nombre + "</span>" + ((item.estado == (int)Estado_Ticket.Llamada) ? "<i class='fa fa-user  pull-right' ></i> " : "") + "</td></tr>";
                    countreso++;
                }
            }
            msj += string.Format(planitlla, "PREFERENCIAL", preferencial);
            msj += string.Format(planitlla, "EMETAC", emetac);
            msj += string.Format(planitlla, "RESOCENTRO", resocentro);
            // regresaestado();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }
        public ActionResult AsyncVisorPaciente2(int sede, int empresa)
        {


            string preferencial = "", paciente = "", caja = "";

            List<List_PacienteViewModels> lista = new List<List_PacienteViewModels>();
            List<TKT_Ticketera> lst = new List<TKT_Ticketera>();

            string sql = "select t.id_Ticket,t.nombre,t.id_tipo_aten,t.estado,t.fec_registro,t.id_counter,t.isRellamada,isPreferencial from TKT_Ticketera t inner join TKT_Tipo_Atencion ta on t.id_tipo_aten=ta.id_tipo_aten where t.idempresa=" + empresa.ToString() + " and t.idSede=" + sede.ToString() + "  and t.isEliminado=0 and t.estado<" + ((int)Estado_Ticket.Cancelado).ToString() + " order by t.estado desc,t.isPreferencial desc,ta.orderby,t.fec_registro; update TKT_Ticketera set isRellamada=0 where idempresa=" + empresa.ToString() + " and idSede=" + sede.ToString() + " and isRellamada=1;";
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        TKT_Ticketera item = new TKT_Ticketera();
                        item.id_Ticket = Convert.ToInt32(reader["id_Ticket"].ToString());
                        item.nombre = (reader["nombre"].ToString());
                        item.id_tipo_aten = Convert.ToInt32(reader["id_tipo_aten"].ToString());
                        item.estado = Convert.ToInt32(reader["estado"].ToString());
                        item.isRellamada = Convert.ToBoolean(reader["isRellamada"].ToString());
                        item.fec_registro = Convert.ToDateTime(reader["fec_registro"].ToString());
                        item.isPreferencial = Convert.ToBoolean(reader["isPreferencial"].ToString());
                        lst.Add(item);
                    }

                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Close();
                }
            }


            int countpre = 1, countpaci = 1, countcaja = 1;
            foreach (var item in lst)
            {
                string nombre = "";
                string[] nom = item.nombre.Split(' ');
                if (nom.Count() >= 2)
                    nombre = nom[0] + ".";
                else
                    nombre = nom[0];
                if (nombre.Length > 20)
                    nombre = nombre.Substring(0, 16) + ".";



                if (item.estado == (int)Estado_Ticket.Caja)
                {
                    caja += "<tr class='odd'><td class='" + (item.isRellamada ? "bg-green" : "") + " estado_" + item.estado + " " + (item.isRellamada ? "resaltar" : "") + "'><gf class='correlativo'> " + countcaja.ToString() + ". </gf><span>" + nombre + "</span>" + (caja != "" ? ((item.isRellamada) ? "<i class='fa fa-user  pull-right' ></i> " : "") : "") + "</td></tr>";
                    countcaja++;
                }
                else if (item.isPreferencial)
                {
                    preferencial += "<tr class='odd'><td class='  " + (item.estado == (int)Estado_Ticket.Llamada ? "bg-blue" : "") + "   estado_" + item.estado + " " + (item.isRellamada ? "resaltar" : "") + "'><gf class='correlativo'> " + countpre.ToString() + ". </gf><span>" + nombre + "</span>" + ((item.estado == (int)Estado_Ticket.Llamada) ? "<i class='fa fa-user  pull-right' ></i> " : "") + "</td></tr>";
                    countpre++;
                }
                else
                {
                    paciente += "<tr class='odd'><td class='" + (item.estado == (int)Estado_Ticket.Llamada ? "bg-celeste" : "") + " estado_" + item.estado + " " + (item.isRellamada ? "resaltar" : "") + "'><gf class='correlativo'> " + countpaci.ToString() + ". </gf><span>" + nombre + "</span>" + ((item.estado == (int)Estado_Ticket.Llamada) ? "<i class='fa fa-user  pull-right' ></i> " : "") + "</td></tr>";
                    countpaci++;
                }
            }
            return Json(new { result = true, preferencial = preferencial, paciente = paciente, caja = caja }, JsonRequestBehavior.DenyGet);
        }
        [Authorize]
        private void regresaestado()
        {
            var lst = db.TKT_Ticketera.Where(x => x.isEliminado == false && x.estado == (int)Estado_Ticket.Rellamada).ToList().AsParallel();
            foreach (var item in lst)
            {
                item.estado = (int)Estado_Ticket.Llamada;
            }
            db.SaveChanges();
        }
        [Authorize]
        public ActionResult CounterOnline()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            var counter = db.TKT_Counter.Where(x => x.nombre == cod_usu).SingleOrDefault();
            return Json(new SelectList((from c in db.TKT_Counter
                                        join eq in db.TKT_Ticketera on c.idcounter equals eq.id_counter into eq_join
                                        from eq in eq_join.DefaultIfEmpty()
                                        where
                                        (c.isActivo == true ||
                                         eq.estado < (int)Estado_Ticket.Cancelado)
                                         && c.idSede == counter.idSede
                                         && c.idempresa == counter.idempresa
                                        group eq by new { c.idcounter, c.USUARIO.ShortName } into group_tkt
                                        select new { codigo = group_tkt.Key.idcounter, nombre = group_tkt.Key.ShortName }).AsQueryable(), "codigo", "nombre"), JsonRequestBehavior.AllowGet);
        }


        //Visor de pacientes de un counter
        public ActionResult VisorPacientexCounter(int idcounter)
        {
            bool result = false;
            string msj = "";
            string planitlla =
"                    <div class='col-lg-12'>" +
"                        <div class='table-responsive'>" +
"                            <table class='table table-striped table-bordered table-hover'>" +
"                                <thead>" +
"                                    <tr>" +
"                                        <th>Paciente</th>" +
"                                        <th>Atención</th>" +
"                                </thead>" +
"                                <tbody>" +
"                                   {1}" +
"                                <tbody>" +
"                            </table>" +
"                        </div>" +
"                    </div>";

            List<List_PacienteViewModels> lista = new List<List_PacienteViewModels>();
            IQueryable<TKT_Ticketera> lst = (from x in db.TKT_Ticketera
                                             where x.isEliminado == false
                                             && x.estado == (int)Estado_Ticket.Nuevo
                                             && x.id_counter == idcounter
                                             orderby x.estado descending, x.fec_registro ascending
                                             select x);
            string items = "";
            string counter = "";
            foreach (var item in lst.ToList().AsParallel())
            {
                if (item.estado == (int)Estado_Ticket.Nuevo)
                {
                    if (item.isPreferencial)
                        items += "<tr class='odd '><td class=' estado_" + item.estado + "'><small class='badge bg-red' ><i class='fa fa-exclamation-circle'></i></small>&nbsp;&nbsp;&nbsp;" + item.nombre.ToUpper() + "</td><td class=' '>" + item.TKT_Tipo_Atencion.nombre.ToLower() + "&nbsp;&nbsp;&nbsp;<button type='button' onclick='jalarPaciente(" + item.id_Ticket.ToString() + ")'  class='btn btn-outline btn-info btn-xs jalar_pac' ><i class='fa fa-arrow-left'></i></button></td></tr>";
                    else if (item.isrecojo)
                        items += "<tr class='odd '><td class=' estado_" + item.estado + "'><small class='badge bg-blue' ><i class='fa fa-files-o'></i></small>&nbsp;&nbsp;&nbsp; " + item.nombre.ToUpper() + "</td><td class=' '>" + item.TKT_Tipo_Atencion.nombre.ToLower() + "&nbsp;&nbsp;&nbsp;<button type='button' onclick='jalarPaciente(" + item.id_Ticket.ToString() + ")'  class='btn btn-outline btn-info btn-xs jalar_pac' ><i class='fa fa-arrow-left'></i></button></td></tr>";
                    else if (item.isrecojo)
                        items += "<tr class='odd '><td class=' estado_" + item.estado + "'><small class='badge bg-yellow' ><i class='fa fa-ambulance'></i></small>&nbsp;&nbsp;&nbsp; " + item.nombre.ToUpper() + "</td><td class=' '>" + item.TKT_Tipo_Atencion.nombre.ToLower() + "&nbsp;&nbsp;&nbsp;<button type='button' onclick='jalarPaciente(" + item.id_Ticket.ToString() + ")'  class='btn btn-outline btn-info btn-xs jalar_pac' ><i class='fa fa-arrow-left'></i></button></td></tr>";

                    else
                        items += "<tr class='odd '><td class=' estado_" + item.estado + "'>" + item.nombre.ToUpper() + "</td><td class=' '>" + item.TKT_Tipo_Atencion.nombre.ToLower() + "&nbsp;&nbsp;&nbsp;<button type='button' onclick='jalarPaciente(" + item.id_Ticket.ToString() + ")'  class='btn btn-outline btn-info btn-xs jalar_pac' ><i class='fa fa-arrow-left'></i></button></td></tr>";
                }
                counter = item.TKT_Counter.USUARIO.ShortName;
            }

            msj += string.Format(planitlla, counter, items);
            items = "";

            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //Cambiar de Counter a paciente
        public ActionResult CambiarCounter_Ticket(int idcounter, int idticket)
        {
            bool result = false;
            string msj = "";
            var _tkt = db.TKT_Ticketera.Where(x => x.id_Ticket == idticket).SingleOrDefault();
            if (_tkt != null)
            {

                TKT_Historial historial = new TKT_Historial();
                historial.idcounter = idcounter;
                historial.tipo = (int)Tipo_Historial.Jalar;
                historial.fecha = DateTime.Now;
                historial.idticketera = _tkt.id_Ticket;
                historial.counter_origen = _tkt.id_counter;
                db.TKT_Historial.Add(historial);
                //TICKET
                if (_tkt.estado == (int)Estado_Ticket.Llamada)
                {
                    _tkt.estado = 0;
                }
                _tkt.id_counter = idcounter;
                db.SaveChanges();
                result = true;
            }

            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        [Authorize]
        public ActionResult ReporteGeneral(DateTime ini, DateTime fin, int sede)
        {
            //=DateTime.Now;
            Reporte_ViewModel rep = new Reporte_ViewModel();

            TKT_Ticketera item = new TKT_Ticketera();
            string sql = "spu_ReportexFecha_Ticketera";
            List<spu_ReportexFecha_Ticketera_Result> lista = new List<spu_ReportexFecha_Ticketera_Result>();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@inicio", SqlDbType.VarChar).Value = ini.ToShortDateString();
                command.Parameters.Add("@fin", SqlDbType.VarChar).Value = fin.ToShortDateString();
                command.Parameters.Add("@empresa", SqlDbType.Int).Value = 1;
                command.Parameters.Add("@sede", SqlDbType.Int).Value = sede;

                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        spu_ReportexFecha_Ticketera_Result t = new spu_ReportexFecha_Ticketera_Result();
                        t.ShortName = (reader["ShortName"].ToString());
                        t.fec_registro = Convert.ToDateTime(reader["fec_registro"].ToString());
                        t.nombre = (reader["nombre"].ToString());
                        if (reader["fec_llamada"] != DBNull.Value)
                            t.fec_llamada = Convert.ToDateTime(reader["fec_llamada"].ToString());
                        if (reader["fec_termino"] != DBNull.Value)
                            t.fec_termino = Convert.ToDateTime(reader["fec_termino"].ToString());
                        if (reader["fec_cancela"] != DBNull.Value)
                            t.fec_cancela = Convert.ToDateTime(reader["fec_cancela"].ToString());
                        t.Tiempo_Espera = Convert.ToInt32(reader["Tiempo Espera"].ToString());
                        t.Tiempo_Atencion = Convert.ToInt32(reader["Tiempo Atencion"].ToString());
                        t.isPreferencial = Convert.ToBoolean(reader["isPreferencial"].ToString());
                        t.isrecojo = Convert.ToBoolean(reader["isrecojo"].ToString());
                        t.Tipo_Atencion = (reader["Tipo Atencion"].ToString());

                        lista.Add(t);
                    }

                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Close();
                }
            }



            rep.general = lista;
            sql = "spu_ReportePromedioxFecha_Ticketera";
            List<spu_ReportePromedioxFecha_Ticketera_Result> lista_pro = new List<spu_ReportePromedioxFecha_Ticketera_Result>();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@inicio", SqlDbType.VarChar).Value = ini.ToShortDateString();
                command.Parameters.Add("@fin", SqlDbType.VarChar).Value = fin.ToShortDateString();
                command.Parameters.Add("@empresa", SqlDbType.Int).Value = 1;
                command.Parameters.Add("@sede", SqlDbType.Int).Value = sede;

                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        spu_ReportePromedioxFecha_Ticketera_Result t = new spu_ReportePromedioxFecha_Ticketera_Result();
                        t.ShortName = (reader["ShortName"].ToString());
                        t.Total = Convert.ToInt32(reader["Total"].ToString());
                        t.Max_T__Espera = Convert.ToInt32(reader["Max T. Espera"].ToString());
                        t.C_Tiempo_Espera = Convert.ToInt32(reader["%Tiempo Espera"].ToString());
                        t.Min_T__Espera = Convert.ToInt32(reader["Min T. Espera"].ToString());
                        t.Max_T__Atencion = Convert.ToInt32(reader["Max T. Atencion"].ToString());
                        t.C_Tiempo_Atencion = Convert.ToInt32(reader["%Tiempo Atencion"].ToString());
                        t.Min_T__Atencion = Convert.ToInt32(reader["Min T. Atencion"].ToString());

                        lista_pro.Add(t);
                    }

                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Close();
                }
            }

            rep.promedio = lista_pro;

            return View(rep);
        }

        #region TIPO DE ATENCION (TKT_Tipo_Atencion)
        [Authorize]
        public ActionResult ListaAtenciones()
        {
            ViewBag.Sede = db.SUCURSAL.Where(x => x.codigounidad == 1).ToList();
            return View(db.TKT_Tipo_Atencion.ToList());
        }
        [Authorize]
        public ActionResult EliminarTipoAtencion(int id)
        {
            var tipo = db.TKT_Tipo_Atencion.SingleOrDefault(x => x.id_tipo_aten == id);
            tipo.isEliminado = !tipo.isEliminado;
            db.SaveChanges();
            return RedirectToAction("ListaAtenciones");
        }
        [Authorize]
        public ActionResult EditarTipoAtencion(int id)
        {
            ViewBag.Sede = new SelectList((db.SUCURSAL.Where(x => x.codigounidad == 1).ToList()), "codigosucursal", "ShortDesc");
            return View(db.TKT_Tipo_Atencion.SingleOrDefault(x => x.id_tipo_aten == id));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTipoAtencion(TKT_Tipo_Atencion item)
        {
            ViewBag.Sede = new SelectList((db.SUCURSAL.Where(x => x.codigounidad == 1).ToList()), "codigosucursal", "ShortDesc");
            var tipo = db.TKT_Tipo_Atencion.SingleOrDefault(x => x.id_tipo_aten == item.id_tipo_aten);
            tipo.nombre = item.nombre;
            tipo.idsede = item.idsede;
            tipo.isEliminado = item.isEliminado;
            tipo.isVisible = item.isVisible;
            tipo.isEmergencia = item.isEmergencia;
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return View(item);
            }
            return RedirectToAction("ListaAtenciones");
        }

        [Authorize]
        public ActionResult RegistrarTipoAtencion()
        {
            ViewBag.Sede = new SelectList((db.SUCURSAL.Where(x => x.codigounidad == 1).ToList()), "codigosucursal", "ShortDesc");
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarTipoAtencion(TKT_Tipo_Atencion item)
        {
            ViewBag.Sede = new SelectList((db.SUCURSAL.Where(x => x.codigounidad == 1).ToList()), "codigosucursal", "ShortDesc");
            try
            {
                db.TKT_Tipo_Atencion.Add(item);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return View(item);
            }
            return RedirectToAction("ListaAtenciones");
        }
        #endregion
        #region COUNTER (TKT_Counter)
        [Authorize]
        public ActionResult ListaCounter()
        {

            ViewBag.Sede = db.SUCURSAL.Where(x => x.codigounidad == 1).ToList();
            return View(db.TKT_Counter.Where(x => x.idempresa == 1).ToList());
        }
        [Authorize]
        public ActionResult EliminarCounter(int id)
        {
            var tipo = db.TKT_Counter.SingleOrDefault(x => x.idcounter == id);
            tipo.isEliminado = !tipo.isEliminado;
            db.SaveChanges();
            return RedirectToAction("ListaCounter");
        }
        [Authorize]
        public ActionResult EditarCounter(int id)
        {
            ViewBag.Sede = new SelectList((db.SUCURSAL.Where(x => x.codigounidad == 1).ToList()), "codigosucursal", "ShortDesc");
            return View(db.TKT_Counter.SingleOrDefault(x => x.idcounter == id));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCounter(TKT_Counter item)
        {
            ViewBag.Sede = new SelectList((db.SUCURSAL.Where(x => x.codigounidad == 1).ToList()), "codigosucursal", "ShortDesc");
            var tipo = db.TKT_Counter.SingleOrDefault(x => x.idcounter == item.idcounter);
            tipo.idSede = item.idSede;
            tipo.isEliminado = item.isEliminado;
            try
            {
                if (tipo.isActivo == false)
                    db.SaveChanges();
            }
            catch (Exception)
            {
                return View(item);
            }
            return RedirectToAction("ListaCounter");
        }

        [Authorize]
        public ActionResult RegistrarCounter()
        {
            ViewBag.Usuarios = new SelectList(db.USUARIO.Where(x => x.bloqueado == false).OrderBy(x => x.ShortName).ToList(), "codigousuario", "ShortName");
            ViewBag.Sede = new SelectList((db.SUCURSAL.Where(x => x.codigounidad == 1).ToList()), "codigosucursal", "ShortDesc");
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarCounter(TKT_Counter item)
        {
            try
            {
                if (db.TKT_Counter.SingleOrDefault(x => x.idcounter == item.idcounter) == null)
                {
                    item.isActivo = false;
                    item.idempresa = 1;
                    db.TKT_Counter.Add(item);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                ViewBag.Usuarios = new SelectList(db.USUARIO.Where(x => x.bloqueado == false).OrderBy(x => x.ShortName).ToList(), "codigousuario", "ShortName");
                ViewBag.Sede = new SelectList((db.SUCURSAL.Where(x => x.codigounidad == 1).ToList()), "codigosucursal", "ShortDesc");
                return View(item);
            }
            return RedirectToAction("ListaCounter");
        }
        #endregion
        #region COUNTER (TKT_Counter)
        [Authorize]
        public ActionResult AtencionCounter(int counter)
        {
            ViewBag.Atenciones = db.TKT_List_Atencion.Where(x => x.idCounter == counter).ToList();
            ViewBag.Counter = db.TKT_Counter.Where(x => x.idcounter == counter).SingleOrDefault();
            ViewBag.LltAten = new SelectList((from eq in db.TKT_Tipo_Atencion
                                              join su in db.SUCURSAL on new { sede = eq.idsede, unidad = eq.idempresa } equals new { sede = su.codigosucursal, unidad = su.codigounidad }
                                              where eq.idempresa == 1
                                              select new
                                              {
                                                  codigo = eq.idsede,
                                                  nombre = su.ShortDesc
                                              }).Distinct().AsQueryable(), "codigo", "nombre");
            ViewBag.Vacio = new SelectList(new Variable().getVacio(), "codigo", "nombre");
            return View();
        }
        public ActionResult addAtencion(int counter, int atencion)
        {
            try
            {
                TKT_List_Atencion p = new TKT_List_Atencion();
                p.idCounter = counter;
                p.idtipo_atencion = atencion;
                db.TKT_List_Atencion.Add(p);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult deleteAtencion(int atencion)
        {
            try
            {
                var obj = db.TKT_List_Atencion.SingleOrDefault(x => x.idList_Atencion == atencion);
                if (obj != null)
                {
                    db.TKT_List_Atencion.Remove(obj);
                    db.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult getAtencionesxCounter(int counter)
        {
            return Json(db.TKT_List_Atencion.Where(x => x.idCounter == counter).Select(x => new { nombre = x.TKT_Tipo_Atencion.nombre, id = x.idList_Atencion }).ToArray(), JsonRequestBehavior.AllowGet);

        }

        #endregion

        public ActionResult getAtenciones(int sede)
        {
            return Json(new SelectList(db.TKT_Tipo_Atencion.Where(x => x.idsede == sede).OrderBy(x => x.nombre).ToList(), "id_tipo_aten", "nombre"), JsonRequestBehavior.AllowGet);

        }
    }
}
