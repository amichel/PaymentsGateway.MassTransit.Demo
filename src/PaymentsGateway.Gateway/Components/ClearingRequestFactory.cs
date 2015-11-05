using System;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway.Components
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
                    CardToken = "ABC123",
                    CardType = CardType.MasterCard,
                    Currency = request.Currency
                };
        }

        public SettlementRequest FromAuthorizationResponse(AuthorizationResponse response)
        {
            return new SettlementRequest
                {
                    AccountNumber = response.AccountNumber,
                    TransactionId = response.TransactionId,
                    ProviderTransactionId = response.ProviderTransactionId
                };
        }
    }
}