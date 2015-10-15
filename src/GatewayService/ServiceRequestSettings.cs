using System;
using System.Configuration;
using Automatonymous;

namespace GatewayService
{
    internal class ServiceRequestSettings : RequestSettings
    {
        public Uri ServiceAddress { get; set; }
        public Uri SchedulingServiceAddress { get; set; }
        public TimeSpan Timeout { get; set; }

        public static ServiceRequestSettings ClearingRequestSettings()
        {
            return new ServiceRequestSettings()
            {
                SchedulingServiceAddress = new Uri(ConfigurationManager.AppSettings["SchedulingServiceAddress"]),
                ServiceAddress = new Uri(ConfigurationManager.AppSettings["ClearingServiceAddress"]),
                Timeout = new TimeSpan(0, 0, 0, int.Parse(ConfigurationManager.AppSettings["ClearingRequestTimeout"]))
            };
        }
    }
}