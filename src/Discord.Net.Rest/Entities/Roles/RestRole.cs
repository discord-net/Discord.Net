using Discord.API.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestRole : RestEntity<ulong>, IRole
    {
        public RestGuild Guild { get; }
        public Color Color { get; private set; }
        public bool IsHoisted { get; private set; }
        public bool IsManaged { get; private set; }
        public string Name { get; private set; }
        public GuildPermissions Permissions { get; private set; }
        public int Position { get; private set; }

        public bool IsEveryone => Id == Guild.Id;
        public string Mention => MentionUtils.MentionRole(Id);

        internal RestRole(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestRole Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestRole(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Name = model.Name;
            IsHoisted = model.Hoist;
            IsManaged = model.Managed;
            Position = model.Position;
            Color = new Color(model.Color);
            Permissions = new GuildPermissions(model.Permissions);
        }

        public Task ModifyAsync(Action<ModifyGuildRoleParams> func)
            => RoleHelper.ModifyAsync(this, Discord, func);
        public Task DeleteAsync()
            => RoleHelper.DeleteAsync(this, Discord);

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";

        //IRole
        IGuild IRole.Guild => Guild;
    }
}
