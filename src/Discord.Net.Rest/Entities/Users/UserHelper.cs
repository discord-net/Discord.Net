using Discord.API.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.User;
using ImageModel = Discord.API.Image;
using System.Linq;

namespace Discord.Rest
{
    internal static class UserHelper
    {
        public static async Task<Model> ModifyAsync(ISelfUser user, BaseDiscordClient client, Action<ModifyCurrentUserParams> func,
            RequestOptions options)
        {
            var args = new ModifyCurrentUserParams();
            func(args);
            var apiArgs = new API.Rest.ModifyCurrentUserParams
            {
                Avatar = args.Avatar.IsSpecified ? ImageModel.Create(args.Avatar.Value) : Optional.Create<ImageModel?>(),
                Username = args.Username
            };

            if (!apiArgs.Avatar.IsSpecified && user.AvatarId != null)
                apiArgs.Avatar = new ImageModel(user.AvatarId);

            return await client.ApiClient.ModifySelfAsync(apiArgs, options).ConfigureAwait(false);
        }
        public static async Task<ModifyGuildMemberParams> ModifyAsync(IGuildUser user, BaseDiscordClient client, Action<ModifyGuildMemberParams> func,
            RequestOptions options)
        {
            var args = new ModifyGuildMemberParams();
            func(args);
            var apiArgs = new API.Rest.ModifyGuildMemberParams
            {
                ChannelId = args.Channel.IsSpecified ? args.Channel.Value.Id : Optional.Create<ulong>(),
                Deaf = args.Deaf,
                Mute = args.Mute,
                Nickname = args.Nickname
            };

            if (args.Roles.IsSpecified)
                apiArgs.RoleIds = args.Roles.Value.Select(x => x.Id).ToArray();
            else if (args.RoleIds.IsSpecified)
                apiArgs.RoleIds = args.RoleIds.Value.ToArray();

            await client.ApiClient.ModifyGuildMemberAsync(user.GuildId, user.Id, apiArgs, options).ConfigureAwait(false);
            return args;
        }

        public static async Task KickAsync(IGuildUser user, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.RemoveGuildMemberAsync(user.GuildId, user.Id, options).ConfigureAwait(false);
        }

        public static async Task<RestDMChannel> CreateDMChannelAsync(IUser user, BaseDiscordClient client,
            RequestOptions options)
        {
            var args = new CreateDMChannelParams(user.Id);
            return RestDMChannel.Create(client, await client.ApiClient.CreateDMChannelAsync(args, options).ConfigureAwait(false));
        }
    }
}
