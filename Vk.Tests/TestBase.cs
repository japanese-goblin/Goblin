namespace Vk.Tests
{
    public class TestBase
    {
        public VkApi GetApi(string data)
        {
            return new VkApi("super-secret-token");
        }
    }
}