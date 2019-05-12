using Xunit;

namespace Vk.Tests
{
    public class UsersTests : TestBase
    {
        [Fact(DisplayName = "users.get")]
        public async void Get()
        {
            var result = await GetApi("users.get").Users.Get(new long[] { 1, 2 });
            Assert.NotEmpty(result);
            var user = result[0];
            Assert.True(user.Id == 1);
            Assert.NotEmpty(new[] { user.FirstName, user.LastName, user.Photo200Orig });
        }
    }
}