using System;
using MassTransit;

namespace PaymentsGateway.Contracts
{
    public class ClearingRequest : CorrelatedBy<Guid>
    {
        public int AccountNumber { get; set; }
        public Guid TransactionId { get; set; }
        public Guid CorrelationId => TransactionId;
    }
}