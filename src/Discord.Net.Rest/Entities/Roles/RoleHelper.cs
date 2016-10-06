using Discord.API.Rest;
using System;
using System.Threading.Tasks;

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
        public static async Task ModifyAsync(IRole role, BaseDiscordClient client, 
            Action<ModifyGuildRoleParams> func, RequestOptions options)
        {
            var args = new ModifyGuildRoleParams();
            func(args);
            await client.ApiClient.ModifyGuildRoleAsync(role.Guild.Id, role.Id, args, options);
        }
    }
}
