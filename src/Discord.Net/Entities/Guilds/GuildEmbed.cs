using System.Diagnostics;
using Model = Discord.API.GuildEmbed;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct GuildEmbed
    {
        public bool IsEnabled { get; private set; }
        public ulong? ChannelId { get; private set; }

        public GuildEmbed(bool isEnabled, ulong? channelId)
        {
            ChannelId = channelId;
            IsEnabled = isEnabled;
        }
        internal GuildEmbed(Model model)
            : this(model.Enabled, model.ChannelId) { }

        public override string ToString() => ChannelId?.ToString();
        private string DebuggerDisplay => $"{ChannelId} ({(IsEnabled ? "Enabled" : "Disabled")})";
    }
}
