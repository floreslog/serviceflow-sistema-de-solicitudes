using Microsoft.AspNetCore.Mvc;

namespace ServiceFlow.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/NotFound")]
        public new IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("Error404");
        }
    }
}