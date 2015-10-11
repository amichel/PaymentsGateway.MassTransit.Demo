namespace PaymentsGateway.Contracts
{
    public class ClearingResponse
    {
        public bool IsAuthorized { get; set; }
        public long TransactionId { get; set; }
        public long ProviderTransactionId { get; set; }
        public int ResponseCode { get; set; }
        public int ErrorCode { get; set; }
    }
}