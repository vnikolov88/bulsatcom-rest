using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace onepoint.Controllers
{
    public class LogoController : Controller
    {


        // GET logo/get/key
        [HttpGet("api/[controller]/get/{name}")]
        public IActionResult Get(string name)
        {
            if (name != null && name.Length > 0)
            {
                if (!name.ToLower().Contains(".png"))
                {
                    name = name + ".png";
                }

                FileStream image = System.IO.File.OpenRead(@".\Resources\Images\logos" + Path.DirectorySeparatorChar + name);

                if (image != null)
                {
                    return File(image, "image/png", name);
                }
            }

            return NotFound();
        }
    }
}
