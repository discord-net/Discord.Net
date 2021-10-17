using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a thread channel received over REST.
    /// </summary>
    public class RestThreadChannel : RestTextChannel, IThreadChannel
    {
        public ThreadType Type { get; private set; }
        /// <inheritdoc/>
        public bool HasJoined { get; private set; }

        /// <inheritdoc/>
        public bool IsArchived { get; private set; }

        /// <inheritdoc/>
        public ThreadArchiveDuration AutoArchiveDuration { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset ArchiveTimestamp { get; private set; }

        /// <inheritdoc/>
        public bool IsLocked { get; private set; }

        /// <inheritdoc/>
        public int MemberCount { get; private set; }

        /// <inheritdoc/>
        public int MessageCount { get; private set; }

        /// <summary>
        ///     Gets the parent text channel id.
        /// </summary>
        public ulong ParentChannelId { get; private set; }

        internal RestThreadChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id) { }

        internal new static RestThreadChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestThreadChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            base.Update(model);

            HasJoined = model.ThreadMember.IsSpecified;

            if (model.ThreadMetadata.IsSpecified)
            {
                IsArchived = model.ThreadMetadata.Value.Archived;
                AutoArchiveDuration = model.ThreadMetadata.Value.AutoArchiveDuration;
                ArchiveTimestamp = model.ThreadMetadata.Value.ArchiveTimestamp;
                IsLocked = model.ThreadMetadata.Value.Locked.GetValueOrDefault(false);
            }

            MemberCount = model.MemberCount.GetValueOrDefault(0);
            MessageCount = model.MessageCount.GetValueOrDefault(0);
            Type = (ThreadType)model.Type;
            ParentChannelId = model.CategoryId.Value;
        }

        /// <summary>
        ///     Gets a user within this thread.
        /// </summary>
        /// <param name="userId">The id of the user to fetch.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous get operation. The task returns a
        ///     <see cref="RestThreadUser"/> if found, otherwise <see langword="null"/>.
        /// </returns>
        public new Task<RestThreadUser> GetUserAsync(ulong userId, RequestOptions options = null)
            => ThreadHelper.GetUserAsync(userId, this, Discord, options);

        /// <summary>
        ///     Gets a collection of users within this thread.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous get operation. The task returns a
        ///     <see cref="IReadOnlyCollection{T}"/> of <see cref="RestThreadUser"/>'s.
        /// </returns>
        public new async Task<IReadOnlyCollection<RestThreadUser>> GetUsersAsync(RequestOptions options = null)
            => (await ThreadHelper.GetUsersAsync(this, Discord, options).ConfigureAwait(false)).ToImmutableArray();

        /// <inheritdoc/>
        public override async Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null)
        {
            var model = await ThreadHelper.ModifyAsync(this, Discord, func, options);
            Update(model);
        }

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task<IInviteMetadata> CreateInviteToStreamAsync(IUser user, int? maxAge, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task<RestWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task<ICategoryChannel> GetCategoryAsync(RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override OverwritePermissions? GetPermissionOverwrite(IRole role)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override OverwritePermissions? GetPermissionOverwrite(IUser user)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null)
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>This method is not supported in threads.</b>
        /// </remarks>
        public override IReadOnlyCollection<Overwrite> PermissionOverwrites
            => throw new NotSupportedException("This method is not supported in threads.");

        /// <inheritdoc/>
        public Task JoinAsync(RequestOptions options = null)
            => Discord.ApiClient.JoinThreadAsync(Id, options);

        /// <inheritdoc/>
        public Task LeaveAsync(RequestOptions options = null)
            => Discord.ApiClient.LeaveThreadAsync(Id, options);

        /// <inheritdoc/>
        public Task AddUserAsync(IGuildUser user, RequestOptions options = null)
            => Discord.ApiClient.AddThreadMemberAsync(Id, user.Id, options);

        /// <inheritdoc/>
        public Task RemoveUserAsync(IGuildUser user, RequestOptions options = null)
            => Discord.ApiClient.RemoveThreadMemberAsync(Id, user.Id, options);
    }
}
