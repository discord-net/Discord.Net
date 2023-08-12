using Discord.WebSocket.Cache;
using System;
namespace Discord.WebSocket
{
    public sealed class SocketStageChannel : SocketVoiceChannel, IStageChannel
    {
        public SocketStageChannel(DiscordSocketClient discord, ulong guildId, IStageChannelModel model)
            : base(discord, guildId, model)
        {
        }
    }
}

