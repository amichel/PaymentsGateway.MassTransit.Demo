using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentsGateway.Contracts
{
    public class DepositValidationResponse
    {
        public CcDepositRequest Request { get; set; }
        public bool IsValid { get; set; }
        public IEnumerable<ValidationResult> ValidationResults { get; set; }
    }

    public enum ValidationResult
    {

    }
}
