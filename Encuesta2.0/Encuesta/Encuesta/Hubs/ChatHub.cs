using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Encuesta.Hubs
{
    public class ChatHub : Hub
    {
        public static List<ClientChat> ConnectedUsers = new List<ClientChat>();
        public void Connect(string username, string codigousuario)
        {
            var user = ConnectedUsers.SingleOrDefault(x => x.codigousuario == codigousuario);
            if(user==null){
            ClientChat c = new ClientChat()
            {
                Username = username,
                codigousuario=codigousuario,
                ID = Context.ConnectionId
            };
                user=c;
            ConnectedUsers.Add(c);
            }
            else
            {
                user.ID = Context.ConnectionId;
            }
            Clients.All.updateUsers(Context.ConnectionId, ConnectedUsers.Select(x => new { id = x.ID, username = x.Username }).ToArray());
            
        }
        public void Send(string to ,string message,string from)
        {
            var sender = ConnectedUsers.FirstOrDefault(x => x.codigousuario.Equals(from));
            var reciver = ConnectedUsers.FirstOrDefault(x => x.ID.Equals(to));
            Clients.All.broadcastMessage(
                sender.ID,
                sender.Username,
                message,
                reciver.codigousuario);
        }

       

        public override Task OnDisconnected(bool stopCalled)
        {
            var disconnectedUser = ConnectedUsers.FirstOrDefault(x => x.ID.Equals(Context.ConnectionId));
            ConnectedUsers.Remove(disconnectedUser);

            Clients.All.updateUsers(Context.ConnectionId,ConnectedUsers.Select(x => new { id = x.ID, username = x.Username }).ToArray());
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