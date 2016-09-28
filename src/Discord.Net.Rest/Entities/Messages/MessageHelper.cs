using Discord.API.Rest;
using System;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class MessageHelper
    {
        public static async Task GetAsync(IMessage msg, DiscordClient client)
        {
            await client.ApiClient.GetChannelMessageAsync(msg.ChannelId, msg.Id);
        }
        public static async Task ModifyAsync(IMessage msg, DiscordClient client, Action<ModifyMessageParams> func)
        {
            var args = new ModifyMessageParams();
            func(args);
            await client.ApiClient.ModifyMessageAsync(msg.ChannelId, msg.Id, args);
        }
        public static async Task DeleteAsync(IMessage msg, DiscordClient client)
        {
            await client.ApiClient.DeleteMessageAsync(msg.ChannelId, msg.Id);
        }

        public static async Task PinAsync(IMessage msg, DiscordClient client)
        {
            await client.ApiClient.AddPinAsync(msg.ChannelId, msg.Id);
        }
        public static async Task UnpinAsync(IMessage msg, DiscordClient client)
        {
            await client.ApiClient.RemovePinAsync(msg.ChannelId, msg.Id);
        }
    }
}
