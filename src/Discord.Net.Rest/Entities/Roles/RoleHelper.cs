using System;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord.Rest
{
    internal static class RoleHelper
    {
        //General
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
            var apiArgs = new API.Rest.ModifyGuildRoleParams
            {
                Color = args.Color.IsSpecified ? args.Color.Value.RawValue : Optional.Create<uint>(),
                Hoist = args.Hoist,
                Mentionable = args.Mentionable,
                Name = args.Name,
                Permissions = args.Permissions.IsSpecified ? args.Permissions.Value.RawValue : Optional.Create<ulong>(),
                Position = args.Position
            };
            return await client.ApiClient.ModifyGuildRoleAsync(role.Guild.Id, role.Id, apiArgs, options).ConfigureAwait(false);
        }
    }
}
