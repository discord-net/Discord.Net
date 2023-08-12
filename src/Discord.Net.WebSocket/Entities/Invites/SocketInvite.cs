using Discord.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.InviteCreateEvent;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based invite to a guild.
    /// </summary>
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
        ChannelType IInvite.ChannelType
        {
            get
            {
                return Channel switch
                {
                    IVoiceChannel voiceChannel => ChannelType.Voice,
                    ICategoryChannel categoryChannel => ChannelType.Category,
                    IDMChannel dmChannel => ChannelType.DM,
                    IGroupChannel groupChannel => ChannelType.Group,
                    INewsChannel newsChannel => ChannelType.News,
                    ITextChannel textChannel => ChannelType.Text,
                    _ => throw new InvalidOperationException("Invalid channel type."),
                };
            }
        }
        /// <inheritdoc />
        string IInvite.ChannelName => Channel.Name;
        /// <inheritdoc />
        string IInvite.GuildName => Guild.Name;
        /// <inheritdoc />
        int? IInvite.PresenceCount => throw new NotImplementedException();
        /// <inheritdoc />
        int? IInvite.MemberCount => throw new NotImplementedException();
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
        /// <summary>
        ///     Gets the user targeted by this invite if available.
        /// </summary>
        public SocketUser TargetUser { get; private set; }
        /// <summary>
        ///     Gets the type of the user targeted by this invite.
        /// </summary>
        public TargetUserType TargetUserType { get; private set; }

        /// <inheritdoc cref="IInvite.Application" />
        public RestApplication Application { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset? ExpiresAt { get; private set; }
        
        /// <inheritdoc />
        public string Code => Id;
        /// <inheritdoc />
        public string Url => $"{DiscordConfig.InviteUrl}{Code}";

        internal SocketInvite(DiscordSocketClient discord, SocketGuild guild, SocketGuildChannel channel, SocketGuildUser inviter, SocketUser target, string id)
            : base(discord, id)
        {
            Guild = guild;
            Channel = channel;
            Inviter = inviter;
            TargetUser = target;
        }
        internal static SocketInvite Create(DiscordSocketClient discord, SocketGuild guild, SocketGuildChannel channel, SocketGuildUser inviter, SocketUser target, Model model)
        {
            var entity = new SocketInvite(discord, guild, channel, inviter, target, model.Code);
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
            TargetUserType = model.TargetUserType.IsSpecified ? model.TargetUserType.Value : TargetUserType.Undefined;
            ExpiresAt = model.ExpiresAt.IsSpecified ? model.ExpiresAt.Value : null;
            Application = model.Application.IsSpecified ? RestApplication.Create(Discord, model.Application.Value) : null;
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

        #region IInvite

        /// <inheritdoc />
        IGuild IInvite.Guild => Guild;
        /// <inheritdoc />
        IChannel IInvite.Channel => Channel;
        /// <inheritdoc />
        IUser IInvite.Inviter => Inviter;
        /// <inheritdoc />
        IUser IInvite.TargetUser => TargetUser;

        /// <inheritdoc />
        IApplication IInvite.Application => Application;
        
        #endregion
    }
}
