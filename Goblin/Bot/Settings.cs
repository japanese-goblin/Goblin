namespace Goblin.Bot
{
    public static class Settings
    {
        public const string ConfirmationToken = "***REMOVED***";
        public const string AccessToken = "***REMOVED***";
        public static long[] Developers = { ***REMOVED*** }; // TODO: вынести в бд?

        static Settings()
        {
            Vk.Api.SetAccessToken(AccessToken);
        }
    }
}