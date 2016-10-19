using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Invite;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestInvite : RestEntity<string>, IInvite, IUpdateable
    {
        public string ChannelName { get; private set; }
        public string GuildName { get; private set; }
        public ulong ChannelId { get; private set; }
        public ulong GuildId { get; private set; }
        internal IChannel Channel { get; private set; }
        internal IGuild Guild { get; private set; }

        public string Code => Id;
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
            GuildId = model.Guild.Id;
            ChannelId = model.Channel.Id;
            GuildName = model.Guild.Name;
            ChannelName = model.Channel.Name;
        }
        
        public async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetInviteAsync(Code, options).ConfigureAwait(false);
            Update(model);
        }
        public Task DeleteAsync(RequestOptions options = null)
            => InviteHelper.DeleteAsync(this, Discord, options);

        public Task AcceptAsync(RequestOptions options = null)
            => InviteHelper.AcceptAsync(this, Discord, options);

        public override string ToString() => Url;
        private string DebuggerDisplay => $"{Url} ({GuildName} / {ChannelName})";
        
        IGuild IInvite.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                var guildChannel = Channel as IGuildChannel;
                if (guildChannel != null)
                    return guildChannel.Guild; //If it fails, it'll still return this exception
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }
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
