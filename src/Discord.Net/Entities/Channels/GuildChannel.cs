using Discord.API.Rest;
using Discord.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    public abstract class GuildChannel : IChannel, IEntity<ulong>
    {
        private readonly PermissionManager _permissions;

        /// <inheritdoc />
        public ulong Id { get; }
        /// <summary> Gets the guild this channel is a member of. </summary>
        public Guild Guild { get; }
        /// <inheritdoc />
        public abstract ChannelType Type { get; }

        /// <inheritdoc />
        public string Name { get; private set; }
        /// <summary> Gets the position of this public channel relative to others of the same type. </summary>
        public int Position { get; private set; }

        /// <inheritdoc />
        public DiscordClient Discord => Guild.Discord;
        /// <summary> Gets a collection of all users in this channel. </summary>
        public IEnumerable<GuildUser> Users => _permissions.GetUsers();
        /// <inheritdoc />
        IEnumerable<User> IChannel.Users => _permissions.GetUsers();
        /// <summary> Gets a collection of permission overwrites for this channel. </summary>
        public IEnumerable<Overwrite> PermissionOverwrites => _permissions.Overwrites;

        internal GuildChannel(ulong id, Guild guild, bool usePermissionsCache)
        {
            Id = id;
            Guild = guild;

            _permissions = new PermissionManager(this, usePermissionsCache);
        }

        internal virtual void Update(Model model)
        {
            Name = model.Name;
            Position = model.Position;

            _permissions.Update(model);
        }

        /// <summary> Gets a user in this channel with the given id. </summary>
        public GuildUser GetUser(ulong id)
            => _permissions.GetUser(id);
        /// <inheritdoc />
        User IChannel.GetUser(ulong id) => GetUser(id);

        /// <summary> Gets the permission overwrite for a specific user, or null if one does not exist. </summary>
        public OverwritePermissions? GetPermissionOverwrite(GuildUser user)
            => _permissions.GetOverwrite(user);
        /// <summary> Gets the permission overwrite for a specific role, or null if one does not exist. </summary>
        public OverwritePermissions? GetPermissionOverwrite(Role role)
            => _permissions.GetOverwrite(role);
        /// <summary> Downloads a collection of all invites to this channel. </summary>
        public async Task<IEnumerable<GuildInvite>> GetInvites()
        {
            var response = await Discord.RestClient.Send(new GetChannelInvitesRequest(Id)).ConfigureAwait(false);
            return response.Select(x =>
            {
                var invite = Discord.CreateGuildInvite(this, x);
                invite.Update(x);
                return invite;
            });
        }

        /// <summary> Adds or updates the permission overwrite for the given user. </summary>
        public Task UpdatePermissionOverwrite(GuildUser user, OverwritePermissions permissions)
            =>  _permissions.AddOrUpdateOverwrite(user, permissions);
        /// <summary> Adds or updates the permission overwrite for the given role. </summary>
        public Task UpdatePermissionOverwrite(Role role, OverwritePermissions permissions)
            => _permissions.AddOrUpdateOverwrite(role, permissions);
        /// <summary> Removes the permission overwrite for the given user, if one exists. </summary>
        public Task RemovePermissionOverwrite(GuildUser user)
            => _permissions.RemoveOverwrite(user);
        /// <summary> Removes the permission overwrite for the given role, if one exists. </summary>
        public Task RemovePermissionOverwrite(Role role)
            => _permissions.RemoveOverwrite(role);

        internal ChannelPermissions GetPermissions(GuildUser user)
            => _permissions.GetPermissions(user);
        internal void UpdatePermissions()
            => _permissions.UpdatePermissions();
        internal void UpdatePermissions(GuildUser user)
            => _permissions.UpdatePermissions(user);

        /// <summary> Creates a new invite to this channel. </summary>
        /// <param name="maxAge"> Time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the guild after closing their client. </param>
        /// <param name="withXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to null. </param>
        public async Task<PublicInvite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool humanReadable = false)
        {
            var response = await Discord.RestClient.Send(new CreateChannelInviteRequest(Id)
            {
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0,
                IsTemporary = tempMembership,
                WithXkcdPass = humanReadable
            }).ConfigureAwait(false);
            return Discord.CreatePublicInvite(response);
        }

        /// <inheritdoc />
        public async Task Delete()
        {
            try { await Discord.RestClient.Send(new DeleteChannelRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
        /// <inheritdoc />
        public async Task Update()
        {
            var response = await Discord.RestClient.Send(new GetChannelRequest(Id)).ConfigureAwait(false);
            if (response != null)
                Update(response);
        }

        /// <inheritdoc />
        public override string ToString() => $"{Guild}/{Name ?? Id.ToString()}";
    }
}
