using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public class DepositValidator : IDepositValidator
    {
        private readonly CcDepositRequest _request;

        public DepositValidator(CcDepositRequest request)
        {
            _request = request;
        }


        public DepositValidationResponse Validate()
        {
            var validationResults = new List<ValidationResult>();
            if (_request.Amount < 0.01)
                validationResults.Add(ValidationResult.InValidAmountRange);

            return new DepositValidationResponse
            {
                IsValid = validationResults.Count == 0,
                Request = _request,
                ValidationResults = validationResults
            };
        }

        public Task<DepositValidationResponse> ValidateAsync()
        {
            return Task.Run(() => Validate());
        }
    }
}