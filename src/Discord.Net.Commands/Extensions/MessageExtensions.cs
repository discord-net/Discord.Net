using System;

namespace Discord.Commands
{
    public static class MessageExtensions
    {
        public static bool HasCharPrefix(this IUserMessage msg, char c, ref int argPos)
        {
            var text = msg.Content;
            if (text.Length > 0 && text[0] == c)
            {
                argPos = 1;
                return true;
            }
            return false;
        }
        public static bool HasStringPrefix(this IUserMessage msg, string str, ref int argPos, StringComparison comparisonType = StringComparison.Ordinal)
        {
            var text = msg.Content;
            if (text.StartsWith(str, comparisonType))
            {
                argPos = str.Length;
                return true;
            }
            return false;
        }
        public static bool HasMentionPrefix(this IUserMessage msg, IUser user, ref int argPos)
        {
            var text = msg.Content;
            if (text.Length <= 3 || text[0] != '<' || text[1] != '@') return false;

            int endPos = text.IndexOf('>');
            if (endPos == -1) return false;
            if (text.Length < endPos + 2 || text[endPos + 1] != ' ') return false; //Must end in "> "

            ulong userId;
            if (!MentionUtils.TryParseUser(text.Substring(0, endPos + 1), out userId)) return false;
            if (userId == user.Id)
            {
                argPos = endPos + 2;
                return true;
            }
            return false;
        }
        public static bool HasCharSuffix(this IUserMessage msg, char c)
            => msg.Content.Length > 0 && msg.Content[msg.Content.Length - 1] == c;
        public static bool HasStringSuffix(this IUserMessage msg, string str, StringComparison comparisonType = StringComparison.Ordinal)
            => msg.Content.EndsWith(str, comparisonType);
        
        public static bool HasMentionSuffix(this IUserMessage msg, IUser user)
        {
            var text = msg.Content;
            if (text.Length <= 3 || text[text.Length - 1] != '>') return false;

            int iniPos = text.LastIndexOf('<');
            if (iniPos == -1) return false;
            if (!MentionUtils.TryParseUser(text.Substring(iniPos, text.Length - iniPos), out ulong userId)) return false;
            if (user.Id == userId) return true;

            return false;
        }
    }
}
