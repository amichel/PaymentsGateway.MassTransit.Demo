using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentsGateway.Contracts
{
    public class CcDepositRequest
    {
        public int AccountNumber { get; set; }
        public int CardId { get; set; }
        public double Amount { get; set; }
    }
}
