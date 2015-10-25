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

        //TODO: resolve components with IOC
        public GatewaySaga(IDepositResponseFactory responseFactory, IClearingRequestFactory clearingRequestFactory, RequestSettings clearingRequestSettings)
        {
            _responseFactory = responseFactory;
            _clearingRequestFactory = clearingRequestFactory;

            InstanceState(x => x.CurrentState);

            Event(() => DepositRequested, x => x.CorrelateBy(request => request.DepositRequest.CorellationKey, context => context.Message.CorellationKey)
                                              .SelectId(context => NewId.NextGuid()));

            Request(() => AuthorizationFlow, state => state.ClearingRequestId, clearingRequestSettings);
            Request(() => SettlementFlow, state => state.ClearingRequestId, clearingRequestSettings);


            Initially(When(DepositRequested)
                .Then(context =>
                {
                    context.Instance.TransactionId = context.Instance.CorrelationId;
                    context.Instance.DepositRequest = context.Data.Copy();
                })
                .Respond(context => _responseFactory.FromPendingRequest(context.Instance.TransactionId.GetValueOrDefault(), context.Instance.DepositRequest))
                .TransitionTo(AuthorizationFlow.Pending)
                .Request(AuthorizationFlow,
                    context => _clearingRequestFactory.FromDepositRequest(context.Instance.TransactionId.GetValueOrDefault(), context.Data)));

            During(AuthorizationFlow.Pending,
                When(AuthorizationFlow.Completed, filter => filter.Data.ClearingStatus == ClearingStatus.Authorized)
                    .Then(context => new CustomerBalance().Credit(context.Instance.DepositRequest, context.Data,
                        response => context.Instance.Response = response))
                        .TransitionTo(SettlementFlow.Pending)
                        .Request(SettlementFlow, context => _clearingRequestFactory.FromAuthorizationResponse(context.Data)),

                When(AuthorizationFlow.Completed, filter => filter.Data.ClearingStatus == ClearingStatus.Rejected)
                    .Then(context => context.Instance.Response = _responseFactory.FromClearingResponse(context.Instance.DepositRequest, context.Data))
                    .Finalize(),

                When(AuthorizationFlow.Faulted)
                    .Then(context => context.Instance.Response = _responseFactory.FromClearingFault(context.Data)).Finalize(),

                When(AuthorizationFlow.TimeoutExpired)
                    .Then(context => context.Instance.Response = _responseFactory.FromClearingTimeout(context.Instance.TransactionId.GetValueOrDefault(),
                            context.Instance.DepositRequest, context.Data)).Finalize());

            During(SettlementFlow.Pending,
               When(SettlementFlow.Completed)
                       .Then(context => context.Instance.Response = _responseFactory.FromClearingResponse(context.Instance.DepositRequest, context.Data))
                       .Finalize(),

               When(SettlementFlow.Faulted)
                   .Then(context => context.Instance.Response = _responseFactory.FromClearingFault(context.Data)).Finalize(),

               When(SettlementFlow.TimeoutExpired)
                   .Then(context => context.Instance.Response = _responseFactory.FromClearingTimeout(context.Instance.TransactionId.GetValueOrDefault(),
                           context.Instance.DepositRequest, context.Data)).Finalize());

            Finally(context => context.Publish(x => x.Instance.Response));

            SetCompletedWhenFinalized();
            //TODO: Example of compensation
        }

        public Request<GatewaySagaState, AuthorizationRequest, AuthorizationResponse> AuthorizationFlow { get; private set; }
        public Request<GatewaySagaState, SettlementRequest, SettlementResponse> SettlementFlow { get; private set; }

        public Event<CcDepositRequest> DepositRequested { get; private set; }
    }
}