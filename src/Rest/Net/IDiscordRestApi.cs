using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Models;

namespace Discord.Net.Rest
{
    internal interface IDiscordRestApi
    {
        #region Audit Log

        /// <summary>
        /// Returns an audit log object for the guild.
        /// </summary>
        /// <remarks>
        /// Requires the <see cref="Permissions.ViewAuditLog"/> permission.
        /// </remarks>
        /// <param name="guildId">
        /// The guild identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        Task<AuditLog> GetGuildAuditLogAsync(Snowflake guildId, GetGuildAuditLogParams? args = null, CancellationToken cancellationToken = default);

        #endregion
    }
}
