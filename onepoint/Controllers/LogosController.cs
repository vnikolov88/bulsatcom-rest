using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace onepoint.Controllers
{
    public class LogosController : Controller
    {
        [HttpGet("/logos.zip")]
        public IActionResult GetZip()
        {
            FileStream zip = System.IO.File.OpenRead(@".\Resources\data\logos.zip");

            if (zip != null)
            {
                return File(zip, "application/zip");
            }

            return NotFound();
        }
    }
}
