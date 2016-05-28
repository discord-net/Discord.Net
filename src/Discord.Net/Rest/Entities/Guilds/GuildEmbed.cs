using System;
using System.Diagnostics;
using Model = Discord.API.GuildEmbed;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
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

        public override string ToString() => Id.ToString();
        private string DebuggerDisplay => $"{Id}{(IsEnabled ? " (Enabled)" : "")}";
    }
}
