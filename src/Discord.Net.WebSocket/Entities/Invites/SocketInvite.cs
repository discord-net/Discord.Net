using Discord.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.InviteCreateEvent;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketInvite : SocketEntity<string>, IInviteMetadata
    {
         private long _createdAtTicks;

        /// <inheritdoc />
        public ulong ChannelId { get; private set; }
        /// <summary>
        ///     Gets the channel where this invite was created.
        /// </summary>
        public SocketGuildChannel Channel { get; private set; }
        /// <inheritdoc />
        public ulong? GuildId { get; private set; }
        /// <summary>
        ///     Gets the guild where this invite was created.
        /// </summary>
        public SocketGuild Guild { get; private set; }
        /// <inheritdoc />
        ChannelType IInvite.ChannelType => throw new NotImplementedException();
        /// <inheritdoc />
        string IInvite.ChannelName => throw new NotImplementedException();
        /// <inheritdoc />
        string IInvite.GuildName => throw new NotImplementedException();
        /// <inheritdoc />
        int? IInvite.PresenceCount => throw new NotImplementedException();
        /// <inheritdoc />
        int? IInvite.MemberCount => throw new NotImplementedException();
        /// <inheritdoc />
        bool IInviteMetadata.IsRevoked => throw new NotImplementedException();
        /// <inheritdoc />
        public bool IsTemporary { get; private set; }
        /// <inheritdoc />
        int? IInviteMetadata.MaxAge { get => MaxAge; }
        /// <inheritdoc />
        int? IInviteMetadata.MaxUses { get => MaxUses; }
        /// <inheritdoc />
        int? IInviteMetadata.Uses { get => Uses; }
        /// <summary>
        ///     Gets the time (in seconds) until the invite expires.
        /// </summary>
        public int MaxAge { get; private set; }
        /// <summary>
        ///     Gets the max number of uses this invite may have.
        /// </summary>
        public int MaxUses { get; private set; }
        /// <summary>
        ///     Gets the number of times this invite has been used.
        /// </summary>
        public int Uses { get; private set; }
        /// <summary>
        ///     Gets the user that created this invite if available.
        /// </summary>
        public SocketGuildUser Inviter { get; private set; }
        /// <inheritdoc />
        DateTimeOffset? IInviteMetadata.CreatedAt => DateTimeUtils.FromTicks(_createdAtTicks);
        /// <summary>
        ///     Gets when this invite was created.
        /// </summary>
        public DateTimeOffset CreatedAt => DateTimeUtils.FromTicks(_createdAtTicks);

        /// <inheritdoc />
        public string Code => Id;
        /// <inheritdoc />
        public string Url => $"{DiscordConfig.InviteUrl}{Code}";

        internal SocketInvite(DiscordSocketClient discord, SocketGuild guild, SocketGuildChannel channel, SocketGuildUser inviter, string id)
            : base(discord, id)
        {
            Guild = guild;
            Channel = channel;
            Inviter = inviter;
        }
        internal static SocketInvite Create(DiscordSocketClient discord, SocketGuild guild, SocketGuildChannel channel, SocketGuildUser inviter, Model model)
        {
            var entity = new SocketInvite(discord, guild, channel, inviter, model.Code);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            ChannelId = model.ChannelId;
            GuildId = model.GuildId.IsSpecified ? model.GuildId.Value : Guild.Id;
            IsTemporary = model.Temporary;
            MaxAge = model.MaxAge;
            MaxUses = model.MaxUses;
            Uses = model.Uses;
            _createdAtTicks = model.CreatedAt.UtcTicks;
        }

        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => InviteHelper.DeleteAsync(this, Discord, options);

        /// <summary>
        ///     Gets the URL of the invite.
        /// </summary>
        /// <returns>
        ///     A string that resolves to the Url of the invite.
        /// </returns>
        public override string ToString() => Url;
        private string DebuggerDisplay => $"{Url} ({Guild?.Name} / {Channel.Name})";

        /// <inheritdoc />
        IGuild IInvite.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                if (Channel is IGuildChannel guildChannel)
                    return guildChannel.Guild; //If it fails, it'll still return this exception
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }
        /// <inheritdoc />
        IChannel IInvite.Channel
        {
            get
            {
                if (Channel != null)
                    return Channel;
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }

        /// <inheritdoc />
        IUser IInviteMetadata.Inviter => Inviter;
    }
}
