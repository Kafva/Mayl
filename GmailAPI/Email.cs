using System;

namespace Gmail
{
    public class EmailMessage
    {
        public string subject {get; set;}
        public string body {get; set;}
        public string sender {get; set;}
        //public string htmlBody {get; set;}
        
        public DateTime date {get; set;}
        
        //******************************//
        public EmailMessage(string subject, string sender, DateTime date, string body)
        {
            this.subject  = subject;
            this.sender   = sender;
            this.date     = date;
            this.body     = body;
            
        }
        
        //private void setBody(string body)
        //{
        //    // Split raw text and HTML (if any)
        //    string[] res = Regex.Split(body, "<!DOCTYPE HTML>", RegexOptions.IgnoreCase);
        //    if(res.Length > 1)
        //    {
        //        this.body = res[0];
        //        this.htmlBody = "<!DOCTYPE HTML>" + res[1];
        //    }
        //    else
        //    {
        //        this.body     = body;
        //        this.htmlBody = "";
        //    }
        //}
    }
    
    public class EmailThread
    {
        public const int SNIPPET_SIZE = 100;
        
        public string threadId {get; set;} 
        public string snippet {get; set;} 
        public EmailMessage[] emails {get; set;}
        
        public EmailThread(string threadId, string snippet, EmailMessage[] emails)
        {
            this.threadId = threadId;
            this.emails   = emails;
            this.snippet  = snippet.Length > SNIPPET_SIZE ? 
                snippet.Substring(0, EmailThread.SNIPPET_SIZE) : 
                snippet;
        }
    }
}
