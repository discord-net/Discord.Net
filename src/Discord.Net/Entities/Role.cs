using Discord.API.Rest;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord
{
    public class Role : IEntity<ulong>, IMentionable
    {
        /// <inheritdoc />
        public ulong Id { get; }
        /// <summary> Returns the guild this role belongs to. </summary>
        public Guild Guild { get; }

        /// <summary> Gets the name of this role. </summary>
        public string Name { get; private set; }
        /// <summary> Returns true if members of this role are isolated in the user list. </summary>
        public bool IsHoisted { get; private set; }
        /// <summary> Gets the position of this role relative to other roles in this guild. </summary>
        public int Position { get; private set; }
        /// <summary> Returns true if this role is managed by the Discord server (e.g. for Twitch integration) </summary>
        public bool IsManaged { get; private set; }
        /// <summary> Gets the permissions given to all members of this role. </summary>
        public GuildPermissions Permissions { get; private set; }
        /// <summary> Gets the color assigned to members of this role. </summary>
        public Color Color { get; private set; }

        /// <inheritdoc />
        public DiscordClient Discord => Guild.Discord;
        /// <summary> Returns true if this is the role representing all users in a server. </summary>
        public bool IsEveryone => Id == Guild.Id;
        /// <summary> Gets the string used to mention this role. </summary>
        public string Mention => IsEveryone ? "@everyone" : "";
        /// <summary> Gets a collection of all members in this role. </summary>
        public IEnumerable<GuildUser> Members { get { throw new NotImplementedException(); } } //TODO: Implement

        internal Role(ulong id, Guild guild)
        {
            Id = id;
            Guild = guild;
        }

        internal void Update(Model model)
        {
            Name = model.Name;
            IsHoisted = model.Hoist.Value;
            IsManaged = model.Managed.Value;
            Position = model.Position.Value;
            Color = new Color(model.Color.Value);
            Permissions = new GuildPermissions(model.Permissions.Value);
        }

        /// <inheritdoc />
        public Task Update() { throw new NotSupportedException(); } //TODO: Not supported yet 

        /// <summary> Modifies the properties of this role. </summary>
        public async Task Modify(Action<ModifyGuildRoleRequest> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var req = new ModifyGuildRoleRequest(Guild.Id, Id);
            func(req);
            await Discord.RestClient.Send(req).ConfigureAwait(false);
        }

        /// <summary> Deletes this message. </summary>
        public async Task Delete()
        {
            try { await Discord.RestClient.Send(new DeleteGuildRoleRequest(Guild.Id, Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        internal void UpdatePermissions()
        {
            foreach (var member in Members)
                Guild.UpdatePermissions(member);
        }

        /// <inheritdoc />
        public override string ToString() => $"{Guild}/{Name ?? Id.ToString()}";
    }
}
