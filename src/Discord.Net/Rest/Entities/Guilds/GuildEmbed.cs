using System;
using Model = Discord.API.GuildEmbed;

namespace Discord.Rest
{
    public class GuildEmbed : IGuildEmbed
    {
        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public bool IsEnabled { get; private set; }
        /// <inheritdoc />
        public ulong? ChannelId { get; private set; }

        internal DiscordClient Discord { get; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeHelper.FromSnowflake(Id);

        internal GuildEmbed(DiscordClient discord, Model model)
        {
            Discord = discord;
            Update(model);
        }

        private void Update(Model model)
        {
            ChannelId = model.ChannelId;
            IsEnabled = model.Enabled;
        }
    }
}
