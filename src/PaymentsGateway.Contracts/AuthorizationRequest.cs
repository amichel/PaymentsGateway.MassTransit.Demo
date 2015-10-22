using System;
using MassTransit;

namespace PaymentsGateway.Contracts
{
    public class AuthorizationRequest : ClearingRequest
    {
        public CardType CardType { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public string CardToken { get; set; }
    }
}