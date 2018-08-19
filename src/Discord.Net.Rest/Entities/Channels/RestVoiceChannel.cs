using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.Audio;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class RestVoiceChannel : RestGuildChannel, IVoiceChannel, IRestAudioChannel
    {
        internal RestVoiceChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id)
        {
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
        public int Bitrate { get; private set; }
        public int? UserLimit { get; private set; }
        public ulong? CategoryId { get; private set; }

        public async Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

        //IAudioChannel
        Task<IAudioClient> IAudioChannel.ConnectAsync(bool selfDeaf, bool selfMute, bool external) =>
            throw new NotSupportedException();

        Task IAudioChannel.DisconnectAsync() => throw new NotSupportedException();

        //IGuildChannel
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(null);

        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode,
            RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

        // INestedChannel
        async Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
        {
            if (CategoryId.HasValue && mode == CacheMode.AllowDownload)
                return await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false) as
                    ICategoryChannel;
            return null;
        }

        internal new static RestVoiceChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestVoiceChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            base.Update(model);
            CategoryId = model.CategoryId;
            Bitrate = model.Bitrate.Value;
            UserLimit = model.UserLimit.Value != 0 ? model.UserLimit.Value : (int?)null;
        }

        public Task<ICategoryChannel> GetCategoryAsync(RequestOptions options = null)
            => ChannelHelper.GetCategoryAsync(this, Discord, options);
    }
}
