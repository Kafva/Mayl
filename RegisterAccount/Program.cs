using System;
using Gmail;
using Utils;

namespace RegisterAccount
{
    class Program
    {
        public static void exitMsg(string str, int code=1) { System.Console.WriteLine(str); System.Environment.Exit(code); }
        static void Main(string[] args)
        {
            string helpStr = string.Format("{0} [-h] [-t] <name@gmail.com>\n\t-t\t Test the connection for a given account or for each Gmail account listed in ./secret/accounts.txt",
                System.AppDomain.CurrentDomain.FriendlyName);
            if (args.Length >= 1)
            { 
                // Switch to the project root
                System.IO.Directory.SetCurrentDirectory("..");
                
                switch(args[0])
                {
                    case "-h":
                        exitMsg(helpStr);
                        break;
                    case "-t":
                        if (args.Length >= 2)
                            testAPI(args[1]);
                        else
                        {
                            foreach(var account in Util.getAccounts())
                                testAPI(account);
                        }
                        break;
                    default:
                        if ( args[0].Contains("@") )
                        // Register a new account
                        {
                            var api = new GmailAPI(GmailAPI.scopes, args[0]); 
                            Console.WriteLine("Test the connection with: `dotnet run -t " + args[0] + "`");
                            Console.WriteLine("If the connection was successful add the account to: " + Util.ACCOUNTS_FILE);
                        }
                        else { exitMsg("Provided argument does not look like an email address (no '@' found)"); }
                        break;
                }
                
            }            
            else { exitMsg(helpStr); }
        }
        private static void testAPI(string account)
        {
            Console.WriteLine(string.Format("=========== {0} =============", account));
            var api = new GmailAPI(GmailAPI.scopes, account); 
            Console.WriteLine( (api.getLabels().Count != 0) ? 
                "Connection successful" : "Connection failed" );
            
        }
    
    }
}


