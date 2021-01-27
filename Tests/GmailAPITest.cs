using Xunit;
using System;
using System.IO;

namespace Gmail.UnitTests
{
    public class TestsFixture : IDisposable
    // Superclass for a shared initalization context between all tests
    {
        public GmailAPI gmailAPI;
        public TestsFixture ()
        // Global init, only called once
        {
            // `cd` out of Tests/bin/Debug/net5.0/
            System.IO.Directory.SetCurrentDirectory("../../../../");
            this.gmailAPI = new GmailAPI("Mayler", GmailAPI.scopes);
        }

        // Global deconstructor, called once during the entire test
        public void Dispose(){}
    } 
    public class Test : IClassFixture<TestsFixture>
    {
        public GmailAPI gmailAPI;
        
        public Test (TestsFixture t)
        {
            this.gmailAPI = t.gmailAPI; 
        }
        
        [Fact]
        private void Test_fetchThreadMessages()
        {
            var messages = this.gmailAPI.fetchThreadMessages("me", "1772f0c19a53628c");
            Assert.Equal("Re: [Kafva/Netrix] new comment (#1)", messages[0].subject);
        }
        
        [Fact]
        private void Test_fetchThreadMessages_Exception()
        {
            var messages = this.gmailAPI.fetchThreadMessages("me", "1111");
            Assert.Empty(messages);
        }
        
        [Fact]
        private void Test_getThreadsFromLabel()
        {
            var threads = this.gmailAPI.getThreadsFromLabel("me", "STARRED");
            Assert.Equal("Verify Email Address for Discord", threads[0].emails[0].subject);
        }
        
        [Fact]
        private void Test_getThreadsFromLabel_Exception()
        {
            var threads = this.gmailAPI.getThreadsFromLabel("me", "NON_EXISTANT");
            Assert.Empty(threads);
        }
        
        [Fact]
        private void Test_getLabels()
        {
            var threads = this.gmailAPI.getLabels("me");
            Assert.NotEmpty(threads);
        }
        

    }
}
