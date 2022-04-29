using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based voice channel in a guild.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketVoiceChannel : SocketTextChannel, IVoiceChannel, ISocketAudioChannel
    {
        #region SocketVoiceChannel
        /// <inheritdoc />
        public int Bitrate { get; private set; }
        /// <inheritdoc />
        public int? UserLimit { get; private set; }
        /// <inheritdoc/>
        public string RTCRegion { get; private set; }

        /// <inheritdoc />
        public ulong? CategoryId { get; private set; }
        /// <summary>
        ///     Gets the parent (category) channel of this channel.
        /// </summary>
        /// <returns>
        ///     A category channel representing the parent of this channel; <c>null</c> if none is set.
        /// </returns>
        public ICategoryChannel Category
            => CategoryId.HasValue ? Guild.GetChannel(CategoryId.Value) as ICategoryChannel : null;

        /// <inheritdoc />
        public string Mention => MentionUtils.MentionChannel(Id);

        /// <inheritdoc />
        public Task SyncPermissionsAsync(RequestOptions options = null)
            => ChannelHelper.SyncPermissionsAsync(this, Discord, options);

        /// <summary>
        ///     Gets a collection of users that are currently connected to this voice channel.
        /// </summary>
        /// <returns>
        ///     A read-only collection of users that are currently connected to this voice channel.
        /// </returns>
        public IReadOnlyCollection<SocketGuildUser> ConnectedUsers
            => Guild.Users.Where(x => x.VoiceChannel?.Id == Id).ToImmutableArray();

        internal SocketVoiceChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id, guild)
        {
        }
        internal new static SocketVoiceChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketVoiceChannel(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }
        /// <inheritdoc />
        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);
            Bitrate = model.Bitrate.Value;
            UserLimit = model.UserLimit.Value != 0 ? model.UserLimit.Value : (int?)null;
            RTCRegion = model.RTCRegion.GetValueOrDefault(null);
        }

        /// <inheritdoc />
        public Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null)
            => ChannelHelper.ModifyAsync(this, Discord, func, options);

        /// <inheritdoc />
        public async Task<IAudioClient> ConnectAsync(bool selfDeaf = false, bool selfMute = false, bool external = false)
        {
            return await Guild.ConnectAudioAsync(Id, selfDeaf, selfMute, external).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DisconnectAsync()
            => await Guild.DisconnectAudioAsync();

        /// <inheritdoc />
        public async Task ModifyAsync(Action<AudioChannelProperties> func, RequestOptions options = null)
        {
            await Guild.ModifyAudioAsync(Id, func, options).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override SocketGuildUser GetUser(ulong id)
        {
            var user = Guild.GetUser(id);
            if (user?.VoiceChannel?.Id == Id)
                return user;
            return null;
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot create threads in voice channels.</exception>
        public override Task<SocketThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread, ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
            => throw new InvalidOperationException("Voice channels cannot contain threads.");
        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot create webhooks in voice channels.</exception>
        public override Task<RestWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null)
            => throw new InvalidOperationException("Voice channels cannot contain webhooks.");
        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot modify text channel properties within a webhook.</exception>
        public override Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null)
            => throw new InvalidOperationException("Cannot modify text channel properties for voice channels.");
        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot get webhooks for a voice channel.</exception>
        public override Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
            => throw new InvalidOperationException("Cannot get webhooks for a voice channel.");
        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot get webhooks for a voice channel.</exception>
        public override Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions options = null)
            => throw new InvalidOperationException("Cannot get webhooks for a voice channel.");
        #endregion

        #region Invites
        /// <inheritdoc />
        public async Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);
        /// <inheritdoc />
        public async Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteToApplicationAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, applicationId, options).ConfigureAwait(false);
        /// <inheritdoc />
        public virtual async Task<IInviteMetadata> CreateInviteToApplicationAsync(DefaultApplications application, int? maxAge = 86400, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteToApplicationAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, (ulong)application, options);
        /// <inheritdoc />
        public async Task<IInviteMetadata> CreateInviteToStreamAsync(IUser user, int? maxAge, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteToStreamAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, user, options).ConfigureAwait(false);
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => await ChannelHelper.GetInvitesAsync(this, Discord, options).ConfigureAwait(false);

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
        internal new SocketVoiceChannel Clone() => MemberwiseClone() as SocketVoiceChannel;
        #endregion

        #region IGuildChannel
        /// <inheritdoc />
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(GetUser(id));
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
        #endregion

        #region INestedChannel
        /// <inheritdoc />
        Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult(Category);
        #endregion
    }
}
