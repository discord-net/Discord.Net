using Discord.API.Rest;
using System;
using System.Threading.Tasks;

namespace Discord
{
    public interface IVoiceChannel : IGuildChannel
    {
        /// <summary> Gets the bitrate, in bits per second, clients in this voice channel are requested to use. </summary>
        int Bitrate { get; }

        /// <summary> Modifies this voice channel. </summary>
        Task Modify(Action<ModifyVoiceChannelParams> func);
    }
}