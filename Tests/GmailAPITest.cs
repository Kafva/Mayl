using Xunit;
using System;
using System.IO;

namespace Gmail.UnitTests
{
    public class GmailAPI_Test_Thread_Fetch
    {
        private void setupEnviroment()
        {
            // `cd` out of Tests/bin/Debug/net5.0/
            System.IO.Directory.SetCurrentDirectory("../../../../");
        }
        
        [Fact]
        public void threadTest()
        {
            this.setupEnviroment();
            var gmailAPI = new GmailAPI("Mayler", GmailAPI.scopes); 
            
            var threads = gmailAPI.getThreads("me");
            
            Console.WriteLine(threads[0][0].subject);
            Assert.Equal("Re: [Kafva/Netrix] new comment (#1)", threads[0][0].subject);
        }
    }
}
