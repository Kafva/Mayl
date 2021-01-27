using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gmail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Services can be of the types transient, scoped or singleton
            //  * Transient objects are always different; a new instance is provided to every controller and every service.
            //  * Scoped objects are the same within a request, but different across different requests.
            //  * Singleton objects are the same for every object and every request. 
            
            services.AddSingleton<IGmailAPI<EmailThread>, GmailAPI>( 
                _ => new GmailAPI(Program.applicationName, GmailAPI.scopes)
            );
            
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            
            app.UseStaticFiles();

            app.UseRouting();
 
            app.UseEndpoints(endpoints =>
            {
                // The filenames of the .cshtml pages under ./Pages/
                // will act as routes for the Razor pages
                endpoints.MapRazorPages();

                // Setup non-interactive REST endpoints
                this.setupEndpoints(endpoints);
            });
        }

        private void setupEndpoints(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/mail", async httpContext =>
            // REST endpoint for fetching all threads
            // ?label=(string)
            // ?fetchBody=(true if present)
            {
                Microsoft.Extensions.Primitives.StringValues label = "";

                if ( httpContext.Request.Query.TryGetValue("label", out label) )
                // 'out' keyword caues a parameter to be passed by reference
                {
                    // Fetch the single-instance GmailAPI service of the app 
                    var services = httpContext.RequestServices;
                    var gmailAPI = (IGmailAPI<EmailThread>)services.GetService(typeof(IGmailAPI<EmailThread>)); 
                    
                    Microsoft.Extensions.Primitives.StringValues _fetchBody;
                    bool fetchBody = false;
                    if (httpContext.Request.Query.TryGetValue("fetchBody", out _fetchBody) ) fetchBody = true;
                    
                    var threads = gmailAPI.getThreadsFromLabel("me", label, fetchBody);
                    await httpContext.Response.WriteAsJsonAsync( JsonSerializer.Serialize(threads) );
                } 
                else { await httpContext.Response.WriteAsync("{}"); }
            });
            
            endpoints.MapGet("/thread", async httpContext =>
            // REST endpoint for fetching data regarding a specific thread
            // ?id=(threadId)
            {
                Microsoft.Extensions.Primitives.StringValues threadId = "";

                if ( httpContext.Request.Query.TryGetValue("id", out threadId) )
                {
                    // Fetch the single-instance GmailAPI service of the app 
                    var services = httpContext.RequestServices;
                    var gmailAPI = (IGmailAPI<EmailThread>)services.GetService(typeof(IGmailAPI<EmailThread>)); 
                    
                    var messages = gmailAPI.fetchThreadMessages("me", threadId);
                    await httpContext.Response.WriteAsJsonAsync( JsonSerializer.Serialize(messages) );
                } 
                else { await httpContext.Response.WriteAsync("{}"); }
            });

        }
    }
}
