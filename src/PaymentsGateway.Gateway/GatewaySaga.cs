using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Binders;
using MassTransit;
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

            Event(() => DepositRequested, x => x.CorrelateBy(request => request.DepositRequest.CorellationKey, context => context.Message.CorellationKey)
                                              .SelectId(context => NewId.NextGuid()));

            Request(() => ClearingFlow, state => state.TransactionId, clearingRequestSettings);


            Initially(When(DepositRequested)
                .Then(context =>
                {
                    context.Instance.TransactionId = context.Instance.CorrelationId;
                    context.Instance.DepositRequest = context.Data.Copy();
                })
                //.TransitionTo(Validating)
                //.Respond(context => _responseFactory.FromPendingRequest(context.Instance.TransactionId.GetValueOrDefault(), context.Instance.DepositRequest))
                .Then(context => ValidateRequest(context.Instance, context.Data))); //TODO: make async

            During(Validating,
                When(DepositValidated, filter => filter.Data.IsValid)
                    .TransitionTo(ClearingFlow.Pending)
                    .Request(ClearingFlow,
                        context => _clearingRequestFactory.FromDepositRequest(context.Instance.TransactionId.GetValueOrDefault(), context.Data.Request)),
                When(DepositValidated, filter => !filter.Data.IsValid)
                    .Then(context => { context.Instance.Response = _responseFactory.FromFailedValidationResponse(context.Instance.TransactionId.GetValueOrDefault(), context.Data); })
                    .Finalize());
            //.Then(context => this.RaiseEvent(context.Instance, Completed,
            //        _responseFactory.FromFailedValidationResponse(context.Instance.TransactionId.GetValueOrDefault(), context.Data))));

            During(ClearingFlow.Pending,
                When(ClearingFlow.Completed, filter => filter.Data.ClearingStatus == ClearingStatus.Authorized)
                    .Then(context => new CustomerBalance().Credit(context.Instance.DepositRequest, context.Data,
                        response => context.Instance.Response = response)).Finalize(),
                When(ClearingFlow.Completed, filter => filter.Data.ClearingStatus == ClearingStatus.Rejected)
                    .Then(context => context.Instance.Response = _responseFactory.FromClearingResponse(context.Instance.DepositRequest, context.Data))
                    .Finalize(),
                When(ClearingFlow.Faulted)
                    .Then(context => context.Instance.Response = _responseFactory.FromClearingFault(context.Data)).Finalize(),
                When(ClearingFlow.TimeoutExpired)
                    .Then(context => context.Instance.Response = _responseFactory.FromClearingTimeout(context.Instance.TransactionId.GetValueOrDefault(),
                            context.Instance.DepositRequest, context.Data)).Finalize());

            //DuringAny(When(Completed)
            //        .Publish(x => x.Data)
            //        .Finalize());

            Finally(context => context.Publish(x => x.Instance.Response));

            SetCompletedWhenFinalized();
            //TODO: Example of compensation
            //TODO: Finally(); - give example of something done on final transition
        }

        private void ValidateRequest(GatewaySagaState instance, CcDepositRequest request)
        {
            var result = _depositValidatorFactory.CreateCcDepositValidator(request).Validate();
            this.RaiseEvent(instance, DepositValidated, result);
        }

        public State Validating { get; private set; }
        //public State Funding { get; private set; }

        public Request<GatewaySagaState, ClearingRequest, ClearingResponse> ClearingFlow { get; private set; }

        public Event<CcDepositRequest> DepositRequested { get; private set; }
        public Event<DepositValidationResponse> DepositValidated { get; private set; }


        // public Event<CcDepositResponse> Completed { get; private set; }
    }
}