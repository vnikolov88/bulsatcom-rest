using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using onepoint.Models.Channel;

namespace onepoint.Services
{
    public class ChannelService
    {
        private List<ChannelModel> channels;
        private readonly ConcurrentDictionary<string, ChannelModel> _channels;
        private string _channelsCache;
        private readonly string _channelRemapBaseUrl;

        public ChannelService([FromServices] IOptions<ConfigOptions> options)
        {
            _channelRemapBaseUrl = options?.Value.ChannelRemapBaseUrl;
            _channelsCache = string.Empty;
            _channels = new ConcurrentDictionary<string, ChannelModel>();
        }

        public void UpdateChannels(List<ChannelModel> channels)
        {
            var tempChannelCache = "#EXTM3U tvg-shift=2"; // This indicates the TV is in GMT+2
            foreach (var channel in channels)
            {
                // Update channel list
                _channels.AddOrUpdate(channel.epg_name, channel, (key, next) => channel);
                // Update channel cache
                tempChannelCache += $"\n#EXTINF:-1 tvg-id=\"{channel.epg_name}\" tvg-name=\"{channel.title}\" tvg-logo=\"{channel.epg?.num}\" group-title=\"{channel.genre}\",{channel.title}\n" +
                                    $"{_channelRemapBaseUrl}/{channel.epg_name}.m3u8";
            }
            _channelsCache = tempChannelCache;
        }

        public string GetCacheM3U8()
        {
            return _channelsCache;
        }

        public ChannelModel GetChannel(string epgName)
        {
            _channels.TryGetValue(epgName, out var result);
            return result;
        }

        public void setChannels(List<ChannelModel> channels)
        {
            this.channels = channels;
        }

        public List<ChannelModel> getChannels()
        {
            return channels;
        }
    }
}
