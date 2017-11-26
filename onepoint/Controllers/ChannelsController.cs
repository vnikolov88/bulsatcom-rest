using System.Net;
using Microsoft.AspNetCore.Mvc;
using onepoint.Services;

namespace onepoint.Controllers
{
    public class ChannelsController : Controller
    {
        /// <summary>
        /// Return a formated m3u8 file with all channels,
        /// The URL's contained inside are pointing to a proxy endpoint
        /// to redirect them to the actual stream
        /// </summary>
        /// <param name="channelService">Needed to get the cached m3u8 for all channels</param>
        /// <returns>The cached m3u8 file in video/m3u8 mime type</returns>
        [HttpGet("/ls.m3u8")]
        public IActionResult List([FromServices] ChannelService channelService)
        {
            return Content(channelService.GetCacheM3U8(), "video/m3u8");
        }
        
        /// <summary>
        /// Proxy endpoint to redirect the player to the actual stream URL
        /// based on the channel epgName
        /// </summary>
        /// <param name="channelService">Needed to get the actual source URL for the channel</param>
        /// <param name="epgName">the epg name of the channel</param>
        [HttpGet("/get/{epgName}.m3u8")]
        public void Get([FromServices] ChannelService channelService, string epgName)
        {
            var channel = channelService.GetChannel(epgName);
            ControllerContext.HttpContext.Response.Redirect(WebUtility.UrlDecode(channel?.source), false);
        }
    }
}
