using Discord.API.Rest;
using System;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class UserHelper
    {
        public static async Task ModifyAsync(ISelfUser user, BaseDiscordClient client, Action<ModifyCurrentUserParams> func,
            RequestOptions options)
        {
            var args = new ModifyCurrentUserParams();
            func(args);
            await client.ApiClient.ModifySelfAsync(args, options);
        }
        public static async Task ModifyAsync(IGuildUser user, BaseDiscordClient client, Action<ModifyGuildMemberParams> func,
            RequestOptions options)
        {
            var args = new ModifyGuildMemberParams();
            func(args);
            await client.ApiClient.ModifyGuildMemberAsync(user.GuildId, user.Id, args, options);
        }

        public static async Task KickAsync(IGuildUser user, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.RemoveGuildMemberAsync(user.GuildId, user.Id, options);
        }

        public static async Task<RestDMChannel> CreateDMChannelAsync(IUser user, BaseDiscordClient client,
            RequestOptions options)
        {
            var args = new CreateDMChannelParams(user.Id);
            return RestDMChannel.Create(client, await client.ApiClient.CreateDMChannelAsync(args, options));
        }
    }
}
