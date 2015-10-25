using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Automatonymous;
using PaymentsGateway.Gateway;

namespace GatewayService
{
    internal class StateMachineObserver : EventObserver<GatewaySagaState>, StateObserver<GatewaySagaState>
    {
        public Task PreExecute(EventContext<GatewaySagaState> context)
        {
            Debug.WriteLine($"PRE:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId}");
            return Task.CompletedTask;
        }

        public Task PreExecute<T>(EventContext<GatewaySagaState, T> context)
        {
            Debug.WriteLine($"PRE:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId},{context.Instance.ClearingRequestId}");
            return Task.CompletedTask;
        }

        public Task PostExecute(EventContext<GatewaySagaState> context)
        {
            Debug.WriteLine($"POST:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId},{context.Instance.ClearingRequestId}");
            return Task.CompletedTask;
        }

        public Task PostExecute<T>(EventContext<GatewaySagaState, T> context)
        {
            Debug.WriteLine($"POST:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId},{context.Instance.ClearingRequestId}");
            return Task.CompletedTask;
        }

        public Task ExecuteFault(EventContext<GatewaySagaState> context, Exception exception)
        {
            Debug.WriteLine($"FAULT:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId},{context.Instance.ClearingRequestId}");
            return Task.CompletedTask;
        }

        public Task ExecuteFault<T>(EventContext<GatewaySagaState, T> context, Exception exception)
        {
            Debug.WriteLine($"FAULT:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId},{context.Instance.ClearingRequestId}");
            return Task.CompletedTask;
        }

        public Task StateChanged(InstanceContext<GatewaySagaState> context, State currentState, State previousState)
        {
            Debug.WriteLine($"State Changed: CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId},{context.Instance.ClearingRequestId}, current={currentState.Name}, previous={previousState?.Name}");
            return Task.CompletedTask;
        }
    }
}