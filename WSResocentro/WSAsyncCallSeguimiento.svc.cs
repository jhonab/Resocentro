using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
namespace WSResocentro
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class WSAsyncCallSeguimiento : IWSAsyncCallSeguimiento
    {
        public bool ActualizarListaEncuesta()
        {
            try
            {
                HubConnection Connection_Hub = new HubConnection("http://extranet.resocentro.com:5052/");
                IHubProxy Proxy = Connection_Hub.CreateHubProxy("EmetacHub");
                Connection_Hub.Start().Wait();
                Proxy.Invoke("NewAdmision").Wait();
                
                return true;
            }
            catch (Exception )
            {
                
                return false;
            }
           
        }
    }
}
