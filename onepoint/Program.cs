using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using onepoint.Services;

namespace onepoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = BuildWebHost(args);
            var optionsProvider =
                (webHost.Services.GetService(typeof(IOptions<ConfigOptions>)) as IOptions<ConfigOptions>);
            var options = optionsProvider?.Value;
            var channelService =
                (webHost.Services.GetService(typeof(ChannelService)) as ChannelService);
            Task.Run(async () =>
            {
                var random = new Random();
                var bulsatcom = new BulsatcomUtils(options.BaseUrl);
                while (true)
                {
                    var result = await bulsatcom.AuthenticateAsync(options.Username, options.Password);
                    if (result)
                    {
                        
                        // get all channels
                        var channels = await bulsatcom.ChannelAsync();
                        // add epg for every channel
                        if (channels.Count > 0)
                        {
                            channels = await bulsatcom.EPGAsync(channels);
                        }

                        channelService?.UpdateChannels(channels);

                        Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}]Channel list updated with {channels?.Count ?? 0}");
                    }

                    // Note: wait from a min to 2 hours on each update
                    Thread.Sleep(random.Next(60 * 1000 * 1, 60 * 1000 * 60)); // 1 min to 60 min wait
                    Thread.Sleep(random.Next(60 * 1000 * 1, 60 * 1000 * 60)); // 1 min to 60 min wait
                }
            });
            webHost.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                    }
                )
                .UseStartup<Startup>()
                .Build();
    }
}
