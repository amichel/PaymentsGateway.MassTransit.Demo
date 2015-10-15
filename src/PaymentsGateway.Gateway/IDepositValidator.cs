using System.Threading.Tasks;

namespace PaymentsGateway.Gateway
{
    public interface IDepositValidator
    {
        void Validate();
        Task ValidateAsync();
    }
}