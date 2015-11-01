using System;
using System.Threading.Tasks;
using Automatonymous;
using MassTransit;
using MassTransit.Saga;
using MassTransit.TestFramework;
using NSubstitute;
using NUnit.Framework;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Clearing.Tests
{
    [TestFixture]
    public class Clearing_orchestration : InMemoryTestFixture
    {
        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(machine, repository);
        }

        private readonly ClearingSaga machine;
        private readonly InMemorySagaRepository<ClearingSagaState> repository;
        private readonly IClearingApi clearingApi;

        public Clearing_orchestration()
        {
            clearingApi = Substitute.For<IClearingApi>();
            machine = new ClearingSaga(clearingApi);
            repository = new InMemorySagaRepository<ClearingSagaState>();
        }

        [Test]
        public async Task Calls_clearingApi()
        {
            var sagaId = Guid.NewGuid();
            clearingApi.ProcessRequest(Arg.Any<AuthorizationRequest>()).Returns(new AuthorizationResponse());
            var authorizationRequest = new AuthorizationRequest {TransactionId = sagaId};
            await Bus.Publish(authorizationRequest);

            var saga =
                await
                    repository.ShouldContainSaga(x => x.CorrelationId == sagaId && Equals(x.CurrentState, machine.Authorizing), TestTimeout);
            Assert.IsTrue(saga.HasValue);

            clearingApi.Received(1).ProcessRequest(Arg.Any<AuthorizationRequest>());
        }
    }
}