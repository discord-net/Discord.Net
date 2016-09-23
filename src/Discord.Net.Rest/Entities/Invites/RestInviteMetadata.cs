using System;
using System.Diagnostics;
using Model = Discord.API.InviteMetadata;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestInviteMetadata : RestInvite, IInviteMetadata
    {
        private long _createdAtTicks;

        public bool IsRevoked { get; private set; }
        public bool IsTemporary { get; private set; }
        public int? MaxAge { get; private set; }
        public int? MaxUses { get; private set; }
        public int Uses { get; private set; }
        public RestUser Inviter { get; private set; }

        public DateTimeOffset CreatedAt => DateTimeUtils.FromTicks(_createdAtTicks);

        internal RestInviteMetadata(DiscordRestClient discord, string id)
            : base(discord, id)
        {
        }
        internal static RestInviteMetadata Create(DiscordRestClient discord, Model model)
        {
            var entity = new RestInviteMetadata(discord, model.Code);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            base.Update(model);
            Inviter = RestUser.Create(Discord, model.Inviter);
            IsRevoked = model.Revoked;
            IsTemporary = model.Temporary;
            MaxAge = model.MaxAge != 0 ? model.MaxAge : (int?)null;
            MaxUses = model.MaxUses;
            Uses = model.Uses;
            _createdAtTicks = model.CreatedAt.UtcTicks;
        }

        IUser IInviteMetadata.Inviter => Inviter;
    }
}
