using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace onepoint.Controllers
{
    public class LogoController : Controller
    {
        [HttpGet("api/[controller]/get/{id}")]
        public IActionResult get(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(@".\Resources\Images\logos\" + id);

            if (image != null)
            {
                return File(image, "image/png");
            }

            return NotFound();
        }
    }
}
