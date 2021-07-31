using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ThreadMember;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a thread user recieved over the REST api.
    /// </summary>
    public class RestThreadUser : RestEntity<ulong>
    {
        /// <summary>
        ///     Gets the <see cref="RestThreadChannel"/> this user is in.
        /// </summary>
        public IThreadChannel Thread { get; }

        /// <summary>
        ///     Gets the timestamp for when this user joined this thread.
        /// </summary>
        public DateTimeOffset JoinedAt { get; private set; }

        /// <summary>
        ///     Gets the guild this user is in.
        /// </summary>
        public IGuild Guild { get; }

        internal RestThreadUser(BaseDiscordClient discord, IGuild guild, IThreadChannel channel, ulong id)
            : base(discord, id)
        {
            this.Guild = guild;
            this.Thread = channel;
        }

        internal static RestThreadUser Create(BaseDiscordClient client, IGuild guild, Model model, IThreadChannel channel)
        {
            var entity = new RestThreadUser(client, guild, channel, model.UserId.Value);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            this.JoinedAt = model.JoinTimestamp;
        }

        /// <summary>
        ///     Gets the guild user for this thread user.
        /// </summary>
        /// <returns>
        ///     A task representing the asyncronous get operation. The task returns a
        ///     <see cref="IGuildUser"/> that represents the current thread user.
        /// </returns>
        public Task<IGuildUser> GetGuildUser()
            => Guild.GetUserAsync(this.Id);
    }
}
