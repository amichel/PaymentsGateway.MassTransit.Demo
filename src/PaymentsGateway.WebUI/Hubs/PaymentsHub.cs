using System;
using System.Configuration;
using System.Threading.Tasks;
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
                var client =
                    _bus.CreateRequestClient<CcDepositRequest, CcDepositResponse>(
                        new Uri(ConfigurationManager.AppSettings["GatewayServiceAddress"]), new TimeSpan(0, 0, 0, 30));
                return await client.Request(request);
            }
            catch (RequestTimeoutException)
            {
                return new CcDepositResponse {AccountNumber = request.AccountNumber, Status = DepositStatus.Timedout};
            }
        }
    }
}