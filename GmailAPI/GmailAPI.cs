#define _DEBUG
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace Gmail
{
    public interface IGmailAPI<T>
    // Let the GmailAPI class inherit from the interface
    {
        public List<string> getLabels(string userId);

        List<T> getThreadsFromLabel(string userId, string label, bool fetchBody=true);
        public EmailMessage[] fetchThreadMessages(string userId, string threadId, bool fetchBody=true);
    }

    public class GmailAPI : IGmailAPI<EmailThread>
    {
        
        public static readonly string[] scopes = { 
            GmailService.Scope.GmailReadonly, 
            GmailService.Scope.MailGoogleCom, 
            GmailService.Scope.GmailModify 
        };
            
        public const string secretPath   = "./secret/";
        public const string tokenDirname = "token";
        public const string credFilename = "credentials.json";
        private GmailService service;
        private UserCredential credentials;
        
        //**********************************************//
        public GmailAPI(string ApplicationName, string[] scopes)
        {
            // Open a browser session to authenticate with the app if no tokens.json directory exists
            try { this.setupCredentials(); }
            catch (Exception e) 
            { 
                Console.Error.WriteLine(e); 
                Environment.Exit(1); 
            }
            
            // Create Gmail API service.
            this.service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });
        }
        private void setupCredentials()
        {
            if ( Directory.Exists(GmailAPI.secretPath) )
            {
                if ( !Directory.Exists(GmailAPI.secretPath + GmailAPI.tokenDirname) )
                {
                    Console.WriteLine(string.Format("Initialising authorization for new token ({0})", GmailAPI.secretPath + GmailAPI.tokenDirname));
                }
                else { Console.WriteLine("Using existing credentials at " + GmailAPI.secretPath + GmailAPI.tokenDirname ); }
                
                var stream = new FileStream(GmailAPI.secretPath + GmailAPI.credFilename, FileMode.Open, FileAccess.Read);
                
                // The token directory stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                // I.e. we only need to authorize once in the browser after which all API communcation
                // will be done server side
                this.credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    GmailAPI.scopes,
                    "user",
                    System.Threading.CancellationToken.None,
                    new FileDataStore(GmailAPI.secretPath +  GmailAPI.tokenDirname, true)
                ).Result;
            }
            else { throw new DirectoryNotFoundException("Missing path: " + GmailAPI.secretPath); }
        }
        
        public List<string> getLabels(string userId)
        {
            List<Label> gmailLabels = new List<Label>(); 
            List<string> labels = new List<string>();
            
            try 
            { 
                gmailLabels = (List<Label>)this.service.Users.Labels.List(userId).Execute().Labels; 
            }
            catch (Exception e){ Console.Error.WriteLine(e); }

            gmailLabels.ForEach( (Label l) => labels.Add(l.Name) );
            return labels;
        }
        public List<EmailThread> getThreadsFromLabel(string userId, string label, bool fetchBody=true)
        {
            // With all threads fetched retrieve the metadata and payload of the corresponding messages
            List<EmailThread> threads = new List<EmailThread>();
            
            // NOTE that the Messages.List() endpoint does NOT return metadata about messages, for that
            // we need to request each message individually...

            string nextToken = "";
            List<Thread> gmailThreads = new List<Thread>();
            UsersResource.ThreadsResource.ListRequest request; 
            ListThreadsResponse response;
            
            do
            // Keep on sending requests (async) until a response without a NextPageToken is
            // encountered, at that point we have fetched all threads
            {
                request = this.service.Users.Threads.List(userId);

                // Limit search to the specified label
                // We can specify an array but then we will only
                // recieve messages matching ALL the labels (&& style)
                request.LabelIds = label;

                // Set the nextToken to fetch the next batch of results
                if (nextToken != null) request.PageToken = nextToken;

                try 
                { 
                    response = request.Execute(); 
                    nextToken = response.NextPageToken;
                    gmailThreads.AddRange( response.Threads );
                }
                catch (Exception e){ Console.Error.WriteLine(e); return threads; }
                
            } while ( nextToken != null );


            Parallel.ForEach( gmailThreads , (thread) =>
            // Parse the messages from each thread in parallell
            {
                var threadMessages = fetchThreadMessages(userId, thread.Id, fetchBody);
                if (threadMessages[0] != null)
                {
                    threads.Add( new EmailThread( thread.Id, thread.Snippet, threadMessages )  );
                }
            });
             
            return threads;
        } 
        public EmailMessage[] fetchThreadMessages(string userId, string threadId, bool fetchBody=true)
        {
            // Initalize for base case return value
            List<Message> gmailThreadMessages = new List<Message>();
            
            #if _DEBUG
                Console.WriteLine(string.Format("Fetching: {0}", threadId ));
            #endif
            try 
            { 
                // HTTP request to fetch all messages based on a threadId (fairly time consuming)
                gmailThreadMessages = (List<Message>)this.service.Users.Threads.Get(userId, threadId).Execute().Messages;
            }
            catch (Exception e){ Console.Error.WriteLine(e); }
        
            #if _DEBUG
                Console.WriteLine(string.Format("Completed: {0}", threadId));
            #endif
            
            return parseMessages(gmailThreadMessages, fetchBody);;
        }
        private EmailMessage[] parseMessages(List<Message> messages, bool fetchBody=true)
        {
            EmailMessage[] threadMessages = new EmailMessage[messages.Count];
            MessagePartHeader[] headers;
            string messageBody = "";
            DateTime date;
            int index = 0;
            
            foreach (Message msg in messages)
            // Go through each message in the thread
            {
                // Extract the body
                if(fetchBody) messageBody = extractBodyFromMessage( msg );

                // Determine the relevant header values and create a new EmailMessage object
                headers = new MessagePartHeader[msg.Payload.Headers.Count]; 
                msg.Payload.Headers.CopyTo(headers,0);

                date = parseDateFromEmailHeaders(headers);
            
                // Incrment the index upon adding a new message
                threadMessages[index++] = new EmailMessage(
                   Array.Find( headers, (MessagePartHeader h) => h.Name == "Subject" ).Value, 
                   Array.Find( headers, (MessagePartHeader h) => h.Name == "From" ).Value, 
                   date, 
                   messageBody
                );
            }

            return threadMessages;
        }

        //***********************************************//
        private DateTime parseDateFromEmailHeaders(MessagePartHeader[] headers)
        {
            string _date = "";
            DateTime date;

            try 
            // Attempt to sanitize the date and parse it
            { 
                _date = Array.Find( headers, (MessagePartHeader h) => h.Name == "Date" ).Value;
                _date = Regex.Replace(_date, Regex.Escape("(") + "(UTC|PST|GMT).*" + Regex.Escape(")") + ".*", "");
                date = DateTime.Parse(_date); 
            }
            catch(Exception e)
            { 
                Console.Error.WriteLine(e);
                if (e is FormatException)
                    Console.Error.WriteLine(_date);
                date = new DateTime();
            }

            return date;
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
        private string extractBodyFromMessage(Message msg)
        {
            byte[] bytes;
            string messageBody = "";

            if (msg.Payload.Parts != null)
            // The retrieved body can be chunked into Payload.Parts
            {
                foreach (MessagePart part in msg.Payload.Parts)
                {
                    if(part.Body.Data == null) continue;

                    // Each body chunk is furthermore obfuscated with URLencoded base64
                    try
                    {
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
                    bytes   = this.decodeBase64Url( msg.Payload.Body.Data ); 
                    messageBody = System.Text.Encoding.UTF8.GetString( bytes );
                }
                catch (Exception e){ Console.Error.WriteLine(e); }
            }

            return messageBody;
        }
    }
}
