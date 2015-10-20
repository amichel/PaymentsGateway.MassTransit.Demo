using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MassTransit;
using Microsoft.AspNet.SignalR;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.WebUI.Hubs
{
    public class PaymentsHub : Hub
    {
        private readonly IBus _bus;

        public PaymentsHub(IBus bus)
        {
            _bus = bus;
        }

        public async Task<CcDepositResponse> CcDeposit(CcDepositRequest request)
        {
            try
            {
                var client = _bus.CreateRequestClient<CcDepositRequest, CcDepositResponse>(new Uri(ConfigurationManager.AppSettings["GatewayServiceAddress"]), new TimeSpan(0, 0, 0, 5));
                return await client.Request(request);
            }
            catch (RequestTimeoutException)
            {
                //TODO: log
                return new CcDepositResponse() { AccountNumber = request.AccountNumber, Status = DepositStatus.Timedout };
            }
        }
    }
}