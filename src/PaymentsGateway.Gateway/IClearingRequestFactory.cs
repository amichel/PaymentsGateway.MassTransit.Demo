using System;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public interface IClearingRequestFactory
    {
        ClearingRequest FromDepositRequest(Guid transactionId, CcDepositRequest request);
    }
}