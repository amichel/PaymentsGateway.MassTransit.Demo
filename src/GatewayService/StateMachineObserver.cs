using System;
using System.Threading.Tasks;
using Automatonymous;
using MassTransit;
using PaymentsGateway.Contracts;
using PaymentsGateway.Gateway;

namespace PaymentsGateway.GatewayService
{
    internal class StateMachineObserver : EventObserver<GatewaySagaState>, StateObserver<GatewaySagaState>
    {
        private readonly IBus _bus;

        public StateMachineObserver(IBus bus)
        {
            _bus = bus;
        }

        public Task PreExecute(EventContext<GatewaySagaState> context)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.PreExecute));
            return Task.CompletedTask;
        }

        public Task PreExecute<T>(EventContext<GatewaySagaState, T> context)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.PreExecute));
            return Task.CompletedTask;
        }

        public Task PostExecute(EventContext<GatewaySagaState> context)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.PostExecute));
            return Task.CompletedTask;
        }

        public Task PostExecute<T>(EventContext<GatewaySagaState, T> context)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.PostExecute));
            return Task.CompletedTask;
        }

        public Task ExecuteFault(EventContext<GatewaySagaState> context, Exception exception)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.ExecutionFaulted));
            return Task.CompletedTask;
        }

        public Task ExecuteFault<T>(EventContext<GatewaySagaState, T> context, Exception exception)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromEventExecution(context.Instance.CorrelationId, context.Instance.CurrentState, context.Event.Name, MonitoringEventType.ExecutionFaulted));
            return Task.CompletedTask;
        }

        public Task StateChanged(InstanceContext<GatewaySagaState> context, State currentState, State previousState)
        {
            _bus.Publish(SagaMonitoringEventExtensions.FromStateTransition(context.Instance.CorrelationId, currentState, previousState));
            return Task.CompletedTask;
        }
    }
}