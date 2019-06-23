namespace Narfu.Tests
{
    public class TestBase
    {
        public NarfuService GetService()
        {
            return new NarfuService(null);
        }
    }
}