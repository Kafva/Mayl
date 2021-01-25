using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gmail;

namespace Web
{
    public class Startup
    {

        //private GmailAPI gmailAPI {get; set;}

        // This method gets called by the runtime. Use this method to add services to the container.
        // https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Services can be of the types transient, scoped or singleton
            //  * Transient objects are always different; a new instance is provided to every controller and every service.
            //  * Scoped objects are the same within a request, but different across different requests.
            //  * Singleton objects are the same for every object and every request. 
            
            services.AddSingleton<IGmailAPI<EmailMessage>, GmailAPI>( 
                _ => new GmailAPI(Program.applicationName, Program.scopes)
            );
            
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();

            app.UseRouting();
 
            // To serve up the page and asyncrounsly supply the client with
            // emails we use webSockets
            app.UseWebSockets();
            
            app.UseEndpoints(endpoints =>
            {
                // The filenames of the .cshtml pages under ./Pages/
                // will act as routes
                endpoints.MapRazorPages();
            });
        }
    }
}
