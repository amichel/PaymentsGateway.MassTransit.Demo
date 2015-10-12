using System;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public interface IDepositValidator
    {
        void ValidateRequest(CcDepositRequest request, Action<DepositValidationResponse> onValidated);
    }
}