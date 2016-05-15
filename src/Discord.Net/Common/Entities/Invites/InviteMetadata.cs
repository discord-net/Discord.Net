using Model = Discord.API.InviteMetadata;

namespace Discord
{
    public class InviteMetadata : Invite, IInviteMetadata
    {
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

        internal InviteMetadata(IDiscordClient client, Model model)
            : base(client, model)
        {
            Update(model);
        }
        private void Update(Model model)
        {
            IsRevoked = model.Revoked;
            IsTemporary = model.Temporary;
            MaxAge = model.MaxAge != 0 ? model.MaxAge : (int?)null;
            MaxUses = model.MaxUses;
            Uses = model.Uses;
        }
    }
}
