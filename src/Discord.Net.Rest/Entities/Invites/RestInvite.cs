using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Invite;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestInvite : RestEntity<string>, IInvite, IUpdateable
    {
        public ChannelType ChannelType { get; private set; }
        /// <inheritdoc />
        public string ChannelName { get; private set; }
        /// <inheritdoc />
        public string GuildName { get; private set; }
        /// <inheritdoc />
        public int? PresenceCount { get; private set; }
        /// <inheritdoc />
        public int? MemberCount { get; private set; }
        /// <inheritdoc />
        public ulong ChannelId { get; private set; }
        /// <inheritdoc />
        public ulong? GuildId { get; private set; }
        /// <inheritdoc />
        public IUser Inviter { get; private set; }
        /// <inheritdoc />
        public IUser TargetUser { get; private set; }
        /// <inheritdoc />
        public TargetUserType TargetUserType { get; private set; }

        /// <summary>
        ///     Gets the guild this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A partial guild object representing the guild that the invite points to.
        /// </returns>
        public InviteGuild InviteGuild { get; private set; }

        /// <inheritdoc cref="IInvite.Application" />
        public RestApplication Application { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset? ExpiresAt { get; private set; }

        /// <summary>
        ///     Gets guild scheduled event data. <see langword="null" /> if event id was invalid.
        /// </summary>
        public RestGuildEvent ScheduledEvent { get; private set; }

        internal IChannel Channel { get; }

        internal IGuild Guild { get; }

        /// <inheritdoc />
        public string Code => Id;
        /// <inheritdoc />
        public string Url => $"{DiscordConfig.InviteUrl}{Code}";

        internal RestInvite(BaseDiscordClient discord, IGuild guild, IChannel channel, string id)
            : base(discord, id)
        {
            Guild = guild;
            Channel = channel;
        }
        internal static RestInvite Create(BaseDiscordClient discord, IGuild guild, IChannel channel, Model model)
        {
            var entity = new RestInvite(discord, guild, channel, model.Code);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            GuildId = model.Guild.IsSpecified ? model.Guild.Value.Id : default(ulong?);
            ChannelId = model.Channel.Id;
            GuildName = model.Guild.IsSpecified ? model.Guild.Value.Name : null;
            ChannelName = model.Channel.Name;
            MemberCount = model.MemberCount.IsSpecified ? model.MemberCount.Value : null;
            PresenceCount = model.PresenceCount.IsSpecified ? model.PresenceCount.Value : null;
            ChannelType = (ChannelType)model.Channel.Type;
            Inviter = model.Inviter.IsSpecified ? RestUser.Create(Discord, model.Inviter.Value) : null;
            TargetUser = model.TargetUser.IsSpecified ? RestUser.Create(Discord, model.TargetUser.Value) : null;
            TargetUserType = model.TargetUserType.IsSpecified ? model.TargetUserType.Value : TargetUserType.Undefined;

            if (model.Guild.IsSpecified)
            {
                InviteGuild = new InviteGuild
                (model.Guild.Value.Id,
                    model.Guild.Value.Name,
                    model.Guild.Value.Description.IsSpecified ? model.Guild.Value.Description.Value : null,
                    model.Guild.Value.Splash.IsSpecified ? model.Guild.Value.Splash.Value : null,
                    model.Guild.Value.BannerHash.IsSpecified ? model.Guild.Value.BannerHash.Value : null,
                    model.Guild.Value.Features,
                    model.Guild.Value.IconHash.IsSpecified ? model.Guild.Value.IconHash.Value : null,
                    model.Guild.Value.VerificationLevel,
                    model.Guild.Value.VanityUrlCode.IsSpecified ? model.Guild.Value.VanityUrlCode.Value : null,
                    model.Guild.Value.PremiumSubscriptionCount.GetValueOrDefault(0),
                    model.Guild.Value.NsfwLevel,
                    model.Guild.Value.WelcomeScreen.IsSpecified
                        ? new WelcomeScreen(
                            model.Guild.Value.WelcomeScreen.Value.Description.IsSpecified ? model.Guild.Value.WelcomeScreen.Value.Description.Value : null,
                            model.Guild.Value.WelcomeScreen.Value.WelcomeChannels.Select(ch =>
                                new WelcomeScreenChannel(
                                    ch.ChannelId,
                                    ch.Description,
                                    ch.EmojiName.IsSpecified ? ch.EmojiName.Value : null,
                                    ch.EmojiId.IsSpecified ? ch.EmojiId.Value : null)).ToImmutableArray())
                        : null);
            }

            if(model.Application.IsSpecified)
                Application = RestApplication.Create(Discord, model.Application.Value);

            ExpiresAt = model.ExpiresAt.IsSpecified ? model.ExpiresAt.Value : null;

            if(model.ScheduledEvent.IsSpecified)
                ScheduledEvent = RestGuildEvent.Create(Discord, Guild, model.ScheduledEvent.Value);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetInviteAsync(Code, options).ConfigureAwait(false);
            Update(model);
        }
        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => InviteHelper.DeleteAsync(this, Discord, options);

        /// <summary>
        ///     Gets the URL of the invite.
        /// </summary>
        /// <returns>
        ///     A string that resolves to the Url of the invite.
        /// </returns>
        public override string ToString() => Url;
        private string DebuggerDisplay => $"{Url} ({GuildName} / {ChannelName})";

        #region IInvite

        /// <inheritdoc />
        IGuild IInvite.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                if (Channel is IGuildChannel guildChannel)
                    return guildChannel.Guild; //If it fails, it'll still return this exception
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }
        /// <inheritdoc />
        IChannel IInvite.Channel
        {
            get
            {
                if (Channel != null)
                    return Channel;
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }

        /// <inheritdoc />
        IApplication IInvite.Application => Application;
        
        #endregion
    }
}
