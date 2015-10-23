using System;
using System.Configuration;
using Automatonymous;
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
        ClearingSaga _machine;
        Lazy<ISagaRepository<ClearingSagaState>> _repository;

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
                    e.StateMachineSaga(_machine, _repository.Value);
                });
            });

            _busHandle = _busControl.Start();
        }

        private void ConfigureSaga()
        {
            _machine = new ClearingSaga(new ClearingApiAdaptor());

            _repository = new Lazy<ISagaRepository<ClearingSagaState>>(() => new InMemorySagaRepository<ClearingSagaState>());
        }
        public void Start()
        {
            ConfigureSaga();
            ConfigureServiceBus();
        }

        public void Stop()
        {
            _busHandle?.Stop();
        }
    }
}
