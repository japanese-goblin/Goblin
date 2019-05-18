using System.IO;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Vk.Tests
{
    public class UsersTests : TestBase
    {
        [Fact(DisplayName = "users.get")]
        public async void Get()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("data/users.get.json"));

                var api = new VkApi("token");
                var result = await api.Users.Get(new long[] { 1, 2 });

                httpTest.ShouldHaveCalled($"{VkApi.EndPoint}*")
                        .WithVerb(HttpMethod.Post)
                        .Times(1);

                Assert.NotEmpty(result);
                Assert.True(result.Length == 2);
                var user = result[0];
                Assert.True(user.Id == 1);
                Assert.NotEmpty(new[] { user.FirstName, user.LastName, user.Photo200Orig });
            }
        }
    }
}