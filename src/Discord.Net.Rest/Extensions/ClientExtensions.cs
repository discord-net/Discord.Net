using System;
using System.Threading.Tasks;

namespace Discord.Rest
{
    public static class ClientExtensions
    {
        /// <summary>
        ///     Adds a user to the specified guild.
        /// </summary>
        /// <remarks>
        ///     This method requires you have an OAuth2 access token for the user, requested with the guilds.join scope, and that the bot have the MANAGE_INVITES permission in the guild.
        /// </remarks>
        /// <param name="client">The Discord client object.</param>
        /// <param name="guildId">The snowflake identifier of the guild.</param>
        /// <param name="userId">The snowflake identifier of the user.</param>
        /// <param name="accessToken">The OAuth2 access token for the user, requested with the guilds.join scope.</param>
        /// <param name="func">The delegate containing the properties to be applied to the user upon being added to the guild.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static Task AddGuildUserAsync(this BaseDiscordClient client, ulong guildId, ulong userId, string accessToken, Action<AddGuildUserProperties> func = null, RequestOptions options = null)
            => GuildHelper.AddGuildUserAsync(guildId, client, userId, accessToken, func, options);
    }
}
