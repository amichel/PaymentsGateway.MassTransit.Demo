using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Events;
using Automatonymous.Lifts;
using MassTransit;
using MassTransit.Saga;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public class DepositRequestSaga : MassTransitStateMachine<GatewaySagaState>
    {
        public DepositRequestSaga()
        {
            InstanceState(x => x.CurrentState);

            //TODO: Declare Events, Requests, States, Schedules ...

            Initially(When(DepositRequested)
                .Then(context => context.Instance.DepositRequest = context.Data)
                .ThenAsync(context => new DepositValidator(context.Data, //TODO: resolve validator with IOC
                (response) => { this.CreateEventLift(DepositValidated).Raise(context.Instance, response); })
                .ValidateAsync())
                .TransitionTo(Validating));

            During(Validating,
                When(DepositValidated, filter => filter.Data.IsValid)
                .Request(ClearingFlow, context => new ClearingRequestFactory().FromDepositRequest(context.Data.Request)) //TODO: resolve factory with IOC
                .TransitionTo(ClearingFlow.Pending),

                  When(DepositValidated, filter => !filter.Data.IsValid)
                .Publish(context => new DepositResponseFactory().FromValidationResponse(context.Data))
                .Finalize());


            During(ClearingFlow.Pending,
                When(ClearingFlow.Completed, filter => filter.Data.IsAuthorized)
                    .TransitionTo(Funding),
                When(ClearingFlow.Completed, filter => !filter.Data.IsAuthorized)
                    .Publish(context => new DepositResponseFactory().FromClearingResponse(context.Data))
                    .Finalize(),
                When(ClearingFlow.Faulted)
                .Publish(context => new DepositResponseFactory().FromClearingFault(context.Data))
                .Finalize(),
                 When(ClearingFlow.TimeoutExpired)
                .Publish(context => new DepositResponseFactory().FromClearingTimeout(context.Data))
                .Finalize());


            //When(Cleared).Request(ClearingDeposit,context=> new ClearingRequestFactory().FromDepositRequest(context.)).TransitionTo(ClearingDeposit.)

        }

        public State Validating { get; private set; }
        public State Funding { get; private set; }
        public State Completed { get; private set; }

        public Request<GatewaySagaState, ClearingRequest, ClearingResponse> ClearingFlow { get; private set; }


        public Event<CcDepositRequest> DepositRequested { get; private set; }
        public Event<DepositValidationResponse> DepositValidated { get; private set; }
        public Event<CcDepositResponse> Funded { get; private set; }
    }

    public class DepositValidator
    {
        private readonly CcDepositRequest _request;
        private readonly Action<DepositValidationResponse> _onValidated;

        public DepositValidator(CcDepositRequest request, Action<DepositValidationResponse> onValidated)
        {
            _request = request;
            _onValidated = onValidated;
        }


        public void Validate()
        {

        }

        public Task ValidateAsync()
        {

        }
    }

    public class ClearingRequestFactory
    {
        public ClearingRequest FromDepositRequest(CcDepositRequest request)
        {
            return new ClearingRequest()
            {
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                CardToken = "ABC123", //TODO: Call Tokenization Service to get token for saved card id
                CardType = CardType.MasterCard, //TODO: Get card type from tokenization service
                Currency = "EUR" //TODO: Get account currency from customers cache
            };
        }
    }

    public class DepositResponseFactory
    {
        public CcDepositResponse FromValidationResponse(DepositValidationResponse validationResponse)
        {
            throw new NotImplementedException();
        }

        public CcDepositResponse FromClearingResponse(ClearingResponse data)
        {
            throw new NotImplementedException();
        }

        public CcDepositResponse FromClearingFault(Fault<ClearingRequest> data)
        {
            throw new NotImplementedException();
        }

        public CcDepositResponse FromClearingTimeout(RequestTimeoutExpired data)
        {
            throw new NotImplementedException();
        }
    }

    public class GatewaySagaState : SagaStateMachineInstance
    {
        public GatewaySagaState(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public CcDepositRequest DepositRequest { get; set; }
        public Guid CorrelationId { get; set; }
        public State CurrentState { get; set; }
    }
}
