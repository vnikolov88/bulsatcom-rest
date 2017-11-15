using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace onepoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var random = new Random();
                var bulsatcom = new BulsatcomUtils("https://api.iptv.bulsat.com");
                while (true)
                {
                    var result = await bulsatcom.AuthenticateAsync("user", "pass");
                    if (result)
                    {
                        // TODO: Get the channel list
                    }

                    // Note: wait from a min to 2 hours on each update
                    Thread.Sleep(random.Next(60 * 1000 * 1, 60 * 1000 * 60)); // 1 min to 60 min wait
                    Thread.Sleep(random.Next(60 * 1000 * 1, 60 * 1000 * 60)); // 1 min to 60 min wait
                }
            });
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
