using System;
using System.Configuration;
using System.Reflection;
using Autofac;
using MassTransit;
using Module = Autofac.Module;

namespace PaymentsGateway.WebUI.IOC
{
    public class ServiceBusModule : Module
    {
        private readonly Assembly[] _assembliesToScan;

        public ServiceBusModule(params Assembly[] assembliesToScan)
        {
            _assembliesToScan = assembliesToScan;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Registers all consumers with our container
            builder.RegisterConsumers(_assembliesToScan).AsSelf();

            // Creates our bus from the factory and registers it as a singleton against two interfaces
            builder.Register(c => Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), h =>
                {
                    h.Username(ConfigurationManager.AppSettings["RabbitMQUser"]);
                    h.Password(ConfigurationManager.AppSettings["RabbitMQPassword"]);
                });

                x.ReceiveEndpoint(host, $"WebUi-{Guid.NewGuid()}", e =>
                {
                    e.AutoDelete = true;
                    e.Exclusive = true;
                    e.LoadFrom(c.Resolve<ILifetimeScope>()); //subscribe consumers
                });
            })).As<IBusControl>().As<IBus>().SingleInstance();
        }
    }
}