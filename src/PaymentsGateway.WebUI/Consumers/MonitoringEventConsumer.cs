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
    public class MonitoringEventConsumer : IConsumer<SagaMonitoringEvent>
    {
        private readonly IConnectionManager _connectionManager;

        public MonitoringEventConsumer(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public Task Consume(ConsumeContext<SagaMonitoringEvent> context)
        {
            var log = context.Message.ToLogString();
            var hostInfo = $"Machine={context.Host.MachineName},ProcessId={context.Host.ProcessId},ProcessName={context.Host.ProcessName},OS={context.Host.OperatingSystemVersion}";
            _connectionManager.GetHubContext<PaymentsHub>().Clients.All.onMonitoringEvent($"{log},{hostInfo}");
            return Task.CompletedTask;
        }
    }
}