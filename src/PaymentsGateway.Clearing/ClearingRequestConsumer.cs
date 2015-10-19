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
    public class ClearingRequestConsumer : IConsumer<ClearingRequest>
    {
        private static int _transactionIdSeed = 100;
        public Task Consume(ConsumeContext<ClearingRequest> context)
        {
            //Long 3rd party Api call
            Thread.Sleep(context.Message.Amount > 1000 ? 10000 : 1000);

            var clearingStatus = context.Message.AccountNumber % 2 == 0 ? ClearingStatus.Authorized : ClearingStatus.Rejected;
            return context.RespondAsync(new ClearingResponse()
            {
                TransactionId = context.Message.TransactionId,
                ClearingStatus = clearingStatus,
                ProviderTransactionId =
                    $"c#{context.Message.TransactionId}#{Interlocked.Increment(ref _transactionIdSeed)}",
                ResponseCode = 100,
                ErrorCode = clearingStatus == ClearingStatus.Authorized ? 0 : 2345
            });
        }
    }
}
