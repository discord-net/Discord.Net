using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based category channel.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestCategoryChannel : RestGuildChannel, ICategoryChannel
    {
        #region RestCategoryChannel
        internal RestCategoryChannel(BaseDiscordClient discord, IGuild guild, ulong id, ulong guildId)
            : base(discord, guild, id, guildId)
        {
        }
        internal new static RestCategoryChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestCategoryChannel(discord, guild, model.Id, guild?.Id ?? model.GuildId.Value);
            entity.Update(model);
            return entity;
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Category)";
        #endregion

        #region IChannel
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">This method is not supported with category channels.</exception>
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => throw new NotSupportedException();
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">This method is not supported with category channels.</exception>
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => throw new NotSupportedException();
        #endregion
    }
}
