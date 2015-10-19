using System;
using Topshelf;

namespace ClearingService
{
    class Program
    {
        static int Main(string[] args)
        {
            var exitCode = (int)HostFactory.Run(x => x.Service<ClearingService>(svc =>
            {
                svc.ConstructUsing(s => new ClearingService());
                svc.WhenStarted(s => s.Start());
                svc.WhenStopped(s => s.Stop());
            }).StartAutomatically());
            Console.WriteLine($"{nameof(ClearingService)} Started. Exit Code = {exitCode}");
            return exitCode;
        }
    }
}
