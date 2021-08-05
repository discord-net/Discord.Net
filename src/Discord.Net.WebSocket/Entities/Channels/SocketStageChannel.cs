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
        ///     Gets a collection of users who are speakers within the stage.
        /// </summary>
        public IReadOnlyCollection<SocketGuildUser> Speakers
            => this.Users.Where(x => !x.IsSuppressed).ToImmutableArray();

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
            this.Live = isLive;
            if (isLive)
            {
                this.Topic = model.Topic;
                this.PrivacyLevel = model.PrivacyLevel;
                this.DiscoverableDisabled = model.DiscoverableDisabled;
            }
            else
            {
                this.Topic = null;
                this.PrivacyLevel = null;
                this.DiscoverableDisabled = null;
            }
        }

        /// <inheritdoc/>
        public async Task StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, RequestOptions options = null)
        {
            var args = new API.Rest.CreateStageInstanceParams()
            {
                ChannelId = this.Id,
                Topic = topic,
                PrivacyLevel = privacyLevel,
            };

            var model = await Discord.ApiClient.CreateStageInstanceAsync(args, options).ConfigureAwait(false);

            this.Update(model, true);
        }

        /// <inheritdoc/>
        public async Task ModifyInstanceAsync(Action<StageInstanceProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options);

            this.Update(model, true);
        }

        /// <inheritdoc/>
        public async Task StopStageAsync(RequestOptions options = null)
        {
            await Discord.ApiClient.DeleteStageInstanceAsync(this.Id, options);

            Update(null, false);
        }

        /// <inheritdoc/>
        public Task RequestToSpeak(RequestOptions options = null)
        {
            var args = new API.Rest.ModifyVoiceStateParams()
            {
                ChannelId = this.Id,
                RequestToSpeakTimestamp = DateTimeOffset.UtcNow
            };
            return Discord.ApiClient.ModifyMyVoiceState(this.Guild.Id, args, options);
        }
    }
}
