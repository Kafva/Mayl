using System;
using Google.Apis.Gmail.v1;

namespace Mayler
{
    class Program
    {
        // NOTE that a random client-side port is used to launch the app and the redirect-uri
        // cannot thus not contain a port number for the authorization flow to work (since it will be random each time)
        // https://github.com/googleapis/google-api-dotnet-client/issues/1316

        static string applicationName = "Mayler";
        static string[] scopes = { 
            GmailService.Scope.GmailReadonly, 
            GmailService.Scope.MailGoogleCom, 
            GmailService.Scope.GmailModify 
        };

        static void Main(string[] args)
        {
            var g = new GmailAPI(applicationName, scopes);
            var threads = g.getThreads("me");
            
            Console.WriteLine(threads[0][0].subject);
        }
    }
}