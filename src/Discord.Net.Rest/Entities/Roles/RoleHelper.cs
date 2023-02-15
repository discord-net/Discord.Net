using System;
using System.Threading.Tasks;
using BulkParams = Discord.API.Rest.ModifyGuildRolesParams;
using Model = Discord.API.Role;

namespace Discord.Rest
{
    internal static class RoleHelper
    {
        #region General
        public static async Task DeleteAsync(IRole role, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.DeleteGuildRoleAsync(role.Guild.Id, role.Id, options).ConfigureAwait(false);
        }
        public static async Task<Model> ModifyAsync(IRole role, BaseDiscordClient client,
            Action<RoleProperties> func, RequestOptions options)
        {
            var args = new RoleProperties();
            func(args);

            if (args.Icon.IsSpecified || args.Emoji.IsSpecified)
            {
                role.Guild.Features.EnsureFeature(GuildFeature.RoleIcons);

                if ((args.Icon.IsSpecified && args.Icon.Value != null) && (args.Emoji.IsSpecified && args.Emoji.Value != null))
                {
                    throw new ArgumentException("Emoji and Icon properties cannot be present on a role at the same time.");
                }
            }

            var apiArgs = new API.Rest.ModifyGuildRoleParams
            {
                Color = args.Color.IsSpecified ? args.Color.Value.RawValue : Optional.Create<uint>(),
                Hoist = args.Hoist,
                Mentionable = args.Mentionable,
                Name = args.Name,
                Permissions = args.Permissions.IsSpecified ? args.Permissions.Value.RawValue.ToString() : Optional.Create<string>(),
                Icon = args.Icon.IsSpecified ? args.Icon.Value?.ToModel() ?? null : Optional<API.Image?>.Unspecified,
                Emoji = args.Emoji.IsSpecified ? args.Emoji.Value?.Name ?? "" : Optional.Create<string>(),
            };

            if ((args.Icon.IsSpecified && args.Icon.Value != null) && role.Emoji != null)
            {
                apiArgs.Emoji = "";
            }

            if ((args.Emoji.IsSpecified && args.Emoji.Value != null) && !string.IsNullOrEmpty(role.Icon))
            {
                apiArgs.Icon = Optional<API.Image?>.Unspecified;
            }

            var model = await client.ApiClient.ModifyGuildRoleAsync(role.Guild.Id, role.Id, apiArgs, options).ConfigureAwait(false);

            if (args.Position.IsSpecified)
            {
                var bulkArgs = new[] { new BulkParams(role.Id, args.Position.Value) };
                await client.ApiClient.ModifyGuildRolesAsync(role.Guild.Id, bulkArgs, options).ConfigureAwait(false);
                model.Position = args.Position.Value;
            }
            return model;
        }
        #endregion
    }
}
