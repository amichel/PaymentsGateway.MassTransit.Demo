using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;

namespace PaymentsGateway.Contracts
{
    public enum MonitoringEventType
    {
        PreExecute,
        PostExecute,
        ExecutionFaulted,
        StateChanged
    }


    public class SagaMonitoringEvent
    {
        public Guid SagaCorellationId { get; set; }
        public string CurrentState { get; set; }
        public string PreviousState { get; set; }
        public string EventName { get; set; }
        public MonitoringEventType EventType { get; set; }
        public DateTime EventTime { get; set; }
    }

    public static class SagaMonitoringEventExtensions
    {
        public static string ToLogString(this SagaMonitoringEvent target)
        {
            if (target.EventType == MonitoringEventType.StateChanged)
                return $"Event Type={target.EventType},CorrelationId={target.SagaCorellationId},From={target.PreviousState},To={target.CurrentState},Time={target.EventTime.ToLongTimeString()}";
            return $"Event Type={target.EventType},CorrelationId={target.SagaCorellationId},State={target.CurrentState},EventName={target.EventName},Time={target.EventTime.ToLongTimeString()}";
        }

        public static SagaMonitoringEvent FromEventExecution(Guid corellationId, State state, string eventName, MonitoringEventType eventType)
        {
            return new SagaMonitoringEvent()
            {
                EventType = eventType,
                CurrentState = state?.Name,
                EventName = eventName,
                SagaCorellationId = corellationId,
                EventTime = DateTime.UtcNow
            };
        }
        public static SagaMonitoringEvent FromStateTransition(Guid corellationId, State currentState, State previousState)
        {
            return new SagaMonitoringEvent()
            {
                EventType = MonitoringEventType.StateChanged,
                CurrentState = currentState?.Name,
                PreviousState = previousState?.Name,
                SagaCorellationId = corellationId,
                EventTime = DateTime.UtcNow
            };
        }
    }
}
