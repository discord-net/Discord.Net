using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestGuildChannelCategory : RestGuildChannel, IGuildChannelCategory
    {
        public string Mention => MentionUtils.MentionChannel(Id);

        internal RestGuildChannelCategory(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id)
        {
        }
        internal new static RestGuildChannelCategory Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestGuildChannelCategory(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);
        }

        public Task<RestGuildUser> GetUserAsync(ulong id, RequestOptions options = null)
            => ChannelHelper.GetUserAsync(this, Guild, Discord, id, options);
        public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(RequestOptions options = null)
            => ChannelHelper.GetUsersAsync(this, Guild, Discord, null, null, options);

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";
    }
}
