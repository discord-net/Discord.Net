using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IPresenceModel
    {
        ulong UserId { get; }
        ulong? GuildId { get; }
        UserStatus Status { get; }
        ClientType[] ActiveClients { get; }
        IActivityModel[] Activities { get; }
    }
}
