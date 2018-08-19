using System.Diagnostics;
using Model = Discord.API.GuildEmbed;

namespace Discord
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public struct RestGuildEmbed
    {
        public bool IsEnabled { get; }
        public ulong? ChannelId { get; }

        internal RestGuildEmbed(bool isEnabled, ulong? channelId)
        {
            ChannelId = channelId;
            IsEnabled = isEnabled;
        }

        internal static RestGuildEmbed Create(Model model) => new RestGuildEmbed(model.Enabled, model.ChannelId);

        public override string ToString() => ChannelId?.ToString();
        private string DebuggerDisplay => $"{ChannelId} ({(IsEnabled ? "Enabled" : "Disabled")})";
    }
}
