using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// An interaction is the base "thing" that is sent when a user invokes a command, and is the same for Slash Commands and other future interaction types.
    /// see <see href="https://discord.com/developers/docs/interactions/slash-commands#interaction"/>
    /// </summary>
    public interface IDiscordInteraction : ISnowflakeEntity
    {
        /// <summary>
        /// id of the interaction
        /// </summary>
        ulong Id { get; }
        InteractionType Type { get; }
        IApplicationCommandInteractionData? Data { get; }
        ulong GuildId { get; }
        ulong ChannelId { get; }
        IGuildUser Member { get; }
        string Token { get; }
        int Version { get; }
    }
}
