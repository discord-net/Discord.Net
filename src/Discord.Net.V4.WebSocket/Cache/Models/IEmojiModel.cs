using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IEmojiModel
    {
        ulong? Id { get; }
        string Name { get; }
        ulong[] Roles { get; }
        bool RequireColons { get; }
        bool IsManaged { get; }
        bool IsAnimated { get; }
        bool IsAvailable { get; }

        ulong? CreatorId { get; }
    }
}
