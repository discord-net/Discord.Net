using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// Represents an entry in an audit log
    /// </summary>
    public interface IAuditLogEntry : ISnowflakeEntity
    {
        /// <summary>
        /// The action which occured to create this entry
        /// </summary>
        ActionType Action { get; }

        /// <summary>
        /// The data for this entry. May be <see cref="null"/> if no data was available.
        /// </summary>
        IAuditLogData Data { get; }

        /// <summary>
        /// The user responsible for causing the changes
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// The reason behind the change. May be <see cref="null"/> if no reason was provided.
        /// </summary>
        string Reason { get; }
    }
}
