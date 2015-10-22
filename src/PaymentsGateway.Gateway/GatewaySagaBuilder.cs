using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;

namespace PaymentsGateway.Gateway
{
    /// <summary>
    /// Builder for the Saga. Not really required when using IOC container
    /// </summary>
    public class GatewaySagaBuilder
    {
        private IDepositResponseFactory _responseFactory;
        private IClearingRequestFactory _clearingRequestFactory;

        private RequestSettings _clearingRequestSettings;

        public GatewaySagaBuilder WithResponseFactory<T>() where T : IDepositResponseFactory, new()
        {
            _responseFactory = new T();
            return this;
        }
        public GatewaySagaBuilder WithClearingRequestFactory<T>() where T : IClearingRequestFactory, new()
        {
            _clearingRequestFactory = new T();
            return this;
        }

        public GatewaySagaBuilder WithClearingRequestSettings(RequestSettings settings)
        {
            _clearingRequestSettings = settings;
            return this;
        }

        public GatewaySagaBuilder WithDefaultImplementation()
        {
            _responseFactory = new DepositResponseFactory();
            _clearingRequestFactory = new ClearingRequestFactory();
            return this;
        }

        public GatewaySaga Build()
        {
            return new GatewaySaga(_responseFactory, _clearingRequestFactory, _clearingRequestSettings);
        }
    }
}
