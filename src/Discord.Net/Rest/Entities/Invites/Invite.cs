using System.Threading.Tasks;
using Model = Discord.API.Invite;

namespace Discord.Rest
{
    public abstract class Invite : IInvite
    {
        protected ulong _guildId, _channelId;

        /// <inheritdoc />
        public string Code { get; }
        /// <inheritdoc />
        public string XkcdCode { get; }

        /// <inheritdoc />
        public string Url => $"{DiscordConfig.InviteUrl}/{XkcdCode ?? Code}";
        /// <inheritdoc />
        public string XkcdUrl => XkcdCode != null ? $"{DiscordConfig.InviteUrl}/{XkcdCode}" : null;

        internal abstract DiscordClient Discord { get; }

        internal Invite(Model model)
        {
            Code = model.Code;
            XkcdCode = model.XkcdPass;

            Update(model);
        }
        protected virtual void Update(Model model)
        {
            _guildId = model.Guild.Id;
            _channelId = model.Channel.Id;
        }

        /// <inheritdoc />
        public async Task Accept()
        {
            await Discord.BaseClient.AcceptInvite(Code).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override string ToString() => XkcdUrl ?? Url;

        string IEntity<string>.Id => Code;
        ulong IInvite.GuildId => _guildId;
        ulong IInvite.ChannelId => _channelId;
    }
}
