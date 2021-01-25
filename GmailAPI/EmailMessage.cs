using System;

namespace Gmail
{
    public class EmailMessage
    {
        public string subject;
        public string body;
        public string sender;
        
        public DateTime date;
        
        //******************************//
        public EmailMessage(string subject, string sender, DateTime date, string body)
        {
            this.subject = subject;
            this.sender  = sender;
            this.date    = date;
            this.body    = body;
        }
        
        public static int do_()
        {
            return 1;
        }
    }

}