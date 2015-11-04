using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Automatonymous;
using MassTransit;
using PaymentsGateway.Clearing;
using PaymentsGateway.Contracts;

namespace ClearingService
{
    internal class StateMachineObserver : EventObserver<ClearingSagaState>, StateObserver<ClearingSagaState>
    {
        private readonly IBus _bus;

        public StateMachineObserver(IBus bus)
        {
            _bus = bus;
        }

        public Task PreExecute(EventContext<ClearingSagaState> context)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.PreExecute));
            return Task.CompletedTask;
        }

        public Task PreExecute<T>(EventContext<ClearingSagaState, T> context)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.PreExecute));
            return Task.CompletedTask;
        }

        public Task PostExecute(EventContext<ClearingSagaState> context)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.PostExecute));
            return Task.CompletedTask;
        }

        public Task PostExecute<T>(EventContext<ClearingSagaState, T> context)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.PostExecute));
            return Task.CompletedTask;
        }

        public Task ExecuteFault(EventContext<ClearingSagaState> context, Exception exception)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.ExecutionFaulted));
            return Task.CompletedTask;
        }

        public Task ExecuteFault<T>(EventContext<ClearingSagaState, T> context, Exception exception)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.ExecutionFaulted));
            return Task.CompletedTask;
        }

        public Task StateChanged(InstanceContext<ClearingSagaState> context, State currentState, State previousState)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromStateTransition(context.Instance.CorrelationId, currentState, previousState));
            return Task.CompletedTask;
        }
    }
}