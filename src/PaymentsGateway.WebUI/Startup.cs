using Microsoft.Owin;
using Owin;
using PaymentsGateway.WebUI;

[assembly: OwinStartup(typeof (Startup))]

namespace PaymentsGateway.WebUI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}