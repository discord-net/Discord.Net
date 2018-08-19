using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using Model = Discord.API.Role;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class SocketRole : SocketEntity<ulong>, IRole
    {
        internal SocketRole(SocketGuild guild, ulong id)
            : base(guild.Discord, id)
        {
            Guild = guild;
        }

        public SocketGuild Guild { get; }
        public bool IsEveryone => Id == Guild.Id;

        public IEnumerable<SocketGuildUser> Members
            => Guild.Users.Where(x => x.Roles.Any(r => r.Id == Id));

        private string DebuggerDisplay => $"{Name} ({Id})";

        public Color Color { get; private set; }
        public bool IsHoisted { get; private set; }
        public bool IsManaged { get; private set; }
        public bool IsMentionable { get; private set; }
        public string Name { get; private set; }
        public GuildPermissions Permissions { get; private set; }
        public int Position { get; private set; }

        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        public string Mention => IsEveryone ? "@everyone" : MentionUtils.MentionRole(Id);

        public Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null)
            => RoleHelper.ModifyAsync(this, Discord, func, options);

        public Task DeleteAsync(RequestOptions options = null)
            => RoleHelper.DeleteAsync(this, Discord, options);

        public int CompareTo(IRole role) => RoleUtils.Compare(this, role);

        //IRole
        IGuild IRole.Guild => Guild;

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
        }

        public override string ToString() => Name;
        internal SocketRole Clone() => MemberwiseClone() as SocketRole;
    }
}
