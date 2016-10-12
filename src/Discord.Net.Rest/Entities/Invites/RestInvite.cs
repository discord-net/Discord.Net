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

        public string Code => Id;
        public string Url => $"{DiscordConfig.InviteUrl}/{Code}";

        internal RestInvite(BaseDiscordClient discord, string id)
            : base(discord, id)
        {
        }
        internal static RestInvite Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestInvite(discord, model.Code);
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

        string IEntity<string>.Id => Code;
    }
}
