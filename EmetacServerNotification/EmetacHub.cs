using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmetacServerNotification
{

    public class EmetacHub : Hub
    {
        public static List<ClientChat> ConnectedUsers = new List<ClientChat>();
        public void NewAdmision()
        {
            Console.WriteLine("Nueva Admision " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "");
            Clients.All.NewAdmision();
        }
        public void ActualizarTecnologoEmetac()
        {
            Console.WriteLine("Actualizar Lista Tecnologo Emetac");
            Clients.All.ActualizarTecnologoEmetac();
        }
        public void ActualizarTecnologoResocentro()
        {
            Console.WriteLine("Actualizar Lista Tecnologo Resocentro");
            Clients.All.ActualizarTecnologoResocentro();
        }

        #region visor Paciente
        public void CallPaciente()
        {
            //VISOR
            Clients.All.actualizarPaciente("s");
            Console.WriteLine("Actualizo visor Ticketera: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            //COUNTER
            Clients.All.actualizarCarga("s");
            Console.WriteLine("Actualizo counter Ticketera: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        }

        public void CallPaciente2()
        {
            //VISOR           
            Clients.All.actualizarPaciente2("Actualizo");
            Console.WriteLine("Actualizo visor Ticketera: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        }
        #endregion

        public void perifoneo(string tipo, string perifoneo)
        {
            //notificaciones medico
            Clients.All.perifoneo(tipo, perifoneo);
            Console.WriteLine("Solicitud de  " + tipo + " al " + perifoneo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        }

        #region Chat
        public void Connect(string username, string codigousuario)
        {
            var user = ConnectedUsers.SingleOrDefault(x => x.codigousuario == codigousuario);
            if (user == null)
            {
                ClientChat c = new ClientChat()
                {
                    Username = username,
                    codigousuario = codigousuario,
                    ID = codigousuario
                };
                user = c;
                ConnectedUsers.Add(c);
            }
            else
            {
                user.ID = codigousuario;
            }

            Console.WriteLine("Usuario Conectado: " + username + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            Clients.All.updateUsers(codigousuario, ConnectedUsers.Select(x => new { id = x.ID, username = x.Username }).OrderBy(x=> x.username).ToArray());

        }

        public void Disconnect(string id)
        {
            var user = ConnectedUsers.SingleOrDefault(x => x.ID == id);
            if (user != null)
            {
                ConnectedUsers.Remove(user);
            }

            Console.WriteLine("Usuario Desconectado: " + user.Username + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            Clients.All.updateUsers(Context.ConnectionId, ConnectedUsers.Select(x => new { id = x.ID, username = x.Username }).ToArray());

        }
        public void Send(string to, string message, string from)
        {
            var sender = ConnectedUsers.FirstOrDefault(x => x.codigousuario.Equals(from));
            var reciver = ConnectedUsers.FirstOrDefault(x => x.ID.Equals(to));
            Clients.All.broadcastMessage(
                sender.ID,
                sender.Username,
                message,
                reciver.codigousuario);
            Console.WriteLine(from + " envio un mensaje a " + to + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        }
        #endregion

        public void notificarusuarios(string tipo, string mensaje)
        {
            Clients.All.notificarusuarios(tipo, mensaje);
            Console.WriteLine("Se envio una notificacion de tipo: " + " mensaje:  " + mensaje + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
    }
    public class ClientChat
    {
        public string Username { get; set; }
        public string codigousuario { get; set; }
        public string ID { get; set; }
    }
}
