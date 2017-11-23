using Microsoft.AspNetCore.Mvc;
using onepoint.Helpers;
using System.Xml.Linq;
using onepoint.Services;

namespace onepoint.Controllers
{
    public class EpgController : Controller
    {
        [HttpGet("api/[controller]/get/epg.xml")]
        public IActionResult get([FromServices] ChannelService channelService)
        {
            XmlEpgHelper xmlEpgHelper = new XmlEpgHelper();

            if (xmlEpgHelper.loadXml() == true)
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