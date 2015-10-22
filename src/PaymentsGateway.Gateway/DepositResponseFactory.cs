using System;
using Automatonymous.Events;
using MassTransit;
using Newtonsoft.Json;
using PaymentsGateway.Contracts;

namespace PaymentsGateway.Gateway
{
    public interface IDepositResponseFactory
    {
        CcDepositResponse FromPendingRequest(Guid transactionId, CcDepositRequest request);
        CcDepositResponse FromFailedValidationResponse(Guid transactionId, DepositValidationResponse source);
        CcDepositResponse FromClearingResponse(CcDepositRequest request, ClearingResponse source);
        CcDepositResponse FromClearingFault(Fault<ClearingRequest> source);

        CcDepositResponse FromClearingTimeout(Guid transactionId, CcDepositRequest request,
            RequestTimeoutExpired source);
    }

    public class DepositResponseFactory : IDepositResponseFactory
    {
        public CcDepositResponse FromPendingRequest(Guid transactionId, CcDepositRequest request)
        {
            return new CcDepositResponse
            {
                AccountNumber = request.AccountNumber,
                Status = DepositStatus.Pending,
                TransactionId = transactionId
            };
        }

        public CcDepositResponse FromFailedValidationResponse(Guid transactionId, DepositValidationResponse source)
        {
            if (source.IsValid)
                throw new InvalidOperationException("Source validation response must be in invalid state");

            return new CcDepositResponse
            {
                AccountNumber = source.Request.AccountNumber,
                Status = DepositStatus.Invalid,
                ErrorMessage = JsonConvert.SerializeObject(source.ValidationResults),
                TransactionId = transactionId
            };
        }

        public CcDepositResponse FromClearingResponse(CcDepositRequest request, ClearingResponse source)
        {
            return new CcDepositResponse
            {
                AccountNumber = request.AccountNumber,
                Status =
                    source.ClearingStatus == ClearingStatus.Authorized ? DepositStatus.Success : DepositStatus.Rejected,
                ErrorMessage =
                    source.ClearingStatus == ClearingStatus.Authorized
                        ? ""
                        : $"Clearing Api rejected transaction. ErrorCode={source.ErrorCode} ResponseCode={source.ResponseCode}",
                TransactionId = source.TransactionId
            };
        }

        public CcDepositResponse FromClearingFault(Fault<ClearingRequest> source)
        {
            return new CcDepositResponse
            {
                AccountNumber = source.Message.AccountNumber,
                Status = DepositStatus.Failed,
                ErrorMessage = $"Clearing Api call failed. FaultId={source.FaultId} Host={source.Host.MachineName}",
                TransactionId = source.Message.TransactionId
            };
        }

        public CcDepositResponse FromClearingTimeout(Guid transactionId, CcDepositRequest request,
            RequestTimeoutExpired source)
        {
            return new CcDepositResponse
            {
                AccountNumber = request.AccountNumber,
                Status = DepositStatus.Timedout,
                ErrorMessage = $"Clearing Api call timed out. Expiration Time={source.ExpirationTime}",
                TransactionId = transactionId
            };
        }
    }
}