using System.Threading.Tasks;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public interface IDepositValidator
    {
        DepositValidationResponse Validate();
        Task<DepositValidationResponse> ValidateAsync();
    }
}