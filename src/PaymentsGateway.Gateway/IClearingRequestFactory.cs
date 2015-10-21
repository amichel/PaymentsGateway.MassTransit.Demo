using System;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public interface IClearingRequestFactory
    {
        AuthorizationRequest FromDepositRequest(Guid transactionId, CcDepositRequest request);
    }
}