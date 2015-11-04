using System;
using Topshelf;

namespace GatewayService
{
    static class Program
    {
        static int Main()
        {
            var exitCode = (int)HostFactory.Run(x => x.Service<PaymentsGateway.GatewayService.GatewayService>().StartAutomatically());
            Console.WriteLine($"{nameof(PaymentsGateway.GatewayService.GatewayService)} Started. Exit Code = {exitCode}");
            return exitCode;
        }
    }
}
