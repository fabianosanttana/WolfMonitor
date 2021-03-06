﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using Totten.Solutions.WolfMonitor.Infra.CrossCutting.Helpers;

namespace Totten.Solutions.WolfMonitor.Cfg.Startup
{
    public static class Run
    {
        public static void Main<TStartup>(string[] args)
        {
            Helper helper = new Helper(null);

            var address = Environment.GetEnvironmentVariable("ADDRESS")
                            ?? helper.GetLocalIpAddress();

            var httpPort = Environment.GetEnvironmentVariable("PORT")
                            ?? helper.GenerateRandomPort().ToString();

            args = new string[] { $"http://{address}:{httpPort}" };

            IWebHost host = CreateWebHostBuilder<TStartup>(args).Build();
            host.Run();
        }
        private static IWebHostBuilder CreateWebHostBuilder<TStartup>(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                            .UseUrls(args)
                            .UseStartup(typeof(TStartup));
        }
    }
}
