using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Role : IRole, IMentionable
    {
        /// <inheritdoc />
        public ulong Id { get; }
        /// <summary> Returns the guild this role belongs to. </summary>
        public Guild Guild { get; }

        /// <inheritdoc />
        public Color Color { get; private set; }
        /// <inheritdoc />
        public bool IsHoisted { get; private set; }
        /// <inheritdoc />
        public bool IsManaged { get; private set; }
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public GuildPermissions Permissions { get; private set; }
        /// <inheritdoc />
        public int Position { get; private set; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public bool IsEveryone => Id == Guild.Id;
        /// <inheritdoc />
        public string Mention => MentionUtils.Mention(this);
        public IEnumerable<GuildUser> Users => Guild.Users.Where(x => x.Roles.Any(y => y.Id == Id));
        internal DiscordClient Discord => Guild.Discord;

        internal Role(Guild guild, Model model)
        {
            Id = model.Id;
            Guild = guild;

            Update(model);
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
        /// <summary> Modifies the properties of this role. </summary>
        public async Task Modify(Action<ModifyGuildRoleParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildRoleParams();
            func(args);
            await Discord.ApiClient.ModifyGuildRole(Guild.Id, Id, args).ConfigureAwait(false);
        }
        /// <summary> Deletes this message. </summary>
        public async Task Delete()
            => await Discord.ApiClient.DeleteGuildRole(Guild.Id, Id).ConfigureAwait(false);

        /// <inheritdoc />
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";

        ulong IRole.GuildId => Guild.Id;

        Task<IEnumerable<IGuildUser>> IRole.GetUsers()
            => Task.FromResult<IEnumerable<IGuildUser>>(Users);
    }
}
