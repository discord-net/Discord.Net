using System;
using System.Diagnostics;
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
    }
}
