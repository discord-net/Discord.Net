namespace Discord.Commands
{
    public static class MessageExtensions
    {
        public static bool HasCharPrefix(this IMessage msg, char c, ref int argPos)
        {
            var text = msg.Content;
            if (text.Length > 0 && text[0] == c)
            {
                argPos = 1;
                return true;
            }
            return false;
        }
        public static bool HasStringPrefix(this IMessage msg, string str, ref int argPos)
        {
            var text = msg.Content;
            if (text.StartsWith(str))
            {
                argPos = str.Length;
                return true;
            }
            return false;
        }
        public static bool HasMentionPrefix(this IMessage msg, IUser user, ref int argPos)
        {
            var text = msg.Content;
            if (text.Length <= 3 || text[0] != '<' || text[1] != '@') return false;

            int endPos = text.IndexOf('>');
            if (endPos == -1) return false;
            if (text.Length < endPos + 2 || text[endPos + 1] != ' ') return false; //Must end in "> "

            ulong userId;
            if (!MentionUtils.TryParseUser(text.Substring(0, endPos + 2), out userId)) return false;
            if (userId == user.Id)
            {
                argPos = endPos + 2;
                return true;
            }
            return false;
        }
    }
}