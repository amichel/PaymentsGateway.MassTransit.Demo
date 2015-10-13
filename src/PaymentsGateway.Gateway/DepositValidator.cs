using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public class DepositValidator
    {
        private readonly Action<DepositValidationResponse> _onValidated;
        private readonly CcDepositRequest _request;

        public DepositValidator(CcDepositRequest request, [NotNull] Action<DepositValidationResponse> onValidated)
        {
            _request = request;
            if (onValidated == null)
                throw new ArgumentNullException(nameof(onValidated), "Valid callback must be supplied for onValidated");
            _onValidated = onValidated;
        }


        public void Validate()
        {
            var validationResults = new List<ValidationResult>();
            if (_request.Amount < 0.01)
                validationResults.Add(ValidationResult.InValidAmountRange);

            _onValidated(new DepositValidationResponse
            {
                IsValid = validationResults.Count == 0,
                Request = _request,
                ValidationResults = validationResults
            });
        }

        public Task ValidateAsync()
        {
            return Task.Run(new Action(Validate));
        }
    }
}