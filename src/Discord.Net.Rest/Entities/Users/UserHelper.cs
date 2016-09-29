using Discord.API.Rest;
using System.Threading.Tasks;
using Model = Discord.API.User;
using MemberModel = Discord.API.GuildMember;
using System;

namespace Discord.Rest
{
    internal static class UserHelper
    {
        public static async Task<Model> GetAsync(IUser user, BaseDiscordClient client)
        {
            return await client.ApiClient.GetUserAsync(user.Id);
        }
        public static async Task<Model> GetAsync(ISelfUser user, BaseDiscordClient client)
        {
            var model = await client.ApiClient.GetMyUserAsync();
            if (model.Id != user.Id)
                throw new InvalidOperationException("Unable to update this object using a different token.");
            return model;
        }
        public static async Task<MemberModel> GetAsync(IGuildUser user, BaseDiscordClient client)
        {
            return await client.ApiClient.GetGuildMemberAsync(user.GuildId, user.Id);
        }
        public static async Task ModifyAsync(ISelfUser user, BaseDiscordClient client, Action<ModifyCurrentUserParams> func)
        {
            if (user.Id != client.CurrentUser.Id)
                throw new InvalidOperationException("Unable to modify this object using a different token.");

            var args = new ModifyCurrentUserParams();
            func(args);
            await client.ApiClient.ModifySelfAsync(args);
        }
        public static async Task ModifyAsync(IGuildUser user, BaseDiscordClient client, Action<ModifyGuildMemberParams> func)
        {
            var args = new ModifyGuildMemberParams();
            func(args);
            await client.ApiClient.ModifyGuildMemberAsync(user.GuildId, user.Id, args);
        }

        public static async Task KickAsync(IGuildUser user, BaseDiscordClient client)
        {
            await client.ApiClient.RemoveGuildMemberAsync(user.GuildId, user.Id);
        }

        public static async Task<IDMChannel> CreateDMChannelAsync(IUser user, BaseDiscordClient client)
        {
            var args = new CreateDMChannelParams(user.Id);
            return RestDMChannel.Create(client, await client.ApiClient.CreateDMChannelAsync(args));
        }
    }
}
