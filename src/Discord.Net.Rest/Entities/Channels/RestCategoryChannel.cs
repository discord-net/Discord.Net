using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class RestCategoryChannel : RestGuildChannel, ICategoryChannel
    {
        internal RestCategoryChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id)
        {
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Category)";

        // IGuildChannel
        Task<IInviteMetadata> IGuildChannel.CreateInviteAsync(int? maxAge, int? maxUses, bool isTemporary,
            bool isUnique, RequestOptions options)
            => throw new NotSupportedException();

        Task<IReadOnlyCollection<IInviteMetadata>> IGuildChannel.GetInvitesAsync(RequestOptions options)
            => throw new NotSupportedException();

        //IChannel
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => throw new NotSupportedException();

        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => throw new NotSupportedException();

        internal new static RestCategoryChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestCategoryChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }
    }
}
