using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace GatewayService
{
    class Program
    {
        static int Main(string[] args)
        {
            var exitCode = (int)HostFactory.Run(x => x.Service<GatewayService>().StartAutomatically());
            Console.WriteLine($"{nameof(GatewayService)} Started. Exit Code = {exitCode}");
            return exitCode;
        }
    }
}
