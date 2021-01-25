using System;
using Gmail;

namespace ConsoleDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory("../");
            var gmailAPI = new GmailAPI("Mayler", GmailAPI.scopes); 
            
            var threads = gmailAPI.getThreads("me");
            
            Console.WriteLine(threads[0][0].subject);
            
        }
    }
}
