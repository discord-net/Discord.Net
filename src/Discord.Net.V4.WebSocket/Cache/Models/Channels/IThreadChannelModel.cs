using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IThreadChannelModel : IGuildTextChannelModel
    {
        bool HasJoined { get; }
        bool IsArchived { get; }
        ThreadArchiveDuration AutoArchiveDuration { get; }
        DateTimeOffset ArchiveTimestamp { get; }
        bool IsLocked { get; }
        int MemberCount { get; }
        int MessageCount { get; }
        bool IsInvitable { get; }
        DateTimeOffset CreatedAt { get; }
    }
}
