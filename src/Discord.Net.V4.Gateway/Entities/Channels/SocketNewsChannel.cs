using Discord.Gateway.Cache;
using System;
namespace Discord.Gateway
{
    public sealed class SocketNewsChannel : SocketTextChannel, INewsChannel
    {
        public SocketNewsChannel(DiscordGatewayClient discord, ulong guildId, IGuildTextChannelModel model)
            : base(discord, guildId, model)
        {
        }

        /// <inheritdoc />
        /// <remarks>
        ///     <note type="important">
        ///     This property is not supported by this type. Attempting to use this property will result in a <see cref="NotSupportedException"/>.
        ///     </note>
        /// </remarks>
        public override int SlowModeInterval
            => throw new NotSupportedException("News channels do not support Slow Mode.");
    }
}

