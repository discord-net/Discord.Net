using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Invite;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class RestInvite : RestEntity<string>, IInvite, IUpdateable
    {
        internal RestInvite(BaseDiscordClient discord, IGuild guild, IChannel channel, string id)
            : base(discord, id)
        {
            Guild = guild;
            Channel = channel;
        }

        internal IChannel Channel { get; }
        internal IGuild Guild { get; }
        private string DebuggerDisplay => $"{Url} ({GuildName} / {ChannelName})";
        public ChannelType ChannelType { get; private set; }
        public string ChannelName { get; private set; }
        public string GuildName { get; private set; }
        public int? PresenceCount { get; private set; }
        public int? MemberCount { get; private set; }
        public ulong ChannelId { get; private set; }
        public ulong? GuildId { get; private set; }

        public string Code => Id;
        public string Url => $"{DiscordConfig.InviteUrl}{Code}";

        public Task DeleteAsync(RequestOptions options = null)
            => InviteHelper.DeleteAsync(this, Discord, options);

        IGuild IInvite.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                if (Channel is IGuildChannel guildChannel)
                    return guildChannel.Guild; //If it fails, it'll still return this exception
                throw new InvalidOperationException(
                    "Unable to return this entity's parent unless it was fetched through that object.");
            }
        }

        IChannel IInvite.Channel
        {
            get
            {
                if (Channel != null)
                    return Channel;
                throw new InvalidOperationException(
                    "Unable to return this entity's parent unless it was fetched through that object.");
            }
        }

        public async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetInviteAsync(Code, options).ConfigureAwait(false);
            Update(model);
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

        public override string ToString() => Url;
    }
}
