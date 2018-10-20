using System;

namespace Discord.Commands
{
    /// <summary>
    ///     Provides extension methods for <see cref="IUserMessage" /> that relates to commands.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        ///     Gets whether the message starts with the provided character.
        /// </summary>
        /// <param name="msg">The message to check against.</param>
        /// <param name="c">The char prefix.</param>
        /// <param name="argPos">References where the command starts.</param>
        /// <returns>
        ///     <c>true</c> if the message begins with the char <paramref name="c"/>; otherwise <c>false</c>.
        /// </returns>
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
        /// <summary>
        ///     Gets whether the message starts with the provided string.
        /// </summary>
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
        /// <summary>
        ///     Gets whether the message starts with the user's mention string.
        /// </summary>
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
    }
}
