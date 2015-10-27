using System;
using System.Configuration;
using Automatonymous;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;
using PaymentsGateway.Clearing;
using PaymentsGateway.Contracts;
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
#if DEBUG
            var observer = new StateMachineObserver();
            _machine.ConnectEventObserver(observer);
            _machine.ConnectStateObserver(observer);
#endif
            _repository = new Lazy<ISagaRepository<ClearingSagaState>>(() => new InMemorySagaRepository<ClearingSagaState>());
        }
        public void Start()
        {
            ConfigureSaga();
            ConfigureServiceBus();

            //var client = _busControl.CreateRequestClient<AuthorizationRequest, AuthorizationResponse>(new Uri("rabbitmq://rabbit.local/payments/clearing"), new TimeSpan(0, 0, 1, 0));
            //var response = await client.Request(new AuthorizationRequest() { AccountNumber = 111, Amount = 100, CardToken = "ZZZ", CardType = CardType.MasterCard, Currency = "EUR", TransactionId = NewId.NextGuid() });
        }

        public void Stop()
        {
            _busHandle?.Stop();
        }
    }
}
