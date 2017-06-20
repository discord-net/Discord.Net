using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Discord.API.AuditLogOptions;

namespace Discord.Rest
{
    public class MessageDeleteAuditLogOptions : IAuditLogOptions
    {
        internal MessageDeleteAuditLogOptions(BaseDiscordClient discord, Model model)
        {
            MessageCount = model.Count;
            SourceChannelId = model.ChannelId;
        }

        //TODO: turn this into an IChannel
        public ulong SourceChannelId { get; }
        public int MessageCount { get; }
    }
}
