using System.Threading.Tasks;
using Automatonymous;
using MassTransit;
using MassTransit.Saga;
using MassTransit.TestFramework;
using NSubstitute;
using NUnit.Framework;
using PaymentsGateway.Contracts;
using PaymentsGateway.Gateway;
using PaymentsGateway.Gateway.Components;

namespace PaymentsGateway.GatewayService.Tests
{
    [TestFixture]
    public class On_CcDepositRequest_Specs : InMemoryTestFixture
    {
        [SetUp]
        public void Setup()
        {
            _accountingLogicFacade.ClearReceivedCalls();
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _accountingLogicFacade = Substitute.For<IAccountingLogicFacade>();
            var requestSettings = Substitute.For<RequestSettings>();
            _machine = new GatewaySaga(new DepositResponseFactory(), new ClearingRequestFactory(), _accountingLogicFacade, requestSettings);
            _repository = new InMemorySagaRepository<GatewaySagaState>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        private GatewaySaga _machine;
        private InMemorySagaRepository<GatewaySagaState> _repository;
        private IAccountingLogicFacade _accountingLogicFacade;

        private static CcDepositRequest NewCcDepositRequest()
        {
            return new CcDepositRequest {AccountNumber = 111, CardId = 100};
        }


        [Test]
        public async Task Moves_saga_to_AuthorizationFlow_Pending()
        {
            var newCcDepositRequest = NewCcDepositRequest();
            var req = await Bus.Request(InputQueueAddress, newCcDepositRequest, x =>
            {
                x.Handle<CcDepositResponse>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);
            await req.Task;

            await _repository.ShouldContainSaga(x => Equals(x.CurrentState, _machine.AuthorizationFlow.Pending), TestTimeout);
        }

        [Test]
        public async Task Repsonds_with_Pending_message()
        {
            var newCcDepositRequest = NewCcDepositRequest();
            Task<CcDepositResponse> response = null;
            var req = await Bus.Request(InputQueueAddress, newCcDepositRequest, x =>
            {
                response = x.Handle<CcDepositResponse>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);
            await req.Task;
            var r = await response;
            Assert.That(r.Status,Is.EqualTo(DepositStatus.Pending));
        }
    }
}