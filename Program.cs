using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mayler
{
    class Program
    {
        // NOTE that a random client-side port is used to launch the app and the redirect-uri
        // cannot thus not contain a port number for the authorization flow to work (since it will be random each time)
        // https://github.com/googleapis/google-api-dotnet-client/issues/1316

        static string[] scopes = { GmailService.Scope.GmailReadonly };
        static string applicationName = "Mayler";

        static void Main(string[] args)
        {
            UserCredential credentials;

            using (var stream = new FileStream("./secret/credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                // I.e. we only need to authorize once in the browser after which all API communcation
                // will be done server side
                string credPath = "./secret/token.json";
                credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
            
            var gmailAPI = new GmailAPIRequest(applicationName, credentials);
            gmailAPI.getLabels();
        }
    }
}