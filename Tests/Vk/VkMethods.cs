//using System.Threading.Tasks;
//using Vk;
//using Xunit;

//namespace Tests.Vk
//{
//TODO: переделать тесты
//    public class VkMethods
//    {
//        [Fact]
//        public async Task GetUsername_Valid_String()
//        {
//            var result = await VkApi.Users.Get(1);
//            Assert.False(string.IsNullOrEmpty(result.ToString()));
//        }

//        [Fact]
//        public async Task GetUsername_NotValid_EmptyString()
//        {
//            var result = await VkApi.Users.Get(0);
//            Assert.True(string.IsNullOrEmpty(result.ToString()));
//        }

//        [Fact]
//        public async Task SendMessage_Correct()
//        {
//            await VkApi.Messages.Send(***REMOVED***, "test solo", new[] { "photo-146048760_456239017" });
//            await VkApi.Messages.Send(new long[] { ***REMOVED***, ***REMOVED*** }, "test multi",
//                                      new[] { "photo-146048760_456239017" });
//        }
//    }
//}

