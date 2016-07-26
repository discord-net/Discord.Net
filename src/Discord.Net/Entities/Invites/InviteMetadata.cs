using System;
using Model = Discord.API.InviteMetadata;

namespace Discord
{
    internal class InviteMetadata : Invite, IInviteMetadata
    {
        private long _createdAtTicks;

        public bool IsRevoked { get; private set; }
        public bool IsTemporary { get; private set; }
        public int? MaxAge { get; private set; }
        public int? MaxUses { get; private set; }
        public int Uses { get; private set; }
        public IUser Inviter { get; private set; }

        public DateTimeOffset CreatedAt => DateTimeUtils.FromTicks(_createdAtTicks);

        public InviteMetadata(DiscordRestClient client, Model model)
            : base(client, model)
        {
            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            Inviter = new User(model.Inviter);
            IsRevoked = model.Revoked;
            IsTemporary = model.Temporary;
            MaxAge = model.MaxAge != 0 ? model.MaxAge : (int?)null;
            MaxUses = model.MaxUses;
            Uses = model.Uses;
            _createdAtTicks = model.CreatedAt.UtcTicks;
        }
    }
}
