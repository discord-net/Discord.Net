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
            //str = str + ' ';
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
            string mention = user.Mention + ' ';
            if (text.StartsWith(mention))
            {
                argPos = mention.Length;
                return true;
            }
            string nickMention = user.NicknameMention + ' ';
            if (text.StartsWith(mention))
            {
                argPos = nickMention.Length;
                return true;
            }
            return false;
        }
    }
}