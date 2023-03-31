using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.GuildScheduledEvent;

namespace Discord.Rest
{
    public class RestGuildEvent : RestEntity<ulong>, IGuildScheduledEvent
    {
        /// <inheritdoc/>
        public IGuild Guild { get; private set; }

        /// <inheritdoc/>
        public ulong GuildId { get; private set; }

        /// <inheritdoc/>
        public ulong? ChannelId { get; private set; }

        /// <inheritdoc/>
        public IUser Creator { get; private set; }

        /// <inheritdoc/>
        public ulong CreatorId { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public string CoverImageId { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset StartTime { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset? EndTime { get; private set; }

        /// <inheritdoc/>
        public GuildScheduledEventPrivacyLevel PrivacyLevel { get; private set; }

        /// <inheritdoc/>
        public GuildScheduledEventStatus Status { get; private set; }

        /// <inheritdoc/>
        public GuildScheduledEventType Type { get; private set; }

        /// <inheritdoc/>
        public ulong? EntityId { get; private set; }

        /// <inheritdoc/>
        public string Location { get; private set; }

        /// <inheritdoc/>
        public int? UserCount { get; private set; }

        internal RestGuildEvent(BaseDiscordClient client, IGuild guild, ulong id)
            : base(client, id)
        {
            Guild = guild;
        }

        internal static RestGuildEvent Create(BaseDiscordClient client, IGuild guild, Model model)
        {
            var entity = new RestGuildEvent(client, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal static RestGuildEvent Create(BaseDiscordClient client, IGuild guild, IUser creator, Model model)
        {
            var entity = new RestGuildEvent(client, guild, model.Id);
            entity.Update(model, creator);
            return entity;
        }

        internal void Update(Model model, IUser creator)
        {
            Update(model);
            Creator = creator;
            CreatorId = creator.Id;
        }

        internal void Update(Model model)
        {
            if (model.Creator.IsSpecified)
            {
                Creator = RestUser.Create(Discord, model.Creator.Value);
            }

            CreatorId = model.CreatorId.ToNullable() ?? 0; // should be changed?
            ChannelId = model.ChannelId.IsSpecified ? model.ChannelId.Value : null;
            Name = model.Name;
            Description = model.Description.GetValueOrDefault();
            StartTime = model.ScheduledStartTime;
            EndTime = model.ScheduledEndTime;
            PrivacyLevel = model.PrivacyLevel;
            Status = model.Status;
            Type = model.EntityType;
            EntityId = model.EntityId;
            Location = model.EntityMetadata?.Location.GetValueOrDefault();
            UserCount = model.UserCount.ToNullable();
            CoverImageId = model.Image;
            GuildId = model.GuildId;
        }

        /// <inheritdoc/>
        public string GetCoverImageUrl(ImageFormat format = ImageFormat.Auto, ushort size = 1024)
            => CDN.GetEventCoverImageUrl(Guild.Id, Id, CoverImageId, format, size);

        /// <inheritdoc/>
        public Task StartAsync(RequestOptions options = null)
            => ModifyAsync(x => x.Status = GuildScheduledEventStatus.Active);

        /// <inheritdoc/>
        public Task EndAsync(RequestOptions options = null)
            => ModifyAsync(x => x.Status = Status == GuildScheduledEventStatus.Scheduled
                ? GuildScheduledEventStatus.Cancelled
                : GuildScheduledEventStatus.Completed);

        /// <inheritdoc/>
        public Task DeleteAsync(RequestOptions options = null)
            => GuildHelper.DeleteEventAsync(Discord, this, options);

        /// <inheritdoc/>
        public async Task ModifyAsync(Action<GuildScheduledEventsProperties> func, RequestOptions options = null)
        {
            var model = await GuildHelper.ModifyGuildEventAsync(Discord, func, this, options).ConfigureAwait(false);
            Update(model);
        }

        /// <summary>
        ///     Gets a collection of N users interested in the event.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         The returned collection is an asynchronous enumerable object; one must call 
        ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual messages as a
        ///         collection.
        ///     </note>
        ///     This method will attempt to fetch all users that are interested in the event.
        ///     The library will attempt to split up the requests according to and <see cref="DiscordConfig.MaxGuildEventUsersPerBatch"/>.
        ///     In other words, if there are 300 users, and the <see cref="Discord.DiscordConfig.MaxGuildEventUsersPerBatch"/> constant
        ///     is <c>100</c>, the request will be split into 3 individual requests; thus returning 3 individual asynchronous
        ///     responses, hence the need of flattening.
        /// </remarks>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of users.
        /// </returns>
        public IAsyncEnumerable<IReadOnlyCollection<RestUser>> GetUsersAsync(RequestOptions options = null)
            => GuildHelper.GetEventUsersAsync(Discord, this, null, null, options);

        /// <summary>
        ///     Gets a collection of N users interested in the event.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         The returned collection is an asynchronous enumerable object; one must call 
        ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual users as a
        ///         collection.
        ///     </note>
        ///     <note type="warning">
        ///         Do not fetch too many users at once! This may cause unwanted preemptive rate limit or even actual
        ///         rate limit, causing your bot to freeze!
        ///     </note>
        ///     This method will attempt to fetch the number of users specified under <paramref name="limit"/> around
        ///     the user <paramref name="fromUserId"/> depending on the <paramref name="dir"/>. The library will
        ///     attempt to split up the requests according to your <paramref name="limit"/> and 
        ///     <see cref="DiscordConfig.MaxGuildEventUsersPerBatch"/>. In other words, should the user request 500 users,
        ///     and the <see cref="Discord.DiscordConfig.MaxGuildEventUsersPerBatch"/> constant is <c>100</c>, the request will
        ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
        ///     of flattening.
        /// </remarks>
        /// <param name="fromUserId">The ID of the starting user to get the users from.</param>
        /// <param name="dir">The direction of the users to be gotten from.</param>
        /// <param name="limit">The numbers of users to be gotten from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of users.
        /// </returns>
        public IAsyncEnumerable<IReadOnlyCollection<RestUser>> GetUsersAsync(ulong fromUserId, Direction dir, int limit = DiscordConfig.MaxGuildEventUsersPerBatch, RequestOptions options = null)
            => GuildHelper.GetEventUsersAsync(Discord, this, fromUserId, dir, limit, options);

        #region IGuildScheduledEvent

        /// <inheritdoc/>
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IGuildScheduledEvent.GetUsersAsync(RequestOptions options)
            => GetUsersAsync(options);
        /// <inheritdoc/>
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IGuildScheduledEvent.GetUsersAsync(ulong fromUserId, Direction dir, int limit, RequestOptions options)
            => GetUsersAsync(fromUserId, dir, limit, options);

        #endregion
    }
}
