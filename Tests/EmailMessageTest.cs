using Xunit;
using Gmail;

namespace Gmail.UnitTests
{
    public class EmailMessage_Test
    {
        [Fact]
        public void myTest()
        {
            int x = EmailMessage.do_();
            Assert.Equal(1,x);
        }
    }
}
