using System;
using Microsoft.AspNetCore.Http;
using Gmail;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Web.Dispatchs
{
   public class Dispatch
   {
        public const string EMPTY_RESPONSE = "[]";
        public const string MAIL_ENDPOINT = "mail";
        public const string THREAD_ENDPOINT = "thread";
        public const string LABELS_ENDPOINT = "labels";
        public Dispatch(){}
        public async Task routeDispatcher(HttpContext httpContext, string dispatch )
        // Action is used instead of Func when the passed function doesn't have a return value 
        {
            object userId; 
            if( httpContext.Request.RouteValues.TryGetValue("userId", out userId) )
            {
                switch(dispatch)
                {
                    case Dispatch.MAIL_ENDPOINT:
                        await this.mailDispatch(httpContext,userId.ToString());
                        break;
                    case Dispatch.THREAD_ENDPOINT:
                        await this.threadDispatch(httpContext,userId.ToString());
                        break;
                    case Dispatch.LABELS_ENDPOINT:
                        await this.labelsDispatch(httpContext,userId.ToString());
                        break;
                    default:
                        await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE);
                        break;
                } 
            } 
            else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
        }

        //************* REST Dispatchs ************//
        private async Task mailDispatch(HttpContext httpContext, string userId)
        {
            StringValues label = "";
            if ( httpContext.Request.Query.TryGetValue("label", out label) )
            // 'out' keyword caues a parameter to be passed by reference
            {
                // Fetch the single-instance GmailAPI service of the app 
                var services = httpContext.RequestServices;
                var gmailAPI = (IGmailAPI<EmailThread>)services.GetService(typeof(IGmailAPI<EmailThread>)); 
                
                Microsoft.Extensions.Primitives.StringValues _fetchBody;
                bool fetchBody = false;
                if (httpContext.Request.Query.TryGetValue("fetchBody", out _fetchBody) ) fetchBody = true;
                
                var threads = gmailAPI.getThreadsFromLabel(userId, label, fetchBody);
                await httpContext.Response.WriteAsJsonAsync( JsonSerializer.Serialize(threads) );
            } 
            else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
        }
        
        private async Task threadDispatch(HttpContext httpContext, string userId)
        {
            StringValues threadId = "";

            if ( httpContext.Request.Query.TryGetValue("id", out threadId) )
            {
                // Fetch the single-instance GmailAPI service of the app 
                var services = httpContext.RequestServices;
                var gmailAPI = (IGmailAPI<EmailThread>)services.GetService(typeof(IGmailAPI<EmailThread>)); 
                
                var messages = gmailAPI.fetchThreadMessages(userId, threadId);
                await httpContext.Response.WriteAsJsonAsync( JsonSerializer.Serialize(messages) );
            }
            else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
        }
        private async Task labelsDispatch(HttpContext httpContext, string userId)
        {
            // Fetch the single-instance GmailAPI service of the app 
            var services = httpContext.RequestServices;
            var gmailAPI = (IGmailAPI<EmailThread>)services.GetService(typeof(IGmailAPI<EmailThread>)); 
            
            var labels = gmailAPI.getLabels(userId);
            await httpContext.Response.WriteAsJsonAsync( JsonSerializer.Serialize(labels) );
        }
        
   }
}