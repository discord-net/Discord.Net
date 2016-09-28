using Discord.API.Rest;
using System;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class RoleHelper
    {
        //General
        public static async Task DeleteAsync(IRole role, DiscordClient client)
        {
            await client.ApiClient.DeleteGuildRoleAsync(role.Guild.Id, role.Id).ConfigureAwait(false);
        }
        public static async Task ModifyAsync(IRole role, DiscordClient client, 
            Action<ModifyGuildRoleParams> func)
        {
            var args = new ModifyGuildRoleParams();
            func(args);
            await client.ApiClient.ModifyGuildRoleAsync(role.Guild.Id, role.Id, args);
        }
    }
}
