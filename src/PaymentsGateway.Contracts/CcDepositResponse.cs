using System;
using MassTransit;

namespace PaymentsGateway.Contracts
{
    public class CcDepositResponse : CorrelatedBy<Guid>
    {
        public DepositStatus Status { get; set; }
        public int AccountNumber { get; set; }
        public Guid TransactionId { get; set; }
        public string ErrorMessage { get; set; }
        public Guid CorrelationId => TransactionId;
    }
}