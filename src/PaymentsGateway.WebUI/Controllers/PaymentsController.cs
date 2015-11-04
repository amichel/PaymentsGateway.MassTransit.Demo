using System.Web.Mvc;

namespace PaymentsGateway.WebUI.Controllers
{
    public class PaymentsController : Controller
    {
        public ActionResult CreditCardDeposit()
        {
            return View();
        }
    }
}