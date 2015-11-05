using System;
using System.Threading.Tasks;
using Automatonymous;
using MassTransit;
using MassTransit.Saga;
using MassTransit.TestFramework;
using NSubstitute;
using NUnit.Framework;
using PaymentsGateway.Contracts;
using PaymentsGateway.Gateway.Components;

namespace PaymentsGateway.Clearing.Tests
{
    [TestFixture]
    public class On_AuthorizationRequest_Specs : InMemoryTestFixture
    {
        [SetUp]
        public void Setup()
        {
            _clearingApi.ClearReceivedCalls();
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _clearingApi = Substitute.For<IClearingApi>();
            _machine = new ClearingSaga(_clearingApi);
            _repository = new InMemorySagaRepository<ClearingSagaState>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        private ClearingSaga _machine;
        private InMemorySagaRepository<ClearingSagaState> _repository;
        private IClearingApi _clearingApi;


        [Test]
        public async Task Calls_clearingApi()
        {
            var sagaId = Guid.NewGuid();
            _clearingApi.ProcessRequest(Arg.Any<AuthorizationRequest>()).Returns(new AuthorizationResponse());
            var req = await Bus.Request(InputQueueAddress, new AuthorizationRequest {TransactionId = sagaId}, x =>
            {
                x.Handle<AuthorizationResponse>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);
            await req.Task;

            var saga = await _repository.ShouldContainSaga(x => x.CorrelationId == sagaId, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            _clearingApi.Received().ProcessRequest(Arg.Any<AuthorizationRequest>());
        }

        [Test]
        public async Task Moves_to_Authorizing_state()
        {
            var sagaId = Guid.NewGuid();
            _clearingApi.ProcessRequest(Arg.Any<AuthorizationRequest>()).Returns(new AuthorizationResponse());
            var authorizationRequest = new AuthorizationRequest {TransactionId = sagaId};
            var req = await Bus.Request(InputQueueAddress, authorizationRequest, x =>
            {
                x.Handle<AuthorizationResponse>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);

            await req.Task;
            var saga =
                await
                    _repository.ShouldContainSaga(x => x.CorrelationId == sagaId && Equals(x.CurrentState, _machine.Authorizing),
                        TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        [Test]
        public async Task Rejects_payment_If_clearingApi_responds_with_Reject()
        {
            var sagaId = Guid.NewGuid();
            _clearingApi.ProcessRequest(Arg.Any<AuthorizationRequest>())
                .Returns(new AuthorizationResponse {ClearingStatus = ClearingStatus.Rejected});
            var authorizationRequest = new AuthorizationRequest {TransactionId = sagaId};

            Task<AuthorizationResponse> response = null;
            var req = await Bus.Request(InputQueueAddress, authorizationRequest, x =>
            {
                response = x.Handle<AuthorizationResponse>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);

            await req.Task;

            Assert.That((await response).ClearingStatus, Is.EqualTo(ClearingStatus.Rejected));
        }
    }

    [TestFixture]
    public class On_SettlementRequest_Specs : InMemoryTestFixture
    {
        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _clearingApi = Substitute.For<IClearingApi>();
            _machine = new ClearingSaga(_clearingApi);
            _repository = new InMemorySagaRepository<ClearingSagaState>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        private ClearingSaga _machine;
        private InMemorySagaRepository<ClearingSagaState> _repository;
        private IClearingApi _clearingApi;
        private Guid transactionId;

        private void ClearingApiAuthorizesSettlement()
        {
            _clearingApi.ProcessRequest(Arg.Any<SettlementRequest>())
                .Returns(new SettlementResponse
                    {
                        TransactionId = transactionId,
                        AccountNumber = 1234,
                        ProviderTransactionId = $"{Guid.NewGuid()}",
                        ClearingStatus = ClearingStatus.Authorized
                    });
        }

        private void ClearingApiRejectsSettlement()
        {
            _clearingApi.ProcessRequest(Arg.Any<SettlementRequest>())
                .Returns(new SettlementResponse
                    {
                        TransactionId = transactionId,
                        AccountNumber = 1234,
                        ProviderTransactionId = $"{Guid.NewGuid()}",
                        ClearingStatus = ClearingStatus.Rejected
                    });
        }

        private void ClearingApiAcceptsAuthorizationRequest()
        {
            transactionId = Guid.NewGuid();
            _clearingApi.ProcessRequest(Arg.Any<AuthorizationRequest>())
                .Returns(new AuthorizationResponse
                    {
                        TransactionId = transactionId,
                        AccountNumber = 1234,
                        ProviderTransactionId = $"{Guid.NewGuid()}"
                    });
        }

        [Test]
        public async Task Accepts_payment_If_settlement_request_received()
        {
            ClearingApiAcceptsAuthorizationRequest();
            ClearingApiAuthorizesSettlement();

            Task<AuthorizationResponse> authorizationResponseTask = null;
            await Bus.Request(InputQueueAddress, new AuthorizationRequest {TransactionId = transactionId}, x =>
            {
                authorizationResponseTask = x.Handle<AuthorizationResponse>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);
            var authorizationResp = await authorizationResponseTask;

            Task<SettlementResponse> settlementRespT = null;
            await Bus.Request(InputQueueAddress, new ClearingRequestFactory().FromAuthorizationResponse(authorizationResp), x =>
            {
                settlementRespT = x.Handle<SettlementResponse>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);

            var settlementResp = await settlementRespT;

            Assert.That(settlementResp.ClearingStatus, Is.EqualTo(ClearingStatus.Authorized));
        }

        [Test]
        public async Task Rejects_payment_If_clearingApi_responds_with_Reject()
        {
            ClearingApiAcceptsAuthorizationRequest();
            ClearingApiRejectsSettlement();

            Task<AuthorizationResponse> authorizationResponseTask = null;
            await Bus.Request(InputQueueAddress, new AuthorizationRequest {TransactionId = transactionId}, x =>
            {
                authorizationResponseTask = x.Handle<AuthorizationResponse>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);
            var authorizationResp = await authorizationResponseTask;

            Task<SettlementResponse> settlementRespT = null;
            await Bus.Request(InputQueueAddress, new ClearingRequestFactory().FromAuthorizationResponse(authorizationResp), x =>
            {
                settlementRespT = x.Handle<SettlementResponse>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);

            var settlementResp = await settlementRespT;

            Assert.That(settlementResp.ClearingStatus, Is.EqualTo(ClearingStatus.Rejected));
        }
    }
}