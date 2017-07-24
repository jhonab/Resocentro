
using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;

namespace EmetacServerNotification
{
    class Program
    {
        static void Main(string[] args)
        {
            //string url = "http://localhost:8089";
            //string url = "http://serverweb:5052";
            string url = "http://extranet.resocentro.com:5052";
            Console.Title = "Servidor: " + url;
            using (WebApp.Start(url))
            {
                Console.WriteLine("Servidor Ejecuntando en {0}", url);
                while (true)
                {
                    string key = Console.ReadLine();
                    if (key.ToUpper() == "C")
                    {
                        break;
                    }
                }

                Console.ReadLine();
            }
        }
    }
}
