namespace Goblin.Application.Extensions
{
    public static class StringExtensions
    {
        public static string ToUpperFirstLetter(this string str)
        {
            return char.ToUpper(str[0]) +
                    str.Substring(1).ToLower();
        }
    }
}