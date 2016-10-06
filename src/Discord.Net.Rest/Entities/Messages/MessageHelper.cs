using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class MessageHelper
    {
        private static readonly Regex _emojiRegex = new Regex(@"<:(.+?):(\d+?)>", RegexOptions.Compiled);

        public static async Task ModifyAsync(IMessage msg, BaseDiscordClient client, Action<ModifyMessageParams> func,
            RequestOptions options)
        {
            var args = new ModifyMessageParams();
            func(args);
            await client.ApiClient.ModifyMessageAsync(msg.ChannelId, msg.Id, args, options);
        }
        public static async Task DeleteAsync(IMessage msg, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.DeleteMessageAsync(msg.ChannelId, msg.Id, options);
        }

        public static async Task PinAsync(IMessage msg, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.AddPinAsync(msg.ChannelId, msg.Id, options);
        }
        public static async Task UnpinAsync(IMessage msg, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.RemovePinAsync(msg.ChannelId, msg.Id, options);
        }

        public static ImmutableArray<Emoji> GetEmojis(string text)
        {
            var matches = _emojiRegex.Matches(text);
            var builder = ImmutableArray.CreateBuilder<Emoji>(matches.Count);
            foreach (var match in matches.OfType<Match>())
            {
                ulong id;
                if (ulong.TryParse(match.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    builder.Add(new Emoji(id, match.Groups[1].Value, match.Index, match.Length));
            }
            return builder.ToImmutable();
        }
    }
}
