using Discord.Gateway.Cache;
using System;

namespace Discord.Gateway
{
    public sealed class GatewayNewsChannel : GatewayTextChannel, INewsChannel
    {
        public GatewayNewsChannel(DiscordGatewayClient discord, ulong guildId, IGuildTextChannelModel model)
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

        public Task<ulong> FollowAnnouncementChannelAsync(ulong channelId, RequestOptions options) => throw new NotImplementedException();
    }
}

