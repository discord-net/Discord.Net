using Model = Discord.API.InviteMetadata;

namespace Discord
{
    internal class InviteMetadata : Invite, IInviteMetadata
    {
        public bool IsRevoked { get; private set; }
        public bool IsTemporary { get; private set; }
        public int? MaxAge { get; private set; }
        public int? MaxUses { get; private set; }
        public int Uses { get; private set; }

        public InviteMetadata(DiscordClient client, Model model)
            : base(client, model)
        {
            Update(model, UpdateSource.Creation);
        }
        private void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            IsRevoked = model.Revoked;
            IsTemporary = model.Temporary;
            MaxAge = model.MaxAge != 0 ? model.MaxAge : (int?)null;
            MaxUses = model.MaxUses;
            Uses = model.Uses;
        }
    }
}
