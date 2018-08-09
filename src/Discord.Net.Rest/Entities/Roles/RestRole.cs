using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based role.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestRole : RestEntity<ulong>, IRole
    {
        internal IGuild Guild { get; }
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
        /// <inheritdoc />
        public GuildPermissions Permissions { get; private set; }
        /// <inheritdoc />
        public int Position { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <summary>
        ///     Gets if this role is the @everyone role of the guild or not.
        /// </summary>
        public bool IsEveryone => Id == Guild.Id;
        /// <inheritdoc />
        public string Mention => IsEveryone ? "@everyone" : MentionUtils.MentionRole(Id);

        internal RestRole(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, id)
        {
            Guild = guild;
        }
        internal static RestRole Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestRole(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Name = model.Name;
            IsHoisted = model.Hoist;
            IsManaged = model.Managed;
            IsMentionable = model.Mentionable;
            Position = model.Position;
            Color = new Color(model.Color);
            Permissions = new GuildPermissions(model.Permissions);
        }

        /// <inheritdoc />
        public async Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null)
        { 
            var model = await RoleHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }
        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => RoleHelper.DeleteAsync(this, Discord, options);

        /// <inheritdoc />
        public int CompareTo(IRole role) => RoleUtils.Compare(this, role);

        /// <summary>
        ///     Gets the name of the role.
        /// </summary>
        /// <returns>
        ///     A string that is the name of the role.
        /// </returns>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";

        //IRole
        /// <inheritdoc />
        IGuild IRole.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }
    }
}
