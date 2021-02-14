using System;
using Microsoft.AspNetCore.Http;
using Gmail;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Utils;

namespace Web.Dispatchs
{
   public class Dispatch
   {
        public const string EMPTY_RESPONSE = "[]";
        public const string MAIL_ENDPOINT = "mail";
        public const string THREAD_ENDPOINT = "thread";
        public const string LABELS_ENDPOINT = "labels";
        public const string ACCOUNTS_ENDPOINT = "accounts";
        public const string UNTAG_ENDPOINT = "untag";
        public const string TRASH_ENDPOINT = "trash";
        public Dispatch(){}

        public async Task routeDispatcher(HttpContext httpContext, string dispatch )
        {
            // Action is used instead of Func when the passed function doesn't have a return value 
            
            object userId; 
            if( httpContext.Request.RouteValues.TryGetValue("userId", out userId) )
            // All endpoints except /accounts require the userId route value
            // (and are sent in base64)
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
                    case Dispatch.UNTAG_ENDPOINT:
                        await this.untagDispatch(httpContext,userId.ToString());
                        break;
                    case Dispatch.TRASH_ENDPOINT:
                        await this.trashDispatch(httpContext,userId.ToString());
                        break;
                    default:
                        await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE);
                        break;
                } 
            } 
            else 
            { 
                if(dispatch == Dispatch.ACCOUNTS_ENDPOINT)
                {
                    await this.accountsDispatch(httpContext);
                }
                else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
            }
        }
        private async Task accountsDispatch(HttpContext httpContext)
        {
            // Return a comma seperated string of accounts
            await httpContext.Response.WriteAsync( string.Join(",", Util.getAccounts().ToArray()) );
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
                var gmailAPI = ((GmailInstanceContainer)(services.GetService(typeof(GmailInstanceContainer))))
                    .getGmailAPIInstance(userId);

                if(gmailAPI != null)
                {
                    Microsoft.Extensions.Primitives.StringValues _fetchBody;
                    bool fetchBody = false;
                    if (httpContext.Request.Query.TryGetValue("fetchBody", out _fetchBody) ) fetchBody = true;
                    
                    var threads = gmailAPI.getThreadsFromLabel(label, fetchBody);
                    
                    // Transmit the data as utf-8 bytes encoded in base64 to avoid
                    // encoding inconsitcies 
                    await httpContext.Response.WriteAsync(  Convert.ToBase64String( 
                            JsonSerializer.SerializeToUtf8Bytes(threads), Base64FormattingOptions.None
                        )
                    );
                }
                else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
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
                var gmailAPI = ((GmailInstanceContainer)(services.GetService(typeof(GmailInstanceContainer))))
                    .getGmailAPIInstance(userId);

                if(gmailAPI != null)
                {
                    var messages = gmailAPI.fetchThreadMessages(threadId);
                    await httpContext.Response.WriteAsync(  Convert.ToBase64String( 
                            JsonSerializer.SerializeToUtf8Bytes(messages), Base64FormattingOptions.None
                        )
                    );
                }
                else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
            }
            else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
        }
        private async Task labelsDispatch(HttpContext httpContext, string userId)
        {
            // Fetch the single-instance GmailAPI service of the app 
            var services = httpContext.RequestServices;
            var gmailAPI = ((GmailInstanceContainer)(services.GetService(typeof(GmailInstanceContainer))))
                .getGmailAPIInstance(userId);

            if(gmailAPI != null)
            {
                var labels = gmailAPI.getLabels();
                await httpContext.Response.WriteAsync(  Convert.ToBase64String( 
                        JsonSerializer.SerializeToUtf8Bytes(labels), Base64FormattingOptions.None
                    )
                );
            }
            else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
        }
        
        private async Task untagDispatch(HttpContext httpContext, string userId)
        {
            StringValues threadId = "";
            StringValues tag = "";

            if ( httpContext.Request.Query.TryGetValue("id", out threadId) )
            {
                if ( httpContext.Request.Query.TryGetValue("tag", out tag) )
                {
                    // Fetch the single-instance GmailAPI service of the app 
                    var services = httpContext.RequestServices;
                    var gmailAPI = ((GmailInstanceContainer)(services.GetService(typeof(GmailInstanceContainer))))
                        .getGmailAPIInstance(userId);

                    if(gmailAPI != null)
                    {
                        // Remove the given label and return true/false based on if the operation was successful
                        var labels = gmailAPI.updateThreadStatus(threadId, UpdateAction.UNTAG, tag);
                        await httpContext.Response.WriteAsync( (!labels.Contains(tag)).ToString() );
                    }
                    else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
                }
                else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
            }
            else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
        }
        private async Task trashDispatch(HttpContext httpContext, string userId)
        {
            StringValues threadId = "";

            if ( httpContext.Request.Query.TryGetValue("id", out threadId) )
            {
                // Fetch the single-instance GmailInstanceContainer service of the app
                // and retrieve the GmailAPI instance for the corresponding 
                var services = httpContext.RequestServices;
                var gmailAPI = ((GmailInstanceContainer)(services.GetService(typeof(GmailInstanceContainer))))
                    .getGmailAPIInstance(userId);

                if(gmailAPI != null)
                {
                    // Add the TRASH label and return true/false based on if the operation was successful
                    var labels = gmailAPI.updateThreadStatus(threadId, UpdateAction.TAG, GmailAPI.TRASH_LABEL);
                    await httpContext.Response.WriteAsync(labels.Contains(GmailAPI.TRASH_LABEL).ToString() );
                }
                else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
            }
            else { await httpContext.Response.WriteAsync(Dispatch.EMPTY_RESPONSE); }
        }
   }
}
