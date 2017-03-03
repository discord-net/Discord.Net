using Discord.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketRole : SocketEntity<ulong>, IRole
    {
        public SocketGuild Guild { get; }

        public Color Color { get; private set; }
        public bool IsHoisted { get; private set; }
        public bool IsManaged { get; private set; }
        public bool IsMentionable { get; private set; }
        public string Name { get; private set; }
        public GuildPermissions Permissions { get; private set; }
        public int Position { get; private set; }

        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);
        public bool IsEveryone => Id == Guild.Id;
        public string Mention => MentionUtils.MentionRole(Id);

        internal SocketRole(SocketGuild guild, ulong id)
            : base(guild.Discord, id)
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
        }

        public Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null)
            => RoleHelper.ModifyAsync(this, Discord, func, options);
        public Task DeleteAsync(RequestOptions options = null)
            => RoleHelper.DeleteAsync(this, Discord, options);

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
        internal SocketRole Clone() => MemberwiseClone() as SocketRole;

        public int CompareTo(IRole role) => RoleUtils.Compare(this, role);

        //IRole
        IGuild IRole.Guild => Guild;
    }
}
