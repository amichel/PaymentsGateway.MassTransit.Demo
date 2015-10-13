using System;

namespace PaymentsGateway.Contracts
{
    public class ClearingResponse
    {
        public ClearingStatus ClearingStatus { get; set; }
        public Guid TransactionId { get; set; }
        public string ProviderTransactionId { get; set; }
        public int ResponseCode { get; set; }
        public int ErrorCode { get; set; }
    }
}