using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace onepoint.Controllers
{
    public class EpgController : Controller
    {
        [HttpGet("api/[controller]/get/epg.xml")]
        public IActionResult get()
        {
            return View();
        }
    }
}