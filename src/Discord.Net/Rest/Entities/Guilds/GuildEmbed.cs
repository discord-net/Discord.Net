using System;
using Model = Discord.API.GuildEmbed;

namespace Discord
{
    public class GuildEmbed : IGuildEmbed
    {
        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public bool IsEnabled { get; private set; }
        /// <inheritdoc />
        public ulong? ChannelId { get; private set; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeUtils.FromSnowflake(Id);

        internal GuildEmbed(Model model)
        {
            Update(model);
        }

        private void Update(Model model)
        {
            ChannelId = model.ChannelId;
            IsEnabled = model.Enabled;
        }

        public override string ToString() => $"{Id} ({(IsEnabled ? "Enabled" : "Disabled")})";
    }
}
