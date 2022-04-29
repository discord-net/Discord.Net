using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based voice channel in a guild.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestVoiceChannel : RestTextChannel, IVoiceChannel, IRestAudioChannel
    {
        #region RestVoiceChannel
        /// <inheritdoc />
        public int Bitrate { get; private set; }
        /// <inheritdoc />
        public int? UserLimit { get; private set; }
        /// <inheritdoc/>
        public string RTCRegion { get; private set; }

        internal RestVoiceChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id)
        {
        }
        internal new static RestVoiceChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestVoiceChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }
        /// <inheritdoc />
        internal override void Update(Model model)
        {
            base.Update(model);

            if(model.Bitrate.IsSpecified)
                Bitrate = model.Bitrate.Value;

            if(model.UserLimit.IsSpecified)
                UserLimit = model.UserLimit.Value != 0 ? model.UserLimit.Value : (int?)null;

            RTCRegion = model.RTCRegion.GetValueOrDefault(null);
        }

        /// <inheritdoc />
        public async Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot modify text channel properties of a voice channel.</exception>
        public override Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null)
            => throw new InvalidOperationException("Cannot modify text channel properties of a voice channel");

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot create a thread within a voice channel.</exception>
        public override Task<RestThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread, ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
            => throw new InvalidOperationException("Cannot create a thread within a voice channel");

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot create a webhook within a voice channel.</exception>
        public override Task<RestWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null)
            => throw new InvalidOperationException();

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot get webhooks for a voice channel.</exception>
        public override Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
            => throw new InvalidOperationException();

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot get webhooks for a voice channel.</exception>
        public override Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions options = null)
            => throw new InvalidOperationException();

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
        #endregion

        #region IAudioChannel
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Connecting to a REST-based channel is not supported.</exception>
        Task<IAudioClient> IAudioChannel.ConnectAsync(bool selfDeaf, bool selfMute, bool external) { throw new NotSupportedException(); }
        Task IAudioChannel.DisconnectAsync() { throw new NotSupportedException(); }
        Task IAudioChannel.ModifyAsync(Action<AudioChannelProperties> func, RequestOptions options) { throw new NotSupportedException(); }
        #endregion

        #region IGuildChannel
        /// <inheritdoc />
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(null);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
        #endregion

        #region INestedChannel
        /// <inheritdoc />
        async Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
        {
            if (CategoryId.HasValue && mode == CacheMode.AllowDownload)
                return (await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false)) as ICategoryChannel;
            return null;
        }
        #endregion
    }
}
