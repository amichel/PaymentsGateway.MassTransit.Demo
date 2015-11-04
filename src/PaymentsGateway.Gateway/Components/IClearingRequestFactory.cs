using System;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway.Components
{
    public interface IClearingRequestFactory
    {
        AuthorizationRequest FromDepositRequest(Guid transactionId, CcDepositRequest request);
        SettlementRequest FromAuthorizationResponse(AuthorizationResponse response);
    }
}