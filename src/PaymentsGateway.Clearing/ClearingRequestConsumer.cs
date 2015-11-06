using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Clearing
{
    public class ClearingApiAdaptor : IClearingApi
    {
        private static int _transactionIdSeed = 100;

        public AuthorizationResponse ProcessRequest(AuthorizationRequest request)
        {
            //Long 3rd party Api call
            Thread.Sleep(request.Amount > 1000 ? 30000 : 2000);

            var clearingStatus = request.AccountNumber % 2 == 0 ? ClearingStatus.Authorized : ClearingStatus.Rejected;
            return new AuthorizationResponse()
            {
                TransactionId = request.TransactionId,
                ClearingStatus = clearingStatus,
                ProviderTransactionId =
                    $"c#{request.TransactionId}#{Interlocked.Increment(ref _transactionIdSeed)}",
                ResponseCode = 100,
                ErrorCode = clearingStatus == ClearingStatus.Authorized ? 0 : 2345
            };
        }

        public SettlementResponse ProcessRequest(SettlementRequest request)
        {
            //Long 3rd party Api call
            Thread.Sleep(2000);

            var clearingStatus = ClearingStatus.Settled;
            return new SettlementResponse()
            {
                TransactionId = request.TransactionId,
                ClearingStatus = clearingStatus,
                ProviderTransactionId = request.ProviderTransactionId
            };
        }
    }
}
