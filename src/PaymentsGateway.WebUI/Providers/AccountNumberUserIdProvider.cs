using Microsoft.AspNet.SignalR;

namespace PaymentsGateway.WebUI.Providers
{
    public class AccountNumberUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.QueryString["AccountNumber"];
        }
    }
}