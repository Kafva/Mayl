using Xunit;
using System;
using System.IO;

namespace Gmail.UnitTests
{
    struct Constant
    {
        public const string threadId = "1772f0c19a53628c"; 
        public const string userId = "<TODO>"; 
        public const string subject = "Re: [Kafva/Netrix] new comment (#1)";
        public const string tag = "STARRED";
    }

    public class TestsFixture : IDisposable
    // Superclass for a shared initalization context between all tests
    {
        public GmailAPI gmailAPI;
    
        public TestsFixture ()
        // Global init, only called once
        {
            // `cd` out of Tests/bin/Debug/net5.0/
            System.IO.Directory.SetCurrentDirectory("../../../../");
            this.gmailAPI = new GmailAPI(GmailAPI.scopes, Constant.userId);
            
            // Setup a dummy thread with only the STARRED label
            gmailAPI.updateThreadStatus(Constant.threadId, UpdateAction.ARCHIVE);
            gmailAPI.updateThreadStatus(Constant.threadId, UpdateAction.TAG, Constant.tag);
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
            var messages = this.gmailAPI.fetchThreadMessages(Constant.threadId);
            Assert.Equal(Constant.subject, messages[0].subject);
        }
        
        [Fact]
        private void Test_fetchThreadMessages_Exception()
        {
            var messages = this.gmailAPI.fetchThreadMessages("1111");
            Assert.Empty(messages);
        }
        
        [Fact]
        private void Test_getThreadsFromLabel()
        {
            var threads = this.gmailAPI.getThreadsFromLabel(Constant.tag);
            Assert.Equal(Constant.subject, threads[0].emails[0].subject);
        }
        
        [Fact]
        private void Test_getThreadsFromLabel_Exception()
        {
            var threads = this.gmailAPI.getThreadsFromLabel("NON_EXISTANT");
            Assert.Empty(threads);
        }
        
        [Fact]
        private void Test_getLabels()
        {
            var threads = this.gmailAPI.getLabels();
            Assert.NotEmpty(threads);
        }
    }
}
