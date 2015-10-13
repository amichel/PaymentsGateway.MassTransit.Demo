namespace PaymentsGateway.Contracts
{
    public class CcDepositRequest
    {
        public int AccountNumber { get; set; }
        public int CardId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}