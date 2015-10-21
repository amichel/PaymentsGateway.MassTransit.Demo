using System;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public class ClearingRequestFactory : IClearingRequestFactory
    {
        public AuthorizationRequest FromDepositRequest(Guid transactionId, CcDepositRequest request)
        {
            return new AuthorizationRequest
            {
                TransactionId = transactionId,
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                CardToken = "ABC123", //TODO: Call Tokenization Service to get token for saved card id
                CardType = CardType.MasterCard, //TODO: Get card type from tokenization service
                Currency = request.Currency
            };
        }
    }
}