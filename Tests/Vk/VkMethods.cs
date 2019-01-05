using System.Threading.Tasks;
using Vk;
using Xunit;

namespace Tests.Vk
{
    public class VkMethods
    {
        [Fact]
        public async Task GetUsername_Valid_String()
        {
            var result = await Api.Users.GetUserName(1);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task GetUsername_NotValid_EmptyString()
        {
            var result = await Api.Users.GetUserName(0);
            Assert.True(string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task SendMessage_Correct()
        {
            await Api.Messages.Send(***REMOVED***, "test solo", new[] {"photo-146048760_456239017"});
            await Api.Messages.Send(new long[] { ***REMOVED***, ***REMOVED*** }, "test multi",
                new[] { "photo-146048760_456239017" });
        }
    }
}
