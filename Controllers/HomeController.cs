using Microsoft.AspNetCore.Mvc;

namespace WebApplicationBasic.Controllers
{
    // DO NOT DELETE. Required for SPA Server Rendering
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
