using Microsoft.AspNetCore.Mvc;
using onepoint.Models.Entities;
using onepoint.Models.Home;
using onepoint.Services;

namespace onepoint.Controllers
{
    public class HomeController : Controller
    {
        public IRepository<UserAccount> _repository;

        public HomeController(IRepository<UserAccount> repository)
        {
            _repository = repository;
        }

        [HttpGet("[controller]/index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[controller]/login")]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost("[controller]/login")]
        public IActionResult Login(LoginModel m)
        {
            if (ModelState.IsValid)
            {
                // if user and pass match whit our table
                RegisterModel rm = new RegisterModel();
                rm.name = m.name;
                rm.password = m.password;
                rm.key = "x_3hVnNwXgPDqu3vNwbWa5-sMpMKY5pRwwUDgOAqS1k="; // TODO get key from table

                return View("Register", rm);
            }

            return View(m);
        }

        [HttpGet("[controller]/register")]
        public IActionResult Register()
        {
            RegisterModel m = new RegisterModel();
            m.key = "x_3hVnNwXgPDqu3vNwbWa5-sMpMKY5pRwwUDgOAqS1k="; // TODO generate key here

            return View(m);
        }

        [HttpPost("[controller]/register")]
        public IActionResult Register(RegisterModel m)
        {
            if (ModelState.IsValid)
            {
                _repository.AddOrUpdate(new UserAccount()
                {
                    Password = m.password,
                    Username = m.name
                });
                _repository.StoreAsync();
            }

            // else return error, not match

            return View(m);
        }
    }
}