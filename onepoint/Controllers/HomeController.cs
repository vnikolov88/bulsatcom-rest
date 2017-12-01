using Microsoft.AspNetCore.Mvc;

namespace onepoint.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("[controller]/index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[controller]/register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("[controller]/login")]
        public IActionResult Login()
        {
            return View();
        }
    }
}