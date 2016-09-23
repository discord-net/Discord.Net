using System;
using System.Globalization;

namespace Discord
{
    public static class MentionUtils
    {
        //If the system can't be positive a user doesn't have a nickname, assume useNickname = true (source: Jake)
        public static string MentionUser(ulong id) => MentionsHelper.MentionUser(id, true);
        public static string MentionChannel(ulong id) => MentionsHelper.MentionChannel(id);
        public static string MentionRole(ulong id) => MentionsHelper.MentionRole(id);

        /// <summary> Parses a provided user mention string. </summary>
        public static ulong ParseUser(string mentionText)
        {
            ulong id;
            if (TryParseUser(mentionText, out id))
                return id;
            throw new ArgumentException("Invalid mention format", nameof(mentionText));
        }
        /// <summary> Tries to parse a provided user mention string. </summary>
        public static bool TryParseUser(string mentionText, out ulong userId)
        {
            mentionText = mentionText.Trim();
            if (mentionText.Length >= 3 && mentionText[0] == '<' && mentionText[1] == '@' && mentionText[mentionText.Length - 1] == '>')
            {
                if (mentionText.Length >= 4 && mentionText[2] == '!')
                    mentionText = mentionText.Substring(3, mentionText.Length - 4); //<@!123>
                else
                    mentionText = mentionText.Substring(2, mentionText.Length - 3); //<@123>
                
                if (ulong.TryParse(mentionText, NumberStyles.None, CultureInfo.InvariantCulture, out userId))
                    return true;
            }
            userId = 0;
            return false;
        }

        /// <summary> Parses a provided channel mention string. </summary>
        public static ulong ParseChannel(string mentionText)
        {
            ulong id;
            if (TryParseChannel(mentionText, out id))
                return id;
            throw new ArgumentException("Invalid mention format", nameof(mentionText));
        }
        /// <summary>Tries to parse a provided channel mention string. </summary>
        public static bool TryParseChannel(string mentionText, out ulong channelId)
        {
            mentionText = mentionText.Trim();
            if (mentionText.Length >= 3 && mentionText[0] == '<' && mentionText[1] == '#' && mentionText[mentionText.Length - 1] == '>')
            {
                mentionText = mentionText.Substring(2, mentionText.Length - 3); //<#123>
                
                if (ulong.TryParse(mentionText, NumberStyles.None, CultureInfo.InvariantCulture, out channelId))
                    return true;
            }
            channelId = 0;
            return false;
        }

        /// <summary> Parses a provided role mention string. </summary>
        public static ulong ParseRole(string mentionText)
        {
            ulong id;
            if (TryParseRole(mentionText, out id))
                return id;
            throw new ArgumentException("Invalid mention format", nameof(mentionText));
        }
        /// <summary>Tries to parse a provided role mention string. </summary>
        public static bool TryParseRole(string mentionText, out ulong roleId)
        {
            mentionText = mentionText.Trim();
            if (mentionText.Length >= 4 && mentionText[0] == '<' && mentionText[1] == '@' && mentionText[2] == '&' && mentionText[mentionText.Length - 1] == '>')
            {
                mentionText = mentionText.Substring(3, mentionText.Length - 4); //<@&123>
                
                if (ulong.TryParse(mentionText, NumberStyles.None, CultureInfo.InvariantCulture, out roleId))
                    return true;
            }
            roleId = 0;
            return false;
        }
    }
}
