using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IPresenseModel : IEntityModel<ulong>
    {
        ulong UserId { get; }
        ulong GuildId { get; }
        UserStatus Status { get; }
        IActivityModel[] Activities { get; }
        ClientType[] ClientStatus { get; }

        ulong IEntityModel<ulong>.Id => UserId;
    }
}
