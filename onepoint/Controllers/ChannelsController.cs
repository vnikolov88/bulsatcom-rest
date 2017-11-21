using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using onepoint.Services;

namespace onepoint.Controllers
{
    public class ChannelsController : Controller
    {
        // GET channels/list
        [HttpGet("channels/list.m3u8")]
        public IActionResult List([FromServices] ChannelService channelService)
        {
            return Content(channelService.GetCacheM3U8(), "video/m3u8");
        }

        // GET channels/get/
        [HttpGet("channels/get/{epgName}.m3u8")]
        public void Get(
            [FromServices] ChannelService channelService,
            string epgName
            )
        {
            var channel = channelService.GetChannel(epgName);
            ControllerContext.HttpContext.Response.Redirect(WebUtility.UrlDecode(channel?.source), false);
        }
    }
}
