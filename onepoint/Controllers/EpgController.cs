using Microsoft.AspNetCore.Mvc;
using onepoint.Helpers;
using onepoint.Services;
using System;
using System.IO;

namespace onepoint.Controllers
{
    public class EpgController : Controller
    {


        // GET epg/get/xml
        [HttpGet("api/[controller]/get/xml/{days}")]
        public IActionResult GetXml([FromServices] ChannelService channelService, string days)
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


        /// <summary>
        /// 
        /// Many IPTV client support archived epg
        /// 
        /// </summary>
        /// <returns></returns>
        // GET epg/get/zip
        [HttpGet("api/[controller]/get/zip/{days}")]
        public IActionResult GetZip(string days)
        {
            XmlEpgHelper xmlEpgHelper = new XmlEpgHelper();

            FileStream zip = xmlEpgHelper.getZip();

            if (zip != null)
            {
                return File(zip, "application/x-gzip", "epg.xml.gz");
            }

            return NotFound();
        }
    }
}