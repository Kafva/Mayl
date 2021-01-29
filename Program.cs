using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Google.Apis.Gmail.v1;
using Gmail;

namespace Web
{
    public class Program
    {
        public const string applicationName = "Mayler";

        public static void Main(string[] args)
        {   
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Change the webroot for static files
                    webBuilder.UseWebRoot("./client/public");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
