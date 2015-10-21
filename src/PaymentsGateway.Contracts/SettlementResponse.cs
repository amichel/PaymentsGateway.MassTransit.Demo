using System;
using MassTransit;

namespace PaymentsGateway.Contracts
{
    public class SettlementResponse : CorrelatedBy<Guid>
    {
        public ClearingStatus ClearingStatus { get; set; }
        public Guid TransactionId { get; set; }
        public string ProviderTransactionId { get; set; }
        public Guid CorrelationId => TransactionId;
    }
}