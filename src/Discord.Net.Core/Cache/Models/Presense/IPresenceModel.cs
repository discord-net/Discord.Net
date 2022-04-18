using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IPresenceModel
    {
        ulong UserId { get; set; }
        ulong? GuildId { get; set; }
        UserStatus Status { get; set; }
        ClientType[] ActiveClients { get; set; }
        IActivityModel[] Activities { get; set; }
    }
}
