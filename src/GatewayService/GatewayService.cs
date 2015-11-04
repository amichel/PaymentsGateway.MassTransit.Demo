using System;
using System.Configuration;
using Automatonymous;
using GatewayService;
using MassTransit;
using MassTransit.Saga;
using PaymentsGateway.Gateway;
using Topshelf;

namespace PaymentsGateway.GatewayService
{
    public class GatewayService : ServiceControl
    {
        IBusControl _busControl;
        BusHandle _busHandle;
        GatewaySaga _machine;
        Lazy<ISagaRepository<GatewaySagaState>> _repository;

        private void ConfigureServiceBus()
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), h =>
                {
                    h.Username(ConfigurationManager.AppSettings["RabbitMQUser"]);
                    h.Password(ConfigurationManager.AppSettings["RabbitMQPassword"]);
                });

                x.ReceiveEndpoint(host, "gateway", e =>
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
            _machine = new GatewaySagaBuilder().WithDefaultImplementation()
                                                            .WithClearingRequestSettings(ServiceRequestSettings.ClearingRequestSettings())
                                                            .Build();
            _repository = new Lazy<ISagaRepository<GatewaySagaState>>(() => new InMemorySagaRepository<GatewaySagaState>());
        }

        private void ConnectObserver()
        {
            var observer = new StateMachineObserver(_busControl);
            _machine.ConnectEventObserver(observer);
            _machine.ConnectStateObserver(observer);
        }

        public bool Start(HostControl hostControl)
        {
            ConfigureSaga();
            ConfigureServiceBus();
#if DEBUG
            ConnectObserver();
#endif
            //_busControl.Publish(new CcDepositRequest() { AccountNumber = 111, Amount = 500, CardId = 2, Currency = "EUR" });

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _busHandle?.Stop();
            return true;
        }
    }
}
