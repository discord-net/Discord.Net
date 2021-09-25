using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Channel;
using StageInstance = Discord.API.StageInstance;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a stage channel recieved over the gateway.
    /// </summary>
    public class SocketStageChannel : SocketVoiceChannel, IStageChannel
    {
        /// <inheritdoc/>
        public string Topic { get; private set; }

        /// <inheritdoc/>
        public StagePrivacyLevel? PrivacyLevel { get; private set; }

        /// <inheritdoc/>
        public bool? DiscoverableDisabled { get; private set; }

        /// <inheritdoc/>
        public bool Live { get; private set; } = false;

        /// <summary>
        ///     Returns <see langword="true"/> if the current user is a speaker within the stage, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsSpeaker
            => !Guild.CurrentUser.IsSuppressed;

        /// <summary>
        ///     Gets a collection of users who are speakers within the stage.
        /// </summary>
        public IReadOnlyCollection<SocketGuildUser> Speakers
            => Users.Where(x => !x.IsSuppressed).ToImmutableArray();

        internal new SocketStageChannel Clone() => MemberwiseClone() as SocketStageChannel;


        internal SocketStageChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id, guild)
        {

        }

        internal new static SocketStageChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketStageChannel(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }

        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);
        }

        internal void Update(StageInstance model, bool isLive = false)
        {
            Live = isLive;
            if (isLive)
            {
                Topic = model.Topic;
                PrivacyLevel = model.PrivacyLevel;
                DiscoverableDisabled = model.DiscoverableDisabled;
            }
            else
            {
                Topic = null;
                PrivacyLevel = null;
                DiscoverableDisabled = null;
            }
        }

        /// <inheritdoc/>
        public async Task StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, RequestOptions options = null)
        {
            var args = new API.Rest.CreateStageInstanceParams()
            {
                ChannelId = Id,
                Topic = topic,
                PrivacyLevel = privacyLevel,
            };

            var model = await Discord.ApiClient.CreateStageInstanceAsync(args, options).ConfigureAwait(false);

            Update(model, true);
        }

        /// <inheritdoc/>
        public async Task ModifyInstanceAsync(Action<StageInstanceProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options);

            Update(model, true);
        }

        /// <inheritdoc/>
        public async Task StopStageAsync(RequestOptions options = null)
        {
            await Discord.ApiClient.DeleteStageInstanceAsync(Id, options);

            Update(null, false);
        }

        /// <inheritdoc/>
        public Task RequestToSpeakAsync(RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams()
            {
                ChannelId = Id,
                RequestToSpeakTimestamp = DateTimeOffset.UtcNow
            };
            return Discord.ApiClient.ModifyMyVoiceState(Guild.Id, args, options);
        }

        /// <inheritdoc/>
        public Task BecomeSpeakerAsync(RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams()
            {
                ChannelId = Id,
                Suppressed = false
            };
            return Discord.ApiClient.ModifyMyVoiceState(Guild.Id, args, options);
        }

        /// <inheritdoc/>
        public Task StopSpeakingAsync(RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams()
            {
                ChannelId = Id,
                Suppressed = true
            };
            return Discord.ApiClient.ModifyMyVoiceState(Guild.Id, args, options);
        }

        /// <inheritdoc/>
        public Task MoveToSpeakerAsync(IGuildUser user, RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams()
            {
                ChannelId = Id,
                Suppressed = false
            };

            return Discord.ApiClient.ModifyUserVoiceState(Guild.Id, user.Id, args);
        }

        /// <inheritdoc/>
        public Task RemoveFromSpeakerAsync(IGuildUser user, RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams()
            {
                ChannelId = Id,
                Suppressed = true
            };

            return Discord.ApiClient.ModifyUserVoiceState(Guild.Id, user.Id, args);
        }
    }
}
