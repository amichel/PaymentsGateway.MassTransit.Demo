using System;
using System.Threading.Tasks;
using Automatonymous;
using MassTransit;
using MassTransit.Saga;
using MassTransit.TestFramework;
using MassTransit.Testing;
using NSubstitute;
using NUnit.Framework;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Clearing.Tests
{
    [TestFixture]
    public class On_AuthorizationRequest_Specs:InMemoryTestFixture
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            _clearingApi.ClearReceivedCalls();
            _client = new MessageRequestClient<AuthorizationRequest, AuthorizationResponse>(Bus, InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _clearingApi = Substitute.For<IClearingApi>();
            _machine = new ClearingSaga(_clearingApi);
            _repository = new InMemorySagaRepository<ClearingSagaState>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        ClearingSaga _machine;
        InMemorySagaRepository<ClearingSagaState> _repository;
        IRequestClient<AuthorizationRequest, AuthorizationResponse> _client;
        private IClearingApi _clearingApi;


        [Test]
        public async Task Should_receive_the_response_message()
        {
            _clearingApi.ProcessRequest(Arg.Any<AuthorizationRequest>()).Returns(new AuthorizationResponse());
            AuthorizationResponse complete = await _client.Request(new AuthorizationRequest(), TestCancellationToken);
        }

        [Test]
        public async Task Calls_clearingApi()
        {
            var sagaId = Guid.NewGuid();
            _clearingApi.ProcessRequest(Arg.Any<AuthorizationRequest>()).Returns(new AuthorizationResponse());
            var authorizationRequest = new AuthorizationRequest { TransactionId = sagaId };
            await _client.Request(authorizationRequest);

            var saga =
                await
                    _repository.ShouldContainSaga(x => x.CorrelationId == sagaId, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            _clearingApi.Received(1).ProcessRequest(Arg.Any<AuthorizationRequest>());
        }

        [Test]
        public async Task Moves_to_Authorizing_state()
        {
            var sagaId = Guid.NewGuid();
            _clearingApi.ProcessRequest(Arg.Any<AuthorizationRequest>()).Returns(new AuthorizationResponse());
            var authorizationRequest = new AuthorizationRequest { TransactionId = sagaId };
            await _client.Request(authorizationRequest);

            var saga =
                await
                    _repository.ShouldContainSaga(x => x.CorrelationId == sagaId && Equals(x.CurrentState, _machine.Authorizing), TestTimeout);
            Assert.IsTrue(saga.HasValue);

        }
        [Test]
        public async Task Rejects_payment_If_clearingApi_responds_with_Reject()
        {
            var sagaId = Guid.NewGuid();
            _clearingApi.ProcessRequest(Arg.Any<AuthorizationRequest>()).Returns(new AuthorizationResponse() { ClearingStatus = ClearingStatus.Rejected }
                );
            var authorizationRequest = new AuthorizationRequest { TransactionId = sagaId };
            await _client.Request(authorizationRequest);

            var saga =
                await
                    _repository.ShouldContainSaga(x => x.CorrelationId == sagaId && Equals(x.CurrentState, _machine.Authorizing), TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }
    }
}