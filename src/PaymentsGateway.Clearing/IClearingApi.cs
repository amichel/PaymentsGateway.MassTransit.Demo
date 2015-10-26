using System.Threading.Tasks;
using MassTransit;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Clearing
{
    public interface IClearingApi
    {
        AuthorizationResponse ProcessRequest(AuthorizationRequest request);
        SettlementResponse ProcessRequest(SettlementRequest request);
    }
}