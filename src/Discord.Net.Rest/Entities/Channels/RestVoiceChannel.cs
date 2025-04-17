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
        /// <summary>
        ///     Gets whether or not the guild has Text-In-Voice enabled and the voice channel is a TiV channel.
        /// </summary>
        [Obsolete("This property is no longer used because Discord enabled text-in-voice for all channels.")]
        public virtual bool IsTextInVoice => true;

        /// <inheritdoc />
        public int Bitrate { get; private set; }
        /// <inheritdoc />
        public int? UserLimit { get; private set; }
        /// <inheritdoc/>
        public string RTCRegion { get; private set; }
        /// <inheritdoc/>
        public VideoQualityMode VideoQualityMode { get; private set; }

        internal RestVoiceChannel(BaseDiscordClient discord, IGuild guild, ulong id, ulong guildId)
            : base(discord, guild, id, guildId)
        {
        }
        internal new static RestVoiceChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestVoiceChannel(discord, guild, model.Id, guild?.Id ?? model.GuildId.Value);
            entity.Update(model);
            return entity;
        }
        /// <inheritdoc />
        internal override void Update(Model model)
        {
            base.Update(model);

            if (model.Bitrate.IsSpecified)
                Bitrate = model.Bitrate.Value;

            if (model.UserLimit.IsSpecified)
                UserLimit = model.UserLimit.Value != 0 ? model.UserLimit.Value : (int?)null;

            VideoQualityMode = model.VideoQualityMode.GetValueOrDefault(VideoQualityMode.Auto);

            RTCRegion = model.RTCRegion.GetValueOrDefault(null);
        }

        /// <inheritdoc />
        public async Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot create a thread within a voice channel.</exception>
        public override Task<RestThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread, ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
            => throw new InvalidOperationException("Cannot create a thread within a voice channel");

        /// <inheritdoc />
        public virtual Task SetStatusAsync(string status, RequestOptions options = null)
            => ChannelHelper.ModifyVoiceChannelStatusAsync(this, status, Discord, options);

        #endregion

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";

        #region TextOverrides

        /// <inheritdoc/> <exception cref="NotSupportedException">Threads are not supported in voice channels</exception>
        public override Task<IReadOnlyCollection<RestThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null)
            => throw new NotSupportedException("Threads are not supported in voice channels");

        #endregion


        #region IAudioChannel
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Connecting to a REST-based channel is not supported.</exception>
        Task<IAudioClient> IAudioChannel.ConnectAsync(bool selfDeaf, bool selfMute, bool external, bool disconnect) { throw new NotSupportedException(); }
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
