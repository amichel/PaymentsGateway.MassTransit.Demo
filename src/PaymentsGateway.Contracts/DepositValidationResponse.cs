using System.Collections.Generic;

namespace PaymentsGateway.Contracts
{
    public class DepositValidationResponse
    {
        public CcDepositRequest Request { get; set; }
        public bool IsValid { get; set; }
        public IEnumerable<ValidationResult> ValidationResults { get; set; }
    }
}