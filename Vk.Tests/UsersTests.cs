using System;
using System.IO;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Vk.Tests
{
    public class UsersTests : TestBase
    {
        [Fact]
        public async void Get_CorrectId_UsersList()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("data/users/get.json"));

                var result = await GetApi().Users.Get(new long[] { 1, 2 });

                httpTest.ShouldHaveCalled($"{VkApi.EndPoint}*")
                        .WithVerb(HttpMethod.Post)
                        .WithRequestBody("user_ids=*")
                        .Times(1);

                Assert.NotEmpty(result);
                Assert.True(result.Length == 2);
                var user = result[0];
                Assert.True(user.Id == 1);
                Assert.NotEmpty(new[] { user.FirstName, user.LastName, user.Photo200Orig });
            }
        }

        [Fact]
        public async void Get_EmptyIds_ThrowsError()
        {
            await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
                                                               await GetApi().Users.Get(new long[] { }));
        }
    }
}