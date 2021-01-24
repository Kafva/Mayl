using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;

class GmailAPIRequest
{
    private GmailService service;
    public GmailAPIRequest(string ApplicationName, UserCredential credentials)
    {
        // Create Gmail API service.
        this.service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credentials,
            ApplicationName = ApplicationName,
        });
            
    }

    public void getLabels()
    {
        // Define parameters of request.
        UsersResource.LabelsResource.ListRequest request = this.service.Users.Labels.List("me");

        // List labels.
        IList<Label> labels = request.Execute().Labels;
        Console.WriteLine("Labels:");
        if (labels != null && labels.Count > 0)
        {
            foreach (var labelItem in labels)
            {
                Console.WriteLine("{0}", labelItem.Name);
            }
        }
        else
        {
            Console.WriteLine("No labels found.");
        }
        Console.Read();
    }
    


}