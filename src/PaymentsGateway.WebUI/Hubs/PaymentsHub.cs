using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace PaymentsGateway.WebUI.Hubs
{
    public class PaymentsHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}