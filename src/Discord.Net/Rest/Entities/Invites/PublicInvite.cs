using System.Threading.Tasks;
using Model = Discord.API.Invite;

namespace Discord.Rest
{
    public class PublicInvite : Invite, IPublicInvite
    {
        /// <inheritdoc />
        public string GuildName { get; private set; }
        /// <inheritdoc />
        public string ChannelName { get; private set; }

        /// <inheritdoc />
        public ulong GuildId => _guildId;
        /// <inheritdoc />
        public ulong ChannelId => _channelId;

        internal override DiscordClient Discord { get; }

        internal PublicInvite(DiscordClient discord, Model model)
            : base(model)
        {
            Discord = discord;
        }
        protected override void Update(Model model)
        {
            base.Update(model);
            GuildName = model.Guild.Name;
            ChannelName = model.Channel.Name;
        }

        /// <inheritdoc />
        public async Task Update()
        {
            var model = await Discord.BaseClient.GetInvite(Code).ConfigureAwait(false);
            Update(model);
        }
    }
}
