using System.Diagnostics;
using Model = Discord.API.GuildWidget;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct RestGuildWidget
    {
        public bool IsEnabled { get; private set; }
        public ulong? ChannelId { get; private set; }

        internal RestGuildWidget(bool isEnabled, ulong? channelId)
        {
            ChannelId = channelId;
            IsEnabled = isEnabled;
        }
        internal static RestGuildWidget Create(Model model)
        {
            return new RestGuildWidget(model.Enabled, model.ChannelId);
        }

        public override string ToString() => ChannelId?.ToString() ?? "Unknown";
        private string DebuggerDisplay => $"{ChannelId} ({(IsEnabled ? "Enabled" : "Disabled")})";
    }
}
