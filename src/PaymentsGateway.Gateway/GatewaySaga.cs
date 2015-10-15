using System;
using System.Diagnostics;
using Automatonymous;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public class GatewaySaga : MassTransitStateMachine<GatewaySagaState>
    {
        private readonly IDepositResponseFactory _responseFactory;
        private readonly IClearingRequestFactory _clearingRequestFactory;
        private readonly IDepositValidatorFactory _depositValidatorFactory;

        //TODO: resolve components with IOC
        public GatewaySaga(IDepositResponseFactory responseFactory, IClearingRequestFactory clearingRequestFactory, IDepositValidatorFactory depositValidatorFactory, RequestSettings clearingRequestSettings)
        {
            _responseFactory = responseFactory;
            _clearingRequestFactory = clearingRequestFactory;
            _depositValidatorFactory = depositValidatorFactory;

            InstanceState(x => x.CurrentState);

            Request(() => ClearingFlow, state => state.TransactionId, clearingRequestSettings);

            Event(() => DepositRequested, x => x.CorrelateBy(request => request.DepositRequest.CorellationKey, context => context.Message.CorellationKey)
                                                .SelectId(context => Guid.NewGuid()));


            Initially(When(DepositRequested)
                .Then(context => context.Instance.DepositRequest = context.Data)
                .ThenAsync(context => _depositValidatorFactory.CreateCcDepositValidator(context.Data,
                    response => { this.CreateEventLift(DepositValidated).Raise(context.Instance, response); })
                    .ValidateAsync())
                .TransitionTo(Validating));

            During(Validating,
                When(DepositValidated, filter => filter.Data.IsValid)
                    .Request(ClearingFlow,
                        context => _clearingRequestFactory.FromDepositRequest(context.Data.Request))
                    .TransitionTo(ClearingFlow.Pending),
                When(DepositValidated, filter => !filter.Data.IsValid)
                    .Then(context => this.RaiseEvent(context.Instance, Completed,
                            _responseFactory.FromFailedValidationResponse(context.Instance.TransactionId, context.Data))));

            During(ClearingFlow.Pending,
                When(ClearingFlow.Completed, filter => filter.Data.ClearingStatus == ClearingStatus.Authorized)
                    .Then(context => new CustomerBalance().Credit(context.Instance.DepositRequest, context.Data,
                        response => { this.CreateEventLift(Completed).Raise(context.Instance, response); }))
                    .TransitionTo(Funding),
                When(ClearingFlow.Completed, filter => filter.Data.ClearingStatus == ClearingStatus.Rejected)
                    .Then(context => this.RaiseEvent(context.Instance, Completed,
                       _responseFactory.FromClearingResponse(context.Instance.DepositRequest,
                            context.Data))),
                When(ClearingFlow.Faulted)
                    .Then(context => _responseFactory.FromClearingFault(context.Data)),
                When(ClearingFlow.TimeoutExpired)
                    .Then(context =>
                       _responseFactory.FromClearingTimeout(context.Instance.TransactionId,
                            context.Instance.DepositRequest, context.Data)));

            DuringAny(When(Completed)
                    .Publish(x => x.Data)
                    .Finalize());

            SetCompletedWhenFinalized();
            //TODO: Example of compensation
            //TODO: Finally(); - give example of something done on final transition
        }

        public State Validating { get; private set; }
        public State Funding { get; private set; }
        public Request<GatewaySagaState, ClearingRequest, ClearingResponse> ClearingFlow { get; private set; }

        public Event<CcDepositRequest> DepositRequested { get; private set; }
        public Event<DepositValidationResponse> DepositValidated { get; private set; }
        public Event<CcDepositResponse> Completed { get; private set; }
    }
}