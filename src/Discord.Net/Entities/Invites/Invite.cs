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
        public override DiscordClient Discord { get; }

        public string Code => Id;
        public string Url => $"{DiscordConfig.InviteUrl}/{XkcdCode ?? Code}";
        public string XkcdUrl => XkcdCode != null ? $"{DiscordConfig.InviteUrl}/{XkcdCode}" : null;

        public Invite(DiscordClient discord, Model model)
            : base(model.Code)
        {
            Discord = discord;

            Update(model, UpdateSource.Creation);
        }
        protected void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            XkcdCode = model.XkcdPass;
            GuildId = model.Guild.Id;
            ChannelId = model.Channel.Id;
            GuildName = model.Guild.Name;
            ChannelName = model.Channel.Name;
        }

        public async Task Accept()
        {
            await Discord.ApiClient.AcceptInvite(Code).ConfigureAwait(false);
        }
        public async Task Delete()
        {
            await Discord.ApiClient.DeleteInvite(Code).ConfigureAwait(false);
        }

        public override string ToString() => XkcdUrl ?? Url;
        private string DebuggerDisplay => $"{XkcdUrl ?? Url} ({GuildName} / {ChannelName})";
    }
}
