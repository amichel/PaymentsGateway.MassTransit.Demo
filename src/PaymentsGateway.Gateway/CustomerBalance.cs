using System;
using System.Diagnostics;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public class CustomerBalance
    {
        public CustomerBalance()
        {
        }

        public void Credit(CcDepositRequest depositRequest, ClearingResponse response,
            Action<CcDepositResponse> onFunded)
        {
            if (depositRequest.Currency == "DEM")
                throw new ApplicationException("Are you serious? Deposits in Deutsche Mark are not supported since 2002!");

            Debug.WriteLine(
                $"Funding Customer Account. TransactionId={response.TransactionId} AccountNumber={depositRequest.AccountNumber} Amount={depositRequest.Amount} Currency={depositRequest.Currency}");
            onFunded(new DepositResponseFactory().FromClearingResponse(depositRequest, response));
        }
    }
}