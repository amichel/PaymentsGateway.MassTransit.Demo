namespace PaymentsGateway.Contracts
{
    public enum DepositStatus
    {
        Pending,
        Failed,
        Success,
        Rejected,
        Invalid,
        Timedout
    }
}