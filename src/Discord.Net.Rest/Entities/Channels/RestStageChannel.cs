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
        ///     This field is always false for stage channels.
        /// </remarks>
        public override bool IsTextInVoice
            => false;

        /// <inheritdoc/>
        public StagePrivacyLevel? PrivacyLevel { get; private set; }

        /// <inheritdoc/>
        public bool? IsDiscoverableDisabled { get; private set; }

        /// <inheritdoc/>
        public bool IsLive { get; private set; }
        internal RestStageChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id) { }

        internal new static RestStageChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestStageChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(StageInstance model, bool isLive = false)
        {
            IsLive = isLive;
            if(isLive)
            {
                PrivacyLevel = model.PrivacyLevel;
                IsDiscoverableDisabled = model.DiscoverableDisabled;
            }
            else
            {
                PrivacyLevel = null;
                IsDiscoverableDisabled = null;
            }
        }

        /// <inheritdoc/>
        public async Task ModifyInstanceAsync(Action<StageInstanceProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options);

            Update(model, true);
        }

        /// <inheritdoc/>
        public async Task StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, RequestOptions options = null)
        {
            var args = new CreateStageInstanceParams
            {
                ChannelId = Id,
                PrivacyLevel = privacyLevel,
                Topic = topic
            };

            var model = await Discord.ApiClient.CreateStageInstanceAsync(args, options);

            Update(model, true);
        }

        /// <inheritdoc/>
        public async Task StopStageAsync(RequestOptions options = null)
        {
            await Discord.ApiClient.DeleteStageInstanceAsync(Id, options);

            Update(null);
        }

        /// <inheritdoc/>
        public override async Task UpdateAsync(RequestOptions options = null)
        {
            await base.UpdateAsync(options);

            var model = await Discord.ApiClient.GetStageInstanceAsync(Id, options);

            Update(model, model != null);
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
    }
}
