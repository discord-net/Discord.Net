using Discord.Rest;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a stage channel received over the gateway.
    /// </summary>
    public class SocketStageChannel : SocketVoiceChannel, IStageChannel
    {
        /// <inheritdoc/>
        /// <remarks>
        ///     This field is always true for stage channels.
        /// </remarks>
        [Obsolete("This property is no longer used because Discord enabled text-in-stage for all channels.")]
        public override bool IsTextInVoice
            => true;

        /// <inheritdoc cref="IStageChannel.StageInstance"/>
        public SocketStageInstance StageInstance => Guild.StageInstances.FirstOrDefault(x => x.ChannelId == Id);

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
            : base(discord, id, guild) { }

        internal new static SocketStageChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketStageChannel(guild?.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }

        /// <inheritdoc cref="IStageChannel.StartStageAsync" />
        public async Task<RestStageInstance> StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, bool sendStartNotification = false,
            RequestOptions options = null)
        {
            var args = new API.Rest.CreateStageInstanceParams
            {
                ChannelId = Id,
                Topic = topic,
                PrivacyLevel = privacyLevel,
                SendNotification = sendStartNotification
            };

            var model = await Discord.ApiClient.CreateStageInstanceAsync(args, options).ConfigureAwait(false);

            return RestStageInstance.Create(Discord, model);
        }

        /// <inheritdoc/>
        public Task ModifyInstanceAsync(Action<StageInstanceProperties> func, RequestOptions options = null)
            => ChannelHelper.ModifyStageAsync(this, Discord, func, options);

        /// <inheritdoc/>
        public Task StopStageAsync(RequestOptions options = null)
            => Discord.ApiClient.DeleteStageInstanceAsync(Id, options);

        /// <inheritdoc/>
        public Task RequestToSpeakAsync(RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams
            {
                ChannelId = Id,
                RequestToSpeakTimestamp = DateTimeOffset.UtcNow
            };
            return Discord.ApiClient.ModifyMyVoiceState(Guild.Id, args, options);
        }

        /// <inheritdoc/>
        public Task BecomeSpeakerAsync(RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams
            {
                ChannelId = Id,
                Suppressed = false
            };
            return Discord.ApiClient.ModifyMyVoiceState(Guild.Id, args, options);
        }

        /// <inheritdoc/>
        public Task StopSpeakingAsync(RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams
            {
                ChannelId = Id,
                Suppressed = true
            };
            return Discord.ApiClient.ModifyMyVoiceState(Guild.Id, args, options);
        }

        /// <inheritdoc/>
        public Task MoveToSpeakerAsync(IGuildUser user, RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams
            {
                ChannelId = Id,
                Suppressed = false
            };

            return Discord.ApiClient.ModifyUserVoiceState(Guild.Id, user.Id, args);
        }

        /// <inheritdoc/>
        public Task RemoveFromSpeakerAsync(IGuildUser user, RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams
            {
                ChannelId = Id,
                Suppressed = true
            };

            return Discord.ApiClient.ModifyUserVoiceState(Guild.Id, user.Id, args);
        }

        #region IStageChannel

        /// <inheritdoc/>
        async Task<IStageInstance> IStageChannel.StartStageAsync(string topic, StagePrivacyLevel privacyLevel, bool sendStartNotification, RequestOptions options)
            => await StartStageAsync(topic, privacyLevel, sendStartNotification, options);

        /// <inheritdoc/>
        IStageInstance IStageChannel.StageInstance => StageInstance;

        #endregion
    }
}
