using Discord.API;
using Discord.API.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based stage channel in a guild.
    /// </summary>
    public class RestStageChannel : RestVoiceChannel, IStageChannel
    {
        /// <inheritdoc/>
        /// <remarks>
        ///     This field is always true for stage channels.
        /// </remarks>
        /// 
        [Obsolete("This property is no longer used because Discord enabled text-in-voice and text-in-stage for all channels.")]
        public override bool IsTextInVoice
            => true;

        /// <inheritdoc cref="IStageChannel.StageInstance" />
        /// <remarks>
        ///     This property might be <see langword="null" /> even if the stage is actually running. Use <see cref="UpdateAsync"/> to
        ///     update entity's state.
        /// </remarks>
        public RestStageInstance StageInstance { get; set; }

        internal RestStageChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id) { }

        internal new static RestStageChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestStageChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void UpdateStageInstance(StageInstance model)
        {
            if (model is null)
            {
                StageInstance = null;
                return;
            }

            if (StageInstance is null)
                StageInstance = RestStageInstance.Create(Discord, model, this);
            else
                StageInstance.Update(model);
        }

        /// <inheritdoc/>
        public async Task ModifyInstanceAsync(Action<StageInstanceProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyStageAsync(this, Discord, func, options);

            UpdateStageInstance(model);
        }

        /// <inheritdoc cref="IStageChannel.StartStageAsync" />
        public async Task<RestStageInstance> StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, bool sendStartNotification = false,
            RequestOptions options = null)
        {
            var args = new CreateStageInstanceParams
            {
                ChannelId = Id,
                PrivacyLevel = privacyLevel,
                Topic = topic,
                SendNotification = sendStartNotification
            };

            var model = await Discord.ApiClient.CreateStageInstanceAsync(args, options);

            StageInstance = RestStageInstance.Create(Discord, model, this);

            return StageInstance;
        }

        /// <inheritdoc/>
        public async Task StopStageAsync(RequestOptions options = null)
        {
            await Discord.ApiClient.DeleteStageInstanceAsync(Id, options);

            UpdateStageInstance(null);
        }

        /// <inheritdoc/>
        public override async Task UpdateAsync(RequestOptions options = null)
        {
            await base.UpdateAsync(options);
            
            UpdateStageInstance(await Discord.ApiClient.GetStageInstanceAsync(Id, options));
        }

        /// <inheritdoc/>
        public Task RequestToSpeakAsync(RequestOptions options = null)
        {
            var args = new ModifyVoiceStateParams
            {
                ChannelId = Id,
                RequestToSpeakTimestamp = DateTimeOffset.UtcNow
            };
            return Discord.ApiClient.ModifyMyVoiceState(Guild.Id, args, options);
        }

        /// <inheritdoc/>
        public Task BecomeSpeakerAsync(RequestOptions options = null)
        {
            var args = new ModifyVoiceStateParams
            {
                ChannelId = Id,
                Suppressed = false
            };
            return Discord.ApiClient.ModifyMyVoiceState(Guild.Id, args, options);
        }

        /// <inheritdoc/>
        public Task StopSpeakingAsync(RequestOptions options = null)
        {
            var args = new ModifyVoiceStateParams
            {
                ChannelId = Id,
                Suppressed = true
            };
            return Discord.ApiClient.ModifyMyVoiceState(Guild.Id, args, options);
        }

        /// <inheritdoc/>
        public Task MoveToSpeakerAsync(IGuildUser user, RequestOptions options = null)
        {
            var args = new ModifyVoiceStateParams
            {
                ChannelId = Id,
                Suppressed = false
            };

            return Discord.ApiClient.ModifyUserVoiceState(Guild.Id, user.Id, args);
        }

        /// <inheritdoc/>
        public Task RemoveFromSpeakerAsync(IGuildUser user, RequestOptions options = null)
        {
            var args = new ModifyVoiceStateParams
            {
                ChannelId = Id,
                Suppressed = true
            };

            return Discord.ApiClient.ModifyUserVoiceState(Guild.Id, user.Id, args);
        }

        #region IStageChannel

        /// <inheritdoc/>
        IStageInstance IStageChannel.StageInstance => StageInstance;

        /// <inheritdoc/>
        async Task<IStageInstance> IStageChannel.StartStageAsync(string topic, StagePrivacyLevel privacyLevel, bool sendStartNotification, RequestOptions options)
            => await StartStageAsync(topic, privacyLevel, sendStartNotification, options);

        #endregion
    }
}
