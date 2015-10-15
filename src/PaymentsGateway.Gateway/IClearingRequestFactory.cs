using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public interface IClearingRequestFactory
    {
        ClearingRequest FromDepositRequest(CcDepositRequest request);
    }
}