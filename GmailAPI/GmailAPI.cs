using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
//using Microsoft.AspNetCore.WebUtilities;


// API: https://googleapis.dev/dotnet/Google.Apis.Gmail.v1/latest/api/Google.Apis.Gmail.v1.html
// TODO
// Label argument for getThreads(), labels are included in each (gmail) Message object
// WebSockets
// Testing framework
// Entity-Model-Controller...


// Labels:
namespace Gmail
{

    public interface IGmailAPI<T>
    // Let the GmailAPI class inherit from the interface
    {
        IList<T[]> getThreads(string userId);
    }

    public class GmailAPI : IGmailAPI<EmailMessage>
    {
        
        public static readonly string[] scopes = { 
            GmailService.Scope.GmailReadonly, 
            GmailService.Scope.MailGoogleCom, 
            GmailService.Scope.GmailModify 
        };
            
        public const string tokenPath = "./secret/token";
        public const string credPath = "./secret/credentials.json";
        private GmailService service;
        private UserCredential credentials;
        
        //**********************************************//
        public GmailAPI(string ApplicationName, string[] scopes)
        {
            // Open a browser session to authenticate with the app if no tokens.json directory exists
            this.setupCredentials();
            
            // Create Gmail API service.
            this.service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });
        }

        private void setupCredentials()
        {
            if ( !File.Exists(GmailAPI.tokenPath) )
            {
                Console.WriteLine(string.Format("Initialising authorization for new token ({0})", GmailAPI.tokenPath));
                using (var stream = new FileStream(GmailAPI.credPath, FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    // I.e. we only need to authorize once in the browser after which all API communcation
                    // will be done server side
                    this.credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        GmailAPI.scopes,
                        "user",
                        System.Threading.CancellationToken.None,
                        new FileDataStore(GmailAPI.tokenPath, true)).Result;
                }
            }
            else { Console.WriteLine("Using existing credentials at " + GmailAPI.tokenPath); }
        }

        private byte[] decodeBase64Url(string arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            var s = arg
                    .PadRight(arg.Length + (4 - arg.Length % 4) % 4, '=')
                    .Replace("_", "/")
                    .Replace("-", "+");
            return Convert.FromBase64String(s);
        }
        public IList<EmailMessage[]> getThreads(string userId)
        {
            // Return a list of EmailMessage arrays (one per Threed)
            // [ 
            //  [ { Subject, Sender, Date, Body }, {...} ],
            //  [...]
            // ]
            EmailMessage[] threadMessages;
            MessagePartHeader[] headers;
            IList<Message> gmailThreadMessages;
            string messageBody;
            byte[] bytes;
            DateTime date;
            string _date = "";

            // Send a HTTP request to the relevant REST endpoint
            // to fetch a list of the IDs for all email-threads
            IList<Thread> gmailThreads = this.service.Users.Threads.List(userId).Execute().Threads; 
            IList<EmailMessage[]> threads = new List<EmailMessage[]>();
            
            foreach(Thread _t in gmailThreads)
            // Go through each thread
            {
                // HTTP request to fetch all messages based on a threadId 
                gmailThreadMessages = this.service.Users.Threads.Get(userId, _t.Id).Execute().Messages;
                threadMessages = new EmailMessage[gmailThreadMessages.Count];

                for (int i = 0; i < gmailThreadMessages.Count; i++)
                // Go through each message in the thread
                {
                    messageBody = "";
                
                    if (gmailThreadMessages[i].Payload.Parts != null)
                    // The retrieved body can be chunked into Payload.Parts
                    {
                        foreach (MessagePart part in gmailThreadMessages[i].Payload.Parts)
                        {
                            if(part.Body.Data == null) continue;

                            // Each body chunk is furthermore obfuscated with URLencoded base64
                            try
                            {

                                //bytes = WebEncoders.Base64UrlDecode( part.Body.Data );
                                bytes   = this.decodeBase64Url( part.Body.Data ); 
                                messageBody += System.Text.Encoding.UTF8.GetString( bytes );
                            }
                            catch (Exception e) { Console.Error.WriteLine(e); } 
                        }
                    }
                    else
                    // Or given in one chunk
                    {
                        try
                        {
                            //bytes = WebEncoders.Base64UrlDecode( gmailThreadMessages[i].Payload.Body.Data );
                            bytes   = this.decodeBase64Url( gmailThreadMessages[i].Payload.Body.Data ); 
                            messageBody = System.Text.Encoding.UTF8.GetString( bytes );
                        }
                        catch (Exception e){ Console.Error.WriteLine(e); }
                    }

                    // Determine the relevant header values and create a new EmaillMessage object
                    headers = new MessagePartHeader[gmailThreadMessages[i].Payload.Headers.Count]; 
                    gmailThreadMessages[i].Payload.Headers.CopyTo(headers,0);

                    try 
                    // Attempt to sanitize the date and parse it
                    { 
                        _date = Array.Find( headers, (MessagePartHeader h) => h.Name == "Date" ).Value;
                        _date = Regex.Replace(_date, Regex.Escape("(") + "(UTC|PST).*" + Regex.Escape(")") + ".*", "");
                        date = DateTime.Parse(_date); 
                    }
                    catch(Exception e)
                    { 
                        Console.Error.WriteLine(e);
                        if (e is FormatException)
                            Console.Error.WriteLine(_date);
                        date = new DateTime();
                    }
                
                    threadMessages[i] = new EmailMessage(
                       Array.Find( headers, (MessagePartHeader h) => h.Name == "Subject" ).Value, 
                       Array.Find( headers, (MessagePartHeader h) => h.Name == "From" ).Value, 
                       date, 
                       messageBody
                    );
                }
                
                threads.Add(threadMessages);
            }
            
            return threads;
        }
    }
        
}
