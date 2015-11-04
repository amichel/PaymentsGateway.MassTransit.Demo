using System;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway.Components
{
    public interface IAccountingLogicFacade : ICustomerBalance
    {
    }

    public class AccountingLogicFacade : IAccountingLogicFacade
    {
        public void Credit(CcDepositRequest depositRequest, AuthorizationResponse response, Action<CcDepositResponse> onFunded)
        {
            new CustomerBalance().Credit(depositRequest, response, onFunded);
        }
    }
}