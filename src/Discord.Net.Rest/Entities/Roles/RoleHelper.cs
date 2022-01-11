using System;
using System.Threading.Tasks;
using Model = Discord.API.Role;
using BulkParams = Discord.API.Rest.ModifyGuildRolesParams;

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

                if (args.Icon.IsSpecified && args.Emoji.IsSpecified)
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
                Icon = args.Icon.IsSpecified ? args.Icon.Value.Value.ToModel() : Optional<API.Image?>.Unspecified,
                Emoji = args.Emoji.GetValueOrDefault()?.Name ?? Optional<string>.Unspecified
            };

            if (args.Icon.IsSpecified && role.Emoji != null)
            {
                apiArgs.Emoji = null;
            }

            if (args.Emoji.IsSpecified && !string.IsNullOrEmpty(role.Icon))
            {
                apiArgs.Icon = null;
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
