using System.Text.RegularExpressions;
using VkNet.Model;

namespace Goblin.Application.Extensions
{
    public static class MessageExtensions
    {
        public static string[] GetCommandInfo(this Message msg)
        {
            if(msg.FromId != msg.PeerId)
            {
                var match = Regex.Match(msg.Text, @"\[club\d+\|.*\] (.*)").Groups[1].Value;
                if(!string.IsNullOrEmpty(match))
                {
                    msg.Text = match;
                }
            }

            return msg.Text.Trim().Split(' ', 2);
        }
    }
}