using System.Threading.Tasks;
using MassTransit;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Clearing
{
    public interface IClearingApi
    {
        Task<AuthorizationResponse> ProcessRequest(AuthorizationRequest request);
        Task<SettlementResponse> ProcessRequest(SettlementRequest request);
    }
}