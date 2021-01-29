using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gmail;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Web.Dispatchs; 

namespace Web
{
    public class Startup
    {
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();  
            
            // Services can be of the types transient, scoped or singleton
            //  * Transient objects are always different; a new instance is provided to every controller and every service.
            //  * Scoped objects are the same within a request, but different across different requests.
            //  * Singleton objects are the same for every object and every request. 
            
            services.AddSingleton<IGmailAPI<EmailThread>, GmailAPI>( 
                _ => new GmailAPI(Program.applicationName, GmailAPI.scopes)
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        {
            //if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                // The filenames of the .cshtml pages under ./Pages/
                // will act as routes for the Razor pages
                //endpoints.MapRazorPages();


                endpoints.MapGet("/", async httpContext =>
                {
                   httpContext.Response.Headers.Append("Content-Type", "text/html");
                   await httpContext.Response.SendFileAsync("./client/public/index.html"); 
                });

                var dispatch = new Dispatch();
                
                // Endpoint for fetching all threads
                // ?label=(string)
                // ?fetchBody=(true if present)
                endpoints.MapGet("/{userId}/" + Dispatch.MAIL_ENDPOINT, httpContext => 
                    dispatch.routeDispatcher(httpContext, Dispatch.MAIL_ENDPOINT) );
                
                // Endpoint for fetching data regarding a specific thread
                // ?id=(threadId)
                endpoints.MapGet("/{userId}/" + Dispatch.THREAD_ENDPOINT, httpContext => 
                    dispatch.routeDispatcher(httpContext, Dispatch.THREAD_ENDPOINT) );
                
                // Endpoint for fetching all labels
                endpoints.MapGet("/{userId}/" + Dispatch.LABELS_ENDPOINT, httpContext => 
                    dispatch.routeDispatcher(httpContext, Dispatch.LABELS_ENDPOINT) ); 
            });
        }

    }
}
