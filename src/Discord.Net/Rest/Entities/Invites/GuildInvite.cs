using System.Threading.Tasks;
using Model = Discord.API.InviteMetadata;

namespace Discord.Rest
{
    public class GuildInvite : Invite, IGuildInvite
    {
        /// <summary> Gets the guild this invite is linked to. </summary>
        public Guild Guild { get; private set; }
        /// <inheritdoc />
        public ulong ChannelId { get; private set; }

        /// <inheritdoc />
        public bool IsRevoked { get; private set; }
        /// <inheritdoc />
        public bool IsTemporary { get; private set; }
        /// <inheritdoc />
        public int? MaxAge { get; private set; }
        /// <inheritdoc />
        public int? MaxUses { get; private set; }
        /// <inheritdoc />
        public int Uses { get; private set; }

        internal override DiscordClient Discord => Guild.Discord;

        internal GuildInvite(Guild guild, Model model)
            : base(model)
        {
            Guild = guild;

            Update(model); //Causes base.Update(Model) to be run twice, but that's fine.
        }
        private void Update(Model model)
        {
            base.Update(model);
            IsRevoked = model.Revoked;
            IsTemporary = model.Temporary;
            MaxAge = model.MaxAge != 0 ? model.MaxAge : (int?)null;
            MaxUses = model.MaxUses;
            Uses = model.Uses;
        }

        /// <inheritdoc />
        public async Task Delete()
        {
            await Discord.BaseClient.DeleteInvite(Code).ConfigureAwait(false);
        }

        IGuild IGuildInvite.Guild => Guild;
        ulong IInvite.GuildId => Guild.Id;
    }
}
