using System;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway.Components
{
    public interface ICustomerBalance
    {
        void Credit(CcDepositRequest depositRequest, AuthorizationResponse response, Action<CcDepositResponse> onFunded);
    }
}