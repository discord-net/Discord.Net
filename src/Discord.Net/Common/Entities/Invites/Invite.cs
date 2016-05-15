using System.Threading.Tasks;
using Model = Discord.API.Invite;

namespace Discord
{
    public class Invite : IInvite
    {
        /// <inheritdoc />
        public string Code { get; }
        internal IDiscordClient Discord { get; }

        /// <inheritdoc />
        public ulong GuildId { get; private set; }
        /// <inheritdoc />
        public ulong ChannelId { get; private set; }
        /// <inheritdoc />
        public string XkcdCode { get; private set; }
        /// <inheritdoc />
        public string GuildName { get; private set; }
        /// <inheritdoc />
        public string ChannelName { get; private set; }

        /// <inheritdoc />
        public string Url => $"{DiscordConfig.InviteUrl}/{XkcdCode ?? Code}";
        /// <inheritdoc />
        public string XkcdUrl => XkcdCode != null ? $"{DiscordConfig.InviteUrl}/{XkcdCode}" : null;


        internal Invite(IDiscordClient discord, Model model)
        {
            Discord = discord;
            Code = model.Code;

            Update(model);
        }
        protected virtual void Update(Model model)
        {
            XkcdCode = model.XkcdPass;
            GuildId = model.Guild.Id;
            ChannelId = model.Channel.Id;
            GuildName = model.Guild.Name;
            ChannelName = model.Channel.Name;
        }

        /// <inheritdoc />
        public async Task Accept()
        {
            await Discord.BaseClient.AcceptInvite(Code).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task Delete()
        {
            await Discord.BaseClient.DeleteInvite(Code).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override string ToString() => XkcdUrl ?? Url;

        string IEntity<string>.Id => Code;
    }
}
