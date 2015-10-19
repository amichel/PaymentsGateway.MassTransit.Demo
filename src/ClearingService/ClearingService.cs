using System;
using System.Configuration;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;
using PaymentsGateway.Clearing;
using Topshelf;

namespace ClearingService
{
    public class ClearingService
    {
        IBusControl _busControl;
        BusHandle _busHandle;

        private void ConfigureServiceBus()
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                IRabbitMqHost host = x.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), h =>
                {
                    h.Username(ConfigurationManager.AppSettings["RabbitMQUser"]);
                    h.Password(ConfigurationManager.AppSettings["RabbitMQPassword"]);
                });

                x.ReceiveEndpoint(host, "clearing", e =>
                {
                    e.Durable = true;
                    e.PrefetchCount = (ushort)Environment.ProcessorCount;
                    e.Consumer<ClearingRequestConsumer>();
                });
            });

            _busHandle = _busControl.Start();
        }

        public void Start()
        {
            ConfigureServiceBus();
        }

        public void Stop()
        {
            _busHandle?.Stop();
        }
    }
}
