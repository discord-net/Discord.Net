using Discord.API.Rest;
using System;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class MessageHelper
    {
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
    }
}
