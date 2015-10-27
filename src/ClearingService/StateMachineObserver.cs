using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Automatonymous;
using PaymentsGateway.Clearing;

namespace ClearingService
{
    internal class StateMachineObserver : EventObserver<ClearingSagaState>, StateObserver<ClearingSagaState>
    {
        public Task PreExecute(EventContext<ClearingSagaState> context)
        {
            Debug.WriteLine($"PRE:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId}");
            return Task.CompletedTask;
        }

        public Task PreExecute<T>(EventContext<ClearingSagaState, T> context)
        {
            Debug.WriteLine($"PRE:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId}");
            return Task.CompletedTask;
        }

        public Task PostExecute(EventContext<ClearingSagaState> context)
        {
            Debug.WriteLine($"POST:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId}");
            return Task.CompletedTask;
        }

        public Task PostExecute<T>(EventContext<ClearingSagaState, T> context)
        {
            Debug.WriteLine($"POST:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId}");
            return Task.CompletedTask;
        }

        public Task ExecuteFault(EventContext<ClearingSagaState> context, Exception exception)
        {
            Debug.WriteLine($"FAULT:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId}");
            return Task.CompletedTask;
        }

        public Task ExecuteFault<T>(EventContext<ClearingSagaState, T> context, Exception exception)
        {
            Debug.WriteLine($"FAULT:Event={context.Event.Name},CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId}");
            return Task.CompletedTask;
        }

        public Task StateChanged(InstanceContext<ClearingSagaState> context, State currentState, State previousState)
        {
            Debug.WriteLine($"State Changed: CorrelationId={context.Instance.CorrelationId},{context.Instance.TransactionId}, current={currentState.Name}, previous={previousState?.Name}");
            return Task.CompletedTask;
        }
    }
}