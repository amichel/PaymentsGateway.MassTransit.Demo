using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public class ClearingRequestFactory
    {
        public ClearingRequest FromDepositRequest(CcDepositRequest request)
        {
            return new ClearingRequest
            {
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                CardToken = "ABC123", //TODO: Call Tokenization Service to get token for saved card id
                CardType = CardType.MasterCard, //TODO: Get card type from tokenization service
                Currency = request.Currency
            };
        }
    }
}