using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace PaymentsGateway.WebUI.Providers
{
    public class AccountNumberUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.QueryString["AccountNumber"];
        }
    }
}