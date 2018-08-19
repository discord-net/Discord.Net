using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Discord
{
    public static class MentionUtils
    {
        private const char SanitizeChar = '\x200b';

        //If the system can't be positive a user doesn't have a nickname, assume useNickname = true (source: Jake)
        internal static string MentionUser(string id, bool useNickname = true) =>
            useNickname ? $"<@!{id}>" : $"<@{id}>";

        public static string MentionUser(ulong id) => MentionUser(id.ToString());
        internal static string MentionChannel(string id) => $"<#{id}>";
        public static string MentionChannel(ulong id) => MentionChannel(id.ToString());
        internal static string MentionRole(string id) => $"<@&{id}>";
        public static string MentionRole(ulong id) => MentionRole(id.ToString());

        /// <summary> Parses a provided user mention string. </summary>
        public static ulong ParseUser(string text)
        {
            if (TryParseUser(text, out var id))
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
            if (TryParseChannel(text, out var id))
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
            if (TryParseRole(text, out var id))
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

        internal static string Resolve(IMessage msg, int startIndex, TagHandling userHandling,
            TagHandling channelHandling, TagHandling roleHandling, TagHandling everyoneHandling,
            TagHandling emojiHandling)
        {
            var text = new StringBuilder(msg.Content.Substring(startIndex));
            var tags = msg.Tags;
            var indexOffset = -startIndex;

            foreach (var tag in tags)
            {
                if (tag.Index < startIndex)
                    continue;

                var newText = "";
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
            if (mode == TagHandling.Remove) return "";
            var user = tag.Value as IUser;
            var guildUser = user as IGuildUser;
            switch (mode)
            {
                case TagHandling.Name:
                    return user != null ? $"@{guildUser?.Nickname ?? user.Username}" : "";
                case TagHandling.NameNoPrefix:
                    return user != null ? $"{guildUser?.Nickname ?? user.Username}" : "";
                case TagHandling.FullName:
                    return user != null ? $"@{user.Username}#{user.Discriminator}" : "";
                case TagHandling.FullNameNoPrefix:
                    return user != null ? $"{user.Username}#{user.Discriminator}" : "";
                case TagHandling.Sanitize:
                    return MentionUser($"{SanitizeChar}{tag.Key}", guildUser == null || guildUser.Nickname != null);
            }

            return "";
        }

        internal static string ResolveChannelMention(ITag tag, TagHandling mode)
        {
            if (mode == TagHandling.Remove) return "";
            var channel = tag.Value as IChannel;
            switch (mode)
            {
                case TagHandling.Name:
                case TagHandling.FullName:
                    return channel != null ? $"#{channel.Name}" : "";
                case TagHandling.NameNoPrefix:
                case TagHandling.FullNameNoPrefix:
                    return channel != null ? $"{channel.Name}" : "";
                case TagHandling.Sanitize:
                    return MentionChannel($"{SanitizeChar}{tag.Key}");
            }

            return "";
        }

        internal static string ResolveRoleMention(ITag tag, TagHandling mode)
        {
            if (mode == TagHandling.Remove) return "";
            var role = tag.Value as IRole;
            switch (mode)
            {
                case TagHandling.Name:
                case TagHandling.FullName:
                    return role != null ? $"@{role.Name}" : "";
                case TagHandling.NameNoPrefix:
                case TagHandling.FullNameNoPrefix:
                    return role != null ? $"{role.Name}" : "";
                case TagHandling.Sanitize:
                    return MentionRole($"{SanitizeChar}{tag.Key}");
            }

            return "";
        }

        internal static string ResolveEveryoneMention(ITag tag, TagHandling mode)
        {
            if (mode == TagHandling.Remove) return "";
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
            return "";
        }

        internal static string ResolveHereMention(ITag tag, TagHandling mode)
        {
            if (mode == TagHandling.Remove) return "";
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
            return "";
        }

        internal static string ResolveEmoji(ITag tag, TagHandling mode)
        {
            if (mode == TagHandling.Remove) return "";
            var emoji = (Emote)tag.Value;

            //Remove if its name contains any bad chars (prevents a few tag exploits)
            if (emoji.Name.Any(c => !char.IsLetterOrDigit(c) && c != '_' && c != '-'))
            {
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

            return "";
        }
    }
}
