using System;
using MassTransit;

namespace PaymentsGateway.Contracts
{
    public class ClearingResponse: CorrelatedBy<Guid>
    {
        public int AccountNumber { get; set; }
        public ClearingStatus ClearingStatus { get; set; }
        public Guid TransactionId { get; set; }
        public string ProviderTransactionId { get; set; }
        public int ResponseCode { get; set; }
        public int ErrorCode { get; set; }
        public Guid CorrelationId => TransactionId;
    }
}