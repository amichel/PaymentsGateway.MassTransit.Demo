namespace PaymentsGateway.Contracts
{
    public class CcDepositRequest
    {
        public int AccountNumber { get; set; }
        public int CardId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }

        public string CorellationKey => $"{AccountNumber}#{CardId}";

        public CcDepositRequest Copy() => MemberwiseClone() as CcDepositRequest;
    }
}