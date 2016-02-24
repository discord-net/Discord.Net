using APIChannel = Discord.API.Client.Channel;
using Discord.API.Client.Rest;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> A public Discord channel </summary>
    public abstract class PublicChannel : Channel, IModel, IMentionable
    {
        internal readonly PermissionManager _permissions;
   
        /// <summary> Gets the server owning this channel. </summary>
        public Server Server { get; }

        /// <summary> Gets or sets the name of this channel. </summary>
        public string Name { get; set; }
        /// <summary> Getsor sets the position of this channel relative to other channels of the same type in this server. </summary>
        public int Position { get; set; }

        /// <summary> Gets the DiscordClient that created this model. </summary>
        public override DiscordClient Client => Server.Client;
        public override User CurrentUser => Server.CurrentUser;
        /// <summary> Gets the string used to mention this channel. </summary>
        public string Mention => $"<#{Id}>";
        /// <summary> Gets a collection of all custom permissions used for this channel. </summary>
		public IEnumerable<PermissionRule> PermissionRules => _permissions.Rules;

        internal PublicChannel(APIChannel model, Server server)
            : this(model.Id, server)
        {
            _permissions = new PermissionManager(this, model, server.Client.Config.UsePermissionsCache ? (int)(server.UserCount * 1.05) : -1);
            Update(model);
        }
        protected PublicChannel(ulong id, Server server)
            : base(id)
        {
            Server = server;
        }

        internal override void Update(APIChannel model)
        {
            if (model.Name != null) Name = model.Name;
            if (model.Position != null) Position = model.Position.Value;

            if (model.PermissionOverwrites != null)
                _permissions.Update(model);
        }

        public async Task Delete()
        {
            try { await Client.ClientAPI.Send(new DeleteChannelRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
        public abstract Task Save();

        internal override User GetUser(ulong id) => Server.GetUser(id);

        public ChannelTriStatePermissions? GetPermissionsRule(User user) => _permissions.GetOverwrite(user);
        public ChannelTriStatePermissions? GetPermissionsRule(Role role) =>  _permissions.GetOverwrite(role);
        public Task AddOrUpdatePermissionsRule(User user, ChannelTriStatePermissions permissions) => _permissions.AddOrUpdateOverwrite(user, permissions);
        public Task AddOrUpdatePermissionsRule(Role role, ChannelTriStatePermissions permissions) => _permissions.AddOrUpdateOverwrite(role, permissions);
        public Task RemovePermissionsRule(User user) => _permissions.RemoveOverwrite(user);
        public async Task RemovePermissionsRule(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            try { await Client.ClientAPI.Send(new RemoveChannelPermissionsRequest(Id, role.Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        internal ChannelPermissions GetPermissions(User user) => _permissions.GetPermissions(user);
        internal void UpdatePermissions() => _permissions.UpdatePermissions();
        internal void UpdatePermissions(User user) => _permissions.UpdatePermissions(user);
        internal bool ResolvePermissions(User user, ref ChannelPermissions permissions) => _permissions.ResolvePermissions(user, ref permissions);

        internal override PermissionManager PermissionManager => null;

        /// <summary> Creates a new invite to this channel. </summary>
        /// <param name="maxAge"> Time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
        /// <param name="withXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to null. </param>
        public async Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false)
        {
            if (maxAge < 0) throw new ArgumentOutOfRangeException(nameof(maxAge));
            if (maxUses < 0) throw new ArgumentOutOfRangeException(nameof(maxUses));

            var request = new CreateInviteRequest(Id)
            {
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0,
                IsTemporary = tempMembership,
                WithXkcdPass = withXkcd
            };

            var response = await Client.ClientAPI.Send(request).ConfigureAwait(false);
            var invite = new Invite(Client, response.Code, response.XkcdPass);
            return invite;
        }

        internal void AddUser(User user) => _permissions.AddUser(user);
        internal void RemoveUser(ulong id) => _permissions.RemoveUser(id);
        
        public override string ToString() => $"{Server}/{Name ?? Id.ToIdString()}";
    }
}
