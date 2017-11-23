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
            if (channelService.getChannels().Count > 0)
            {
                XElement xml = XmlHelper.generateXml(channelService.getChannels());
            }
            
            return View();
        }
    }
}