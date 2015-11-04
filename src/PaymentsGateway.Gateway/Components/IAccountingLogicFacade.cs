using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
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
