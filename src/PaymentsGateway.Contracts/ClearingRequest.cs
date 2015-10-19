using System;
using MassTransit;

namespace PaymentsGateway.Contracts
{
    public class ClearingRequest : CorrelatedBy<Guid>
    {
        public int AccountNumber { get; set; }
        public Guid TransactionId { get; set; }
        public CardType CardType { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public string CardToken { get; set; }
        public Guid CorrelationId => TransactionId;
    }
}