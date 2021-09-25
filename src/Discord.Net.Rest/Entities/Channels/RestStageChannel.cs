using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Channel;
using StageInstance = Discord.API.StageInstance;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based stage channel in a guild.
    /// </summary>
    public class RestStageChannel : RestVoiceChannel, IStageChannel
    {
        /// <inheritdoc/>
        public string Topic { get; private set; }

        /// <inheritdoc/>
        public StagePrivacyLevel? PrivacyLevel { get; private set; }

        /// <inheritdoc/>
        public bool? DiscoverableDisabled { get; private set; }

        /// <inheritdoc/>
        public bool Live { get; private set; }
        internal RestStageChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id)
        {

        }

        internal static new RestStageChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestStageChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(StageInstance model, bool isLive = false)
        {
            Live = isLive;
            if(isLive)
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
        public async Task ModifyInstanceAsync(Action<StageInstanceProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options);

            Update(model, true);
        }

        /// <inheritdoc/>
        public async Task StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, RequestOptions options = null)
        {
            var args = new API.Rest.CreateStageInstanceParams()
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

            Update(null, false);
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
