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

namespace Gmail
{
    public enum UpdateAction
    {
        UNTAG, TAG, ARCHIVE
    }
    
    public interface IGmailAPI<T>
    // Let the GmailAPI class inherit from the interface
    {
        public List<string> getLabels(string userId);
        public List<string> updateThreadStatus(string userId, string threadId, UpdateAction updateAction, string tag="");
        List<T> getThreadsFromLabel(string userId, string label, bool fetchBody=true);
        public EmailMessage[] fetchThreadMessages(string userId, string threadId, bool fetchBody=true);
    }


    public class GmailAPI : IGmailAPI<EmailThread>
    {
        
        public static readonly string[] scopes = { 
            GmailService.Scope.GmailModify, 
            GmailService.Scope.GmailLabels, 
            GmailService.Scope.MailGoogleCom,
        };
        
        private const string UNREAD_LABEL = "UNREAD";
        private const string TRASH_LABEL = "TRASH";
        private const int maxLabelCount = 50;
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
            // To reinitalise the credentials, remove secrets/credentials.json and once this method is called
            // a prompt will be generated SERVER-SIDE for authn of the app

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
        
        public List<string> updateThreadStatus(string userId, string threadId, UpdateAction updateAction, string tag="")
        {
            // Note that every thread belongs to exactly 1 category, archiving or trashing a message
            // won't remove the category, adding a new category will remove the old one however

            bool ret = false;
            List<string> arr = new List<string>();
            
            switch(updateAction)
            {
                case UpdateAction.ARCHIVE:
                    // Remove all tags from the thread to 'archive' it 
                    ret = this.removeLabelsFromThread(userId, threadId, 
                            this.getLabelsOfThread(userId, threadId) );
                    break;
                case UpdateAction.UNTAG:
                    // Remove the given tag (should it exist)
                    if( !this.getLabels(userId).Contains(tag) )
                        Console.Error.WriteLine("Unknown tag: " + tag);
                    else
                    {
                        arr.Add(tag);
                        ret = this.removeLabelsFromThread(userId, threadId, arr);
                    }
                    break;
                case UpdateAction.TAG:
                    // Add the given tag (should it exist)
                    if( !this.getLabels(userId).Contains(tag) )
                        Console.Error.WriteLine("Unknown tag: " + tag);
                    else
                    {
                        arr.Add(tag);
                        ret = this.addLabelsToThread(userId, threadId, arr);
                    }
                    break;
            }

            var res = this.getLabelsOfThread(userId,threadId);
            return res;
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
                try
                {
                    if (threadMessages[0] != null)
                    {
                        threads.Add( new EmailThread( thread.Id, thread.Snippet, threadMessages )  );
                    }
                }
                catch (Exception e){ Console.Error.WriteLine(e); }
            });
             
            return threads;
        } 
        public EmailMessage[] fetchThreadMessages(string userId, string threadId, bool fetchBody=true)
        {
            return parseMessages( fetchGmailThreadMessages(userId, threadId), fetchBody);;
        }
        private EmailMessage[] parseMessages(List<Message> messages, bool fetchBody=true)
        {
            EmailMessage[] threadMessages = new EmailMessage[messages.Count];
            MessagePartHeader[] headers;
            // Firs index contains raw text and the second contains HTML (if any)
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
        
        public bool addLabelsToThread(string userId, string threadId, List<string> labelIds)
        {
            // The /users/{userId}/threads/{id}/modify endpoint takes labels to add or remove
            // as a list argument via the POST body, to 'archive' a message we remove all tags
            // and to 'trash' a message we add the TRASH tag (this will prevent it from
            // appearing in any other tags its still a member of)
            ModifyThreadRequest body = new ModifyThreadRequest();
            body.AddLabelIds = labelIds;
            
            // Create and execute the HTTP request
            UsersResource.ThreadsResource.ModifyRequest request = this.service.Users.Threads
                .Modify(body, userId, threadId);            
            try
            {
                request.Execute();
                return true;
            }
            catch(Google.GoogleApiException e)
            { 
                Console.Error.WriteLine(e); 
                return false;
            }
        } 

        private bool removeLabelsFromThread(string userId, string threadId, List<string> labelIds)
        {
            ModifyThreadRequest body = new ModifyThreadRequest();
            body.RemoveLabelIds = labelIds;
            
            // Create and execute the HTTP request
            UsersResource.ThreadsResource.ModifyRequest request = this.service.Users.Threads
                .Modify(body, userId, threadId);  
            try
            {
                request.Execute();
                return true;
            }
            catch(Google.GoogleApiException e)
            { 
                Console.Error.WriteLine(e); 
                return false;
            }
        } 
        
        private List<string> getLabelsOfThread(string userId, string threadId)
        {
            // Note that if a label is missing from at least one message it won't
            // be included in the result!

            List<string> labels = new List<string>();
            string[]    _labels = new string[maxLabelCount];
            
            List<Message> gmailThreadMessages = fetchGmailThreadMessages(userId, threadId); 
            foreach( Message m in gmailThreadMessages )
            // We will recieve an error if we try to remove a label that does not exist for the
            // the thread and therefore need to determine which labels are shared between all messages
            {
                if (labels.Count == 0)
                {
                    labels.AddRange( m.LabelIds );
                    labels.CopyTo(_labels);
                    continue;
                }

                foreach (string l in _labels)
                // Each label not present in '_labels' should be removed from 'labels'
                // with this approach we are least likely to throw an exception
                // We iterate over the static _labels instead of the list which is being manipulated 
                {
                    if ( l == null ) continue;
                    
                    if ( ! m.LabelIds.Contains(l) )
                    { 
                        #if _DEBUG
                            Console.WriteLine(String.Format("Missing label in (message={0}) : '{1}' in (thread={2})", m.Id, l, threadId));
                        #endif
                        labels.Remove(l);
                    }
                }

                labels.CopyTo(_labels);
            }
            #if _DEBUG
                Console.WriteLine("Labels for " + threadId);
                foreach(string l in labels){ Console.WriteLine("\t"+l); }
            #endif
            
            return labels;
        }

        private List<Message> fetchGmailThreadMessages(string userId, string threadId)
        {
            // Initalize for base case return value
            List<Message> gmailThreadMessages = new List<Message>();
            
            #if _DEBUG
                Console.WriteLine(string.Format("Fetching: {0}", threadId ));
            #endif
            try 
            { 
                // HTTP request to fetch all messages based on a threadId (fairly time consuming)
                gmailThreadMessages = (List<Message>)this.service.Users.Threads.Get(
                    userId, threadId).Execute().Messages;
            }
            catch (Exception e){ Console.Error.WriteLine(e); }
        
            #if _DEBUG
                Console.WriteLine(string.Format("Completed: {0}", threadId));
            #endif

            return gmailThreadMessages;
        }
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
