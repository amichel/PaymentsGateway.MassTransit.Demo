using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MassTransit;
using Microsoft.AspNet.SignalR.Infrastructure;
using PaymentsGateway.Contracts;
using PaymentsGateway.WebUI.Hubs;

namespace PaymentsGateway.WebUI.Consumers
{
    public class PaymentResponseConsumer : IConsumer<CcDepositResponse>
    {
        private readonly IConnectionManager _connectionManager;

        public PaymentResponseConsumer(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public Task Consume(ConsumeContext<CcDepositResponse> context)
        {
            _connectionManager.GetHubContext<PaymentsHub>().Clients.User(context.Message.AccountNumber.ToString()).onDepositResponse(context.Message);
            return Task.CompletedTask;
        }
    }
}