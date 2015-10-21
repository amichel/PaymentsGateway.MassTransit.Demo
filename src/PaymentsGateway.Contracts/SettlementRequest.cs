using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace PaymentsGateway.Contracts
{
    public class SettlementRequest : CorrelatedBy<Guid>
    {
        public Guid TransactionId { get; set; }
        public string ProviderTransactionId { get; set; }
        public Guid CorrelationId => TransactionId;
    }
}
