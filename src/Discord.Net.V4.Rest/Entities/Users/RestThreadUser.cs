using System;
using System.Threading.Tasks;
using Model = Discord.API.ThreadMember;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a thread user received over the REST api.
    /// </summary>
    public class RestThreadUser : RestEntity<ulong>, IThreadUser
    {
        /// <inheritdoc/>
        public IThreadChannel Thread { get; }

        /// <inheritdoc/>
        public DateTimeOffset ThreadJoinedAt { get; private set; }

        /// <inheritdoc/>
        public IGuild Guild { get; }

        /// <inheritdoc cref="IThreadUser.GuildUser"/>
        public RestGuildUser GuildUser { get; private set; }

        /// <inheritdoc/>
        public string Mention => MentionUtils.MentionUser(Id);

        internal RestThreadUser(BaseDiscordClient discord, IGuild guild, IThreadChannel channel, ulong id)
            : base(discord, id)
        {
            Guild = guild;
            Thread = channel;
        }

        internal static RestThreadUser Create(BaseDiscordClient client, IGuild guild, Model model, IThreadChannel channel)
        {
            var entity = new RestThreadUser(client, guild, channel, model.UserId.Value);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            ThreadJoinedAt = model.JoinTimestamp;
            if (model.GuildMember.IsSpecified)
                GuildUser = RestGuildUser.Create(Discord, Guild, model.GuildMember.Value);
        }

        /// <inheritdoc />
        IGuildUser IThreadUser.GuildUser => GuildUser;

        /// <summary>
        ///     Gets the guild user for this thread user.
        /// </summary>
        /// <returns>
        ///     A task representing the asynchronous get operation. The task returns a
        ///     <see cref="IGuildUser"/> that represents the current thread user.
        /// </returns>
        public Task<IGuildUser> GetGuildUser()
            => Guild.GetUserAsync(Id);
    }
}
