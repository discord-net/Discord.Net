using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Invite;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class Invite : Entity<string>, IInvite
    {
        public string ChannelName { get; private set; }
        public string GuildName { get; private set; }
        public string XkcdCode { get; private set; }

        public ulong ChannelId { get; private set; }
        public ulong GuildId { get; private set; }
        public override DiscordRestClient Discord { get; }

        public string Code => Id;
        public string Url => $"{DiscordRestConfig.InviteUrl}/{XkcdCode ?? Code}";
        public string XkcdUrl => XkcdCode != null ? $"{DiscordRestConfig.InviteUrl}/{XkcdCode}" : null;

        public Invite(DiscordRestClient discord, Model model)
            : base(model.Code)
        {
            Discord = discord;

            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            XkcdCode = model.XkcdPass;
            GuildId = model.Guild.Id;
            ChannelId = model.Channel.Id;
            GuildName = model.Guild.Name;
            ChannelName = model.Channel.Name;
        }

        public async Task AcceptAsync()
        {
            await Discord.ApiClient.AcceptInviteAsync(Code).ConfigureAwait(false);
        }
        public async Task DeleteAsync()
        {
            await Discord.ApiClient.DeleteInviteAsync(Code).ConfigureAwait(false);
        }

        public override string ToString() => XkcdUrl ?? Url;
        private string DebuggerDisplay => $"{XkcdUrl ?? Url} ({GuildName} / {ChannelName})";
    }
}
