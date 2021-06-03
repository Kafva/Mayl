using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace Web
{
    public class Program
    {
        public const int PORT = 5443;
        public static void Main(string[] args)
        {   
            CreateHostBuilder(args).Build().Run();    
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var config = createSSLConfig();
                    
                    // Read in the location of the cert from certificate.json
                    var certificateSettings    = config.GetSection("certificateSettings");
                    string certificateFileName = certificateSettings.GetValue<string>("filename");
                    string certificatePassword = certificateSettings.GetValue<string>("password");

                    var certificate = new X509Certificate2(certificateFileName, certificatePassword);
                            
                    webBuilder.UseKestrel(
                    options =>
                    {
                        options.AddServerHeader = false;
                        // Switch to 'Any' if deploying outside localhost
                        options.Listen(IPAddress.Loopback, Program.PORT, listenOptions =>
                        {
                            listenOptions.UseHttps(certificate);
                        });
                    }); 
                    
                    webBuilder.UseConfiguration(config);
                    
                    // Change the webroot for static files
                    webBuilder.UseWebRoot("./Client/public");
                    webBuilder.UseStartup<Startup>();
                });
        
        private static IConfigurationRoot createSSLConfig()
        // https://www.humankode.com/asp-net-core/develop-locally-with-https-self-signed-certificates-and-asp-net-core
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("secret/certificate.json", optional: true, reloadOnChange: true)
                .Build();

            return config;
        }
    }
}
