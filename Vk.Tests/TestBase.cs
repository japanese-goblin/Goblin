namespace Vk.Tests
{
    public class TestBase
    {
        public const string Token = "super-secret-token";

        public VkApi GetApi()
        {
            return new VkApi(Token);
        }
    }
}