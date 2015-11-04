using System.Reflection;
using Autofac;
using Autofac.Integration.SignalR;
using MassTransit;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Owin;
using PaymentsGateway.WebUI.IOC;
using PaymentsGateway.WebUI.Providers;

namespace PaymentsGateway.WebUI
{
    public class IocConfig
    {
        private static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.Register(c => new AccountNumberUserIdProvider()).AsImplementedInterfaces();
            builder.RegisterModule(new ServiceBusModule(Assembly.GetExecutingAssembly()));
            return builder.Build();
        }

        public static void ConfigureSignalR(IAppBuilder app)
        {
            // Call our IoC static helper method to start the typical Autofac SignalR setup
            var container = RegisterDependencies();

            // Get your HubConfiguration. In OWIN, we create one rather than using GlobalHost
            // Sets the dependency resolver to be autofac.
            var hubConfig = new HubConfiguration {Resolver = new AutofacDependencyResolver(container)};

            // OWIN SIGNALR SETUP:
            // Register the Autofac middleware FIRST, then the standard SignalR middleware.
            app.UseAutofacMiddleware(container);
            app.MapSignalR("/signalr", hubConfig);

            var builder = new ContainerBuilder();
            var connManager = hubConfig.Resolver.Resolve<IConnectionManager>();
            builder.RegisterInstance(connManager).As<IConnectionManager>().SingleInstance();
            builder.Update(container);

            // Starts the bus.
            container.Resolve<IBusControl>().Start();
        }
    }
}