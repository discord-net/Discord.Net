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
        /// <summary>
        ///     Gets whether or not the guild has Text-In-Voice enabled and the voice channel is a TiV channel.
        /// </summary>
        /// <remarks>
        ///     Discord currently doesn't have a way to disable Text-In-Voice yet so this field is always
        ///     <see langword="true"/> on <see cref="SocketVoiceChannel"/>s and <see langword="true"/> on
        ///     <see cref="SocketStageChannel"/>s.
        /// </remarks>
        [Obsolete("This property is no longer used because Discord enabled text-in-voice for all channels.")]
        public virtual bool IsTextInVoice => true;

        /// <inheritdoc />
        public int Bitrate { get; private set; }
        /// <inheritdoc />
        public int? UserLimit { get; private set; }
        /// <inheritdoc />
        public string RTCRegion { get; private set; }
        /// <inheritdoc/>
        public VideoQualityMode VideoQualityMode { get; private set; }

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
            var entity = new SocketVoiceChannel(guild?.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }
        /// <inheritdoc />
        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);
            Bitrate = model.Bitrate.GetValueOrDefault(64000);
            UserLimit = model.UserLimit.GetValueOrDefault() != 0 ? model.UserLimit.Value : (int?)null;
            VideoQualityMode = model.VideoQualityMode.GetValueOrDefault(VideoQualityMode.Auto);
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

        /// <inheritdoc/> <exception cref="InvalidOperationException">Cannot create threads in voice channels.</exception>
        public override Task<SocketThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread, ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
            => throw new InvalidOperationException("Voice channels cannot contain threads.");

        #endregion

        #region TextOverrides

        /// <inheritdoc/> <exception cref="NotSupportedException">Threads are not supported in voice channels</exception>
        public override Task<IReadOnlyCollection<RestThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null)
            => throw new NotSupportedException("Threads are not supported in voice channels");

        #endregion

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
        internal new SocketVoiceChannel Clone() => MemberwiseClone() as SocketVoiceChannel;

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
