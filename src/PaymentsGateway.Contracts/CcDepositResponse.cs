using System;

namespace PaymentsGateway.Contracts
{
    public class CcDepositResponse
    {
        public DepositStatus Status { get; set; }
        public int AccountNumber { get; set; }
        public Guid TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }
}