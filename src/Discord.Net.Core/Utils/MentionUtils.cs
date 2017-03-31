using System;
using System.Globalization;
using System.Text;

namespace Discord
{
    public static class MentionUtils
    {
        private const char SanitizeChar = '\x200b';

        //If the system can't be positive a user doesn't have a nickname, assume useNickname = true (source: Jake)
        internal static string MentionUser(string id, bool useNickname = true) => useNickname ? $"<@!{id}>" : $"<@{id}>";
        public static string MentionUser(ulong id) => MentionUser(id.ToString(), true);
        internal static string MentionChannel(string id) => $"<#{id}>";
        public static string MentionChannel(ulong id) => MentionChannel(id.ToString());
        internal static string MentionRole(string id) => $"<@&{id}>";        
        public static string MentionRole(ulong id) => MentionRole(id.ToString());

        /// <summary> Parses a provided user mention string. </summary>
        public static ulong ParseUser(string text)
        {
            ulong id;
            if (TryParseUser(text, out id))
                return id;
            throw new ArgumentException("Invalid mention format", nameof(text));
        }
        /// <summary> Tries to parse a provided user mention string. </summary>
        public static bool TryParseUser(string text, out ulong userId)
        {
            if (text.Length >= 3 && text[0] == '<' && text[1] == '@' && text[text.Length - 1] == '>')
            {
                if (text.Length >= 4 && text[2] == '!')
                    text = text.Substring(3, text.Length - 4); //<@!123>
                else
                    text = text.Substring(2, text.Length - 3); //<@123>
                
                if (ulong.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out userId))
                    return true;
            }
            userId = 0;
            return false;
        }

        /// <summary> Parses a provided channel mention string. </summary>
        public static ulong ParseChannel(string text)
        {
            ulong id;
            if (TryParseChannel(text, out id))
                return id;
            throw new ArgumentException("Invalid mention format", nameof(text));
        }
        /// <summary>Tries to parse a provided channel mention string. </summary>
        public static bool TryParseChannel(string text, out ulong channelId)
        {
            if (text.Length >= 3 && text[0] == '<' && text[1] == '#' && text[text.Length - 1] == '>')
            {
                text = text.Substring(2, text.Length - 3); //<#123>
                
                if (ulong.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out channelId))
                    return true;
            }
            channelId = 0;
            return false;
        }

        /// <summary> Parses a provided role mention string. </summary>
        public static ulong ParseRole(string text)
        {
            ulong id;
            if (TryParseRole(text, out id))
                return id;
            throw new ArgumentException("Invalid mention format", nameof(text));
        }
        /// <summary>Tries to parse a provided role mention string. </summary>
        public static bool TryParseRole(string text, out ulong roleId)
        {
            if (text.Length >= 4 && text[0] == '<' && text[1] == '@' && text[2] == '&' && text[text.Length - 1] == '>')
            {
                text = text.Substring(3, text.Length - 4); //<@&123>
                
                if (ulong.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out roleId))
                    return true;
            }
            roleId = 0;
            return false;
        }

        internal static string Resolve(IMessage msg, int startIndex, TagHandling userHandling, TagHandling channelHandling, TagHandling roleHandling, TagHandling everyoneHandling, TagHandling emojiHandling)
        {
            var text = new StringBuilder(msg.Content.Substring(startIndex));
            var tags = msg.Tags;
            int indexOffset = -startIndex;

            foreach (var tag in tags)
            {
                if (tag.Index < startIndex)
                    continue;

                string newText = "";
                switch (tag.Type)
                {
                    case TagType.UserMention:
                        if (userHandling == TagHandling.Ignore) continue;
                        newText = ResolveUserMention(tag, userHandling);
                        break;
                    case TagType.ChannelMention:
                        if (channelHandling == TagHandling.Ignore) continue;
                        newText = ResolveChannelMention(tag, channelHandling);
                        break;
                    case TagType.RoleMention:
                        if (roleHandling == TagHandling.Ignore) continue;
                        newText = ResolveRoleMention(tag, roleHandling);
                        break;
                    case TagType.EveryoneMention:
                        if (everyoneHandling == TagHandling.Ignore) continue;
                        newText = ResolveEveryoneMention(tag, everyoneHandling);
                        break;
                    case TagType.HereMention:
                        if (everyoneHandling == TagHandling.Ignore) continue;
                        newText = ResolveHereMention(tag, everyoneHandling);
                        break;
                    case TagType.Emoji:
                        if (emojiHandling == TagHandling.Ignore) continue;
                        newText = ResolveEmoji(tag, emojiHandling);
                        break;
                }
                text.Remove(tag.Index + indexOffset, tag.Length);
                text.Insert(tag.Index + indexOffset, newText);
                indexOffset += newText.Length - tag.Length;
            }
            return text.ToString();
        }
        internal static string ResolveUserMention(ITag tag, TagHandling mode)
        {
            if (mode != TagHandling.Remove)
            {
                var user = tag.Value as IUser;
                var guildUser = user as IGuildUser;
                switch (mode)
                {
                    case TagHandling.Name:
                        if (user != null)
                            return $"@{guildUser?.Nickname ?? user?.Username}";
                        else
                            return $"";
                    case TagHandling.NameNoPrefix:
                        if (user != null)
                            return $"{guildUser?.Nickname ?? user?.Username}";
                        else
                            return $"";
                    case TagHandling.FullName:
                        if (user != null)
                            return $"@{user.Username}#{user.Discriminator}";
                        else
                            return $"";
                    case TagHandling.FullNameNoPrefix:
                        if (user != null)
                            return $"{user.Username}#{user.Discriminator}";
                        else
                            return $"";
                    case TagHandling.Sanitize:
                        if (guildUser != null && guildUser.Nickname == null)
                            return MentionUser($"{SanitizeChar}{tag.Key}", false);
                        else
                            return MentionUser($"{SanitizeChar}{tag.Key}", true);
                }
            }
            return "";
        }
        internal static string ResolveChannelMention(ITag tag, TagHandling mode)
        {
            if (mode != TagHandling.Remove)
            {
                var channel = tag.Value as IChannel;
                switch (mode)
                {
                    case TagHandling.Name:
                    case TagHandling.FullName:
                        if (channel != null)
                            return $"#{channel.Name}";
                        else
                            return $"";
                    case TagHandling.NameNoPrefix:
                    case TagHandling.FullNameNoPrefix:
                        if (channel != null)
                            return $"{channel.Name}";
                        else
                            return $"";
                    case TagHandling.Sanitize:
                        return MentionChannel($"{SanitizeChar}{tag.Key}");
                }
            }
            return "";
        }
        internal static string ResolveRoleMention(ITag tag, TagHandling mode)
        {
            if (mode != TagHandling.Remove)
            {
                var role = tag.Value as IRole;
                switch (mode)
                {
                    case TagHandling.Name:
                    case TagHandling.FullName:
                        if (role != null)
                            return $"@{role.Name}";
                        else
                            return $"";
                    case TagHandling.NameNoPrefix:
                    case TagHandling.FullNameNoPrefix:
                        if (role != null)
                            return $"{role.Name}";
                        else
                            return $"";
                    case TagHandling.Sanitize:
                        return MentionRole($"{SanitizeChar}{tag.Key}");
                }
            }
            return "";
        }
        internal static string ResolveEveryoneMention(ITag tag, TagHandling mode)
        {
            if (mode != TagHandling.Remove)
            {
                switch (mode)
                {
                    case TagHandling.Name:
                    case TagHandling.FullName:
                    case TagHandling.NameNoPrefix:
                    case TagHandling.FullNameNoPrefix:
                        return "everyone";
                    case TagHandling.Sanitize:
                        return $"@{SanitizeChar}everyone";
                }
            }
            return "";
        }
        internal static string ResolveHereMention(ITag tag, TagHandling mode)
        {
            if (mode != TagHandling.Remove)
            {
                switch (mode)
                {
                    case TagHandling.Name:
                    case TagHandling.FullName:
                    case TagHandling.NameNoPrefix:
                    case TagHandling.FullNameNoPrefix:
                        return "here";
                    case TagHandling.Sanitize:
                        return $"@{SanitizeChar}here";
                }
            }
            return "";
        }
        internal static string ResolveEmoji(ITag tag, TagHandling mode)
        {
            if (mode != TagHandling.Remove)
            {
                Emoji emoji = (Emoji)tag.Value;

                //Remove if its name contains any bad chars (prevents a few tag exploits)
                for (int i = 0; i < emoji.Name.Length; i++)
                {
                    char c = emoji.Name[i];
                    if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
                        return "";
                }

                switch (mode)
                {
                    case TagHandling.Name:
                    case TagHandling.FullName:
                        return $":{emoji.Name}:";
                    case TagHandling.NameNoPrefix:
                    case TagHandling.FullNameNoPrefix:
                        return $"{emoji.Name}";
                    case TagHandling.Sanitize:
                        return $"<{emoji.Id}{SanitizeChar}:{SanitizeChar}{emoji.Name}>";
                }
            }
            return "";
        }
    }
}
