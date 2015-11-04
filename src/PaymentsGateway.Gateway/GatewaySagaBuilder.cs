using Automatonymous;
using PaymentsGateway.Gateway.Components;

namespace PaymentsGateway.Gateway
{
    /// <summary>
    ///     Builder for the Saga. Not really required when using IOC container
    /// </summary>
    public class GatewaySagaBuilder
    {
        private IAccountingLogicFacade _accountingLogic;
        private IClearingRequestFactory _clearingRequestFactory;
        private RequestSettings _clearingRequestSettings;
        private IDepositResponseFactory _responseFactory;

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

        public GatewaySagaBuilder WithAccountingLogic<T>() where T : IAccountingLogicFacade, new()
        {
            _accountingLogic = new T();
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
            _accountingLogic = new AccountingLogicFacade();
            return this;
        }

        public GatewaySaga Build()
        {
            return new GatewaySaga(_responseFactory, _clearingRequestFactory, _accountingLogic, _clearingRequestSettings);
        }
    }
}