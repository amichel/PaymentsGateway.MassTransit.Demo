using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public interface IDepositValidatorFactory
    {
        IDepositValidator CreateCcDepositValidator(CcDepositRequest request,
            [NotNull] Action<DepositValidationResponse> onValidated);
    }

    public class DepositValidatorFactory : IDepositValidatorFactory
    {
        public IDepositValidator CreateCcDepositValidator(CcDepositRequest request, Action<DepositValidationResponse> onValidated)
        {
            return new DepositValidator(request, onValidated);
        }
    }
}
