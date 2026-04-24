using Microsoft.AspNetCore.Mvc;

namespace ServiceFlow.Web.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
