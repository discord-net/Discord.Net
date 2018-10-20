using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic audit log entry.
    /// </summary>
    public interface IAuditLogEntry : ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the action which occurred to create this entry.
        /// </summary>
        /// <returns>
        ///     The type of action for this audit log entry.
        /// </returns>
        ActionType Action { get; }

        /// <summary>
        ///     Gets the data for this entry.
        /// </summary>
        /// <returns>
        ///     An <see cref="IAuditLogData" /> for this audit log entry; <c>null</c> if no data is available.
        /// </returns>
        IAuditLogData Data { get; }

        /// <summary>
        ///     Gets the user responsible for causing the changes.
        /// </summary>
        /// <returns>
        ///     A user object.
        /// </returns>
        IUser User { get; }

        /// <summary>
        ///     Gets the reason behind the change.
        /// </summary>
        /// <returns>
        ///     A string containing the reason for the change; <c>null</c> if none is provided.
        /// </returns>
        string Reason { get; }
    }
}
