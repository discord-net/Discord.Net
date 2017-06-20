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
    public interface IAuditLogEntry : IEntity<ulong>
    {
        /// <summary>
        /// The action which occured to create this entry
        /// </summary>
        ActionType Action { get; }

        /// <summary>
        /// The changes which occured within this entry. May be empty if no changes occured.
        /// </summary>
        IReadOnlyCollection<IAuditLogChange> Changes { get; }

        /// <summary>
        /// Any options which apply to this entry. If no options were provided, this may be <see cref="null"/>.
        /// </summary>
        IAuditLogOptions Options { get; }

        /// <summary>
        /// The id which the target applies to
        /// </summary>
        ulong TargetId { get; }

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
