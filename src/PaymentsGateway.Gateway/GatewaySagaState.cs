using System;
using Automatonymous;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public class GatewaySagaState : SagaStateMachineInstance
    {
        public GatewaySagaState(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid? TransactionId { get; set; }
        //use Saga correllation id for transaction id. In real service of course it will be generated in DB when payment transaction is initiated.
        public CcDepositRequest DepositRequest { get; set; }
        public CcDepositResponse Response { get; set; }
        public State CurrentState { get; set; }
        public Guid CorrelationId { get; set; }
    }
}