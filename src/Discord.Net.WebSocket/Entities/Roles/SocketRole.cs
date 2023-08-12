using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based role to be given to a guild user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketRole : SocketEntity<ulong>, IRole
    {
        #region SocketRole
        /// <summary>
        ///     Gets the guild that owns this role.
        /// </summary>
        /// <returns>
        ///     A <see cref="SocketGuild"/> representing the parent guild of this role.
        /// </returns>
        public SocketGuild Guild { get; }

        /// <inheritdoc />
        public Color Color { get; private set; }
        /// <inheritdoc />
        public bool IsHoisted { get; private set; }
        /// <inheritdoc />
        public bool IsManaged { get; private set; }
        /// <inheritdoc />
        public bool IsMentionable { get; private set; }
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc/>
        public Emoji Emoji { get; private set; }
        /// <inheritdoc />
        public string Icon { get; private set; }
        /// <inheritdoc />
        public GuildPermissions Permissions { get; private set; }
        /// <inheritdoc />
        public int Position { get; private set; }
        /// <inheritdoc />
        public RoleTags Tags { get; private set; }

        /// <inheritdoc />
        public RoleFlags Flags { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <summary>
        ///     Returns a value that determines if the role is an @everyone role.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the role is @everyone; otherwise <see langword="false" />.
        /// </returns>
        public bool IsEveryone => Id == Guild.Id;
        /// <inheritdoc />
        public string Mention => IsEveryone ? "@everyone" : MentionUtils.MentionRole(Id);

        /// <summary>
        ///     Returns an IEnumerable containing all <see cref="SocketGuildUser"/> that have this role.
        /// </summary>
        public IEnumerable<SocketGuildUser> Members
            => Guild.Users.Where(x => x.Roles.Any(r => r.Id == Id));

        internal SocketRole(SocketGuild guild, ulong id)
            : base(guild?.Discord, id)
        {
            Guild = guild;
        }
        internal static SocketRole Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketRole(guild, model.Id);
            entity.Update(state, model);
            return entity;
        }
        internal void Update(ClientState state, Model model)
        {
            Name = model.Name;
            IsHoisted = model.Hoist;
            IsManaged = model.Managed;
            IsMentionable = model.Mentionable;
            Position = model.Position;
            Color = new Color(model.Color);
            Permissions = new GuildPermissions(model.Permissions);
            Flags = model.Flags;

            if (model.Tags.IsSpecified)
                Tags = model.Tags.Value.ToEntity();

            if (model.Icon.IsSpecified)
            {
                Icon = model.Icon.Value;
            }

            if (model.Emoji.IsSpecified)
            {
                Emoji = new Emoji(model.Emoji.Value);
            }
        }

        /// <inheritdoc />
        public Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null)
            => RoleHelper.ModifyAsync(this, Discord, func, options);
        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => RoleHelper.DeleteAsync(this, Discord, options);

        /// <inheritdoc />
        public string GetIconUrl()
            => CDN.GetGuildRoleIconUrl(Id, Icon);

        /// <summary>
        ///     Gets the name of the role.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="Discord.WebSocket.SocketRole.Name" />.
        /// </returns>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
        internal SocketRole Clone() => MemberwiseClone() as SocketRole;

        /// <inheritdoc />
        public int CompareTo(IRole role) => RoleUtils.Compare(this, role);
        #endregion

        #region IRole
        /// <inheritdoc />
        IGuild IRole.Guild => Guild;
        #endregion
    }
}
