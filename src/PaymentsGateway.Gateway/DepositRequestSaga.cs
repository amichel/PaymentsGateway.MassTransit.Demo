using System;
using System.Diagnostics;
using Automatonymous;
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
                    response => { this.CreateEventLift(DepositValidated).Raise(context.Instance, response); })
                    .ValidateAsync())
                .TransitionTo(Validating));

            During(Validating,
                When(DepositValidated, filter => filter.Data.IsValid)
                    .Request(ClearingFlow,
                        context => new ClearingRequestFactory().FromDepositRequest(context.Data.Request))
                    //TODO: resolve factory with IOC
                    .TransitionTo(ClearingFlow.Pending),
                When(DepositValidated, filter => !filter.Data.IsValid)
                    .Publish(
                        context =>
                            new DepositResponseFactory().FromFailedValidationResponse(context.Instance.TransactionId,
                                context.Data))
                    .Finalize());

            During(ClearingFlow.Pending,
                When(ClearingFlow.Completed, filter => filter.Data.ClearingStatus == ClearingStatus.Authorized)
                    .Then(context => CreditCustomerAccount(context.Instance.DepositRequest, context.Data,
                        response => { this.CreateEventLift(Funded).Raise(context.Instance, response); }))
                    .Catch<ApplicationException>(x => x.TransitionTo(Final)) //TODO: Example of compensation
                    .TransitionTo(Funding),
                When(ClearingFlow.Completed, filter => filter.Data.ClearingStatus == ClearingStatus.Rejected)
                    .Publish(
                        context =>
                            new DepositResponseFactory().FromClearingResponse(context.Instance.DepositRequest,
                                context.Data))
                    .Finalize(),
                When(ClearingFlow.Faulted)
                    .Publish(context => new DepositResponseFactory().FromClearingFault(context.Data))
                    .Finalize(),
                When(ClearingFlow.TimeoutExpired)
                    .Publish(
                        context =>
                            new DepositResponseFactory().FromClearingTimeout(context.Instance.TransactionId,
                                context.Instance.DepositRequest, context.Data))
                    .Finalize());

            During(Funding,
                When(Funded)
                    .Publish(x => x.Data)
                    .Finalize());

            //TODO: DuringAny() - give example of duringany
            //TODO: Finally(); - give example of something done on final transition
        }

        public State Validating { get; private set; }
        public State Funding { get; private set; }
        public State Completed { get; private set; }

        public Request<GatewaySagaState, ClearingRequest, ClearingResponse> ClearingFlow { get; private set; }

        public Event<CcDepositRequest> DepositRequested { get; private set; }
        public Event<DepositValidationResponse> DepositValidated { get; private set; }
        public Event<CcDepositResponse> Funded { get; private set; }

        private void CreditCustomerAccount(CcDepositRequest depositRequest, ClearingResponse response,
            Action<CcDepositResponse> onFunded)
        {
            Debug.WriteLine(
                $"Funding Customer Account. TransactionId={response.TransactionId} AccountNumber={depositRequest.AccountNumber} Amount={depositRequest.Amount} Currency={depositRequest.Currency}");
            onFunded(new DepositResponseFactory().FromClearingResponse(depositRequest, response));
        }
    }
}