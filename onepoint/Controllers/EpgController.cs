using Microsoft.AspNetCore.Mvc;
using onepoint.Helpers;
using onepoint.Services;
using System.IO;

namespace onepoint.Controllers
{
    public class EpgController : Controller
    {
        [HttpGet("/epg.xml.gz")]
        public IActionResult GetZip()
        {
            XmlEpgHelper xmlEpgHelper = new XmlEpgHelper();

            FileStream zip = xmlEpgHelper.getZip();

            if (zip != null)
            {
                return File(zip, "application/x-gzip");
            }

            return NotFound();
        }

        [HttpGet("/epg.xml")]
        public IActionResult GetXml([FromServices] ChannelService channelService)
        {
            XmlEpgHelper xmlEpgHelper = new XmlEpgHelper();

            if (xmlEpgHelper.loadXml(true) != null)
            {
                return Content(xmlEpgHelper.doc.ToString(), "text/xml");
            }
            else if (xmlEpgHelper.createXml(channelService.getChannels()) == true)
            {
                return Content(xmlEpgHelper.doc.ToString(), "text/xml");
            }
            
            return NotFound();
        }
    }
}