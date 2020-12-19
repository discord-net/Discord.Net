using System;
using Model = Discord.API.InviteMetadata;

namespace Discord.Rest
{
    /// <summary> Represents additional information regarding the REST-based invite object. </summary>
    public class RestInviteMetadata : RestInvite, IInviteMetadata
    {
        private long _createdAtTicks;

        /// <inheritdoc />
        [Obsolete("This property doesn't exist anymore and shouldn't be used.")]
        public bool IsRevoked { get; private set; }
        /// <inheritdoc />
        public bool IsTemporary { get; private set; }
        /// <inheritdoc />
        public int? MaxAge { get; private set; }
        /// <inheritdoc />
        public int? MaxUses { get; private set; }
        /// <inheritdoc />
        public int? Uses { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset? CreatedAt => DateTimeUtils.FromTicks(_createdAtTicks);

        internal RestInviteMetadata(BaseDiscordClient discord, IGuild guild, IChannel channel, string id)
            : base(discord, guild, channel, id)
        {
        }
        internal static RestInviteMetadata Create(BaseDiscordClient discord, IGuild guild, IChannel channel, Model model)
        {
            var entity = new RestInviteMetadata(discord, guild, channel, model.Code);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            base.Update(model);
            IsTemporary = model.Temporary;
            MaxAge = model.MaxAge;
            MaxUses = model.MaxUses;
            Uses = model.Uses;
            _createdAtTicks = model.CreatedAt.UtcTicks;
        }
    }
}
