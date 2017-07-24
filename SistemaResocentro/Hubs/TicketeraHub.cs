using Microsoft.AspNet.SignalR;
using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SistemaResocentro.Hubs
{
    public class TicketeraHub : Hub
    {
        public void CallPaciente()
        {
            //VISOR
            Clients.All.actualizarPaciente("s");
            //COUNTER
            Clients.All.actualizarCarga("s");

        }

        public void CallPaciente2()
        {
            //VISOR           
            Clients.All.actualizarPaciente2("Actualizo");

        }


        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
    }
}