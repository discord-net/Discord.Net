using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Models;
using Discord.Net.Rest;

namespace Discord.Net
{
    /// <summary>
    /// TBD
    /// </summary>
    public class DiscordRestClient : IDiscordRestApi
    {
        private readonly IDiscordRestApi _api;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="api"></param>
        internal DiscordRestClient(IDiscordRestApi api)
        {
            _api = api;
        }

        #region Audit Log

        /// <inheritdoc/>
        public Task<AuditLog> GetGuildAuditLogAsync(Snowflake guildId, GetGuildAuditLogParams? args = null, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            if (args == null)
                args = new();
            else
                args.Validate();
            return _api.GetGuildAuditLogAsync(guildId, args, cancellationToken);
        }

        #endregion
    }
}
