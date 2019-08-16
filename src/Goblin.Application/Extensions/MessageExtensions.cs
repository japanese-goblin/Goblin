using System.Linq;
using System.Text.RegularExpressions;
using VkNet.Model;

namespace Goblin.Application.Extensions
{
    public static class MessageExtensions
    {
        public static string GetCommand(this Message msg)
        {
            msg = GetCommandInfo(msg);

            return msg.Text.Split(' ', 2)[0].ToLower();
        }

        public static string[] GetCommandParameters(this Message msg)
        {
            msg = GetCommandInfo(msg);

            var @params = msg.Text.Split(' ');
            if(@params.Length == 0)
            {
                return new[] { string.Empty };
            }

            return @params.Skip(1).ToArray();
        }

        private static Message GetCommandInfo(Message msg)
        {
            if(msg.FromId != msg.PeerId)
            {
                var match = Regex.Match(msg.Text, @"\[club\d+\|.*\] (.*)").Groups[1].Value;
                if(!string.IsNullOrEmpty(match))
                {
                    msg.Text = match;
                }
            }

            msg.Text = msg.Text.Trim();

            return msg;
        }
    }
}