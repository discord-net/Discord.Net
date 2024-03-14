using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageModel = Discord.API.Image;
using Model = Discord.API.User;

namespace Discord.Rest
{
    internal static class UserHelper
    {
        public static Task<Model> ModifyAsync(ISelfUser user, BaseDiscordClient client, Action<SelfUserProperties> func, RequestOptions options)
        {
            var args = new SelfUserProperties();
            func(args);
            var apiArgs = new API.Rest.ModifyCurrentUserParams
            {
                Avatar = args.Avatar.IsSpecified ? args.Avatar.Value?.ToModel() : Optional.Create<ImageModel?>(),
                Username = args.Username,
                Banner = args.Banner.IsSpecified ? args.Banner.Value?.ToModel() : Optional.Create<ImageModel?>()
            };

            if (!apiArgs.Avatar.IsSpecified && user.AvatarId != null)
                apiArgs.Avatar = new ImageModel(user.AvatarId);

            return client.ApiClient.ModifySelfAsync(apiArgs, options);
        }
        public static async Task<GuildUserProperties> ModifyAsync(IGuildUser user, BaseDiscordClient client, Action<GuildUserProperties> func,
            RequestOptions options)
        {
            var args = new GuildUserProperties();
            func(args);

            if (args.TimedOutUntil.IsSpecified && args.TimedOutUntil.Value.Value.Offset > (new TimeSpan(28, 0, 0, 0)))
                throw new ArgumentOutOfRangeException(nameof(args.TimedOutUntil), "Offset cannot be more than 28 days from the current date.");

            var apiArgs = new API.Rest.ModifyGuildMemberParams
            {
                Deaf = args.Deaf,
                Mute = args.Mute,
                Nickname = args.Nickname,
                TimedOutUntil = args.TimedOutUntil,
                Flags = args.Flags
            };

            if (args.Channel.IsSpecified)
                apiArgs.ChannelId = args.Channel.Value?.Id;
            else if (args.ChannelId.IsSpecified)
                apiArgs.ChannelId = args.ChannelId.Value;

            if (args.Roles.IsSpecified)
                apiArgs.RoleIds = args.Roles.Value.Select(x => x.Id).ToArray();
            else if (args.RoleIds.IsSpecified)
                apiArgs.RoleIds = args.RoleIds.Value.ToArray();

            /*
             * Ensure that the nick passed in the params of the request is not null.
             * string.Empty ("") is the only way to reset the user nick in the API,
             * a value of null does not. This is a workaround.
             */
            if (apiArgs.Nickname.IsSpecified && apiArgs.Nickname.Value == null)
                apiArgs.Nickname = new Optional<string>(string.Empty);

            await client.ApiClient.ModifyGuildMemberAsync(user.GuildId, user.Id, apiArgs, options).ConfigureAwait(false);
            return args;
        }

        public static Task KickAsync(IGuildUser user, BaseDiscordClient client, string reason, RequestOptions options)
            => client.ApiClient.RemoveGuildMemberAsync(user.GuildId, user.Id, reason, options);

        public static async Task<RestDMChannel> CreateDMChannelAsync(IUser user, BaseDiscordClient client,
            RequestOptions options)
        {
            var args = new CreateDMChannelParams(user.Id);
            return RestDMChannel.Create(client, await client.ApiClient.CreateDMChannelAsync(args, options).ConfigureAwait(false));
        }

        public static async Task AddRolesAsync(IGuildUser user, BaseDiscordClient client, IEnumerable<ulong> roleIds, RequestOptions options)
        {
            foreach (var roleId in roleIds)
                await client.ApiClient.AddRoleAsync(user.Guild.Id, user.Id, roleId, options).ConfigureAwait(false);
        }

        public static async Task RemoveRolesAsync(IGuildUser user, BaseDiscordClient client, IEnumerable<ulong> roleIds, RequestOptions options)
        {
            foreach (var roleId in roleIds)
                await client.ApiClient.RemoveRoleAsync(user.Guild.Id, user.Id, roleId, options).ConfigureAwait(false);
        }

        public static Task SetTimeoutAsync(IGuildUser user, BaseDiscordClient client, TimeSpan span, RequestOptions options)
        {
            if (span.TotalDays > 28) // As its double, an exact value of 28 can be accepted.
                throw new ArgumentOutOfRangeException(nameof(span), "Offset cannot be more than 28 days from the current date.");
            if (span.Ticks <= 0)
                throw new ArgumentOutOfRangeException(nameof(span), "Offset cannot hold no value or have a negative value.");
            var apiArgs = new API.Rest.ModifyGuildMemberParams()
            {
                TimedOutUntil = DateTimeOffset.UtcNow.Add(span)
            };
            return client.ApiClient.ModifyGuildMemberAsync(user.Guild.Id, user.Id, apiArgs, options);
        }

        public static Task RemoveTimeOutAsync(IGuildUser user, BaseDiscordClient client, RequestOptions options)
        {
            var apiArgs = new API.Rest.ModifyGuildMemberParams()
            {
                TimedOutUntil = null
            };
            return client.ApiClient.ModifyGuildMemberAsync(user.Guild.Id, user.Id, apiArgs, options);
        }
    }
}
