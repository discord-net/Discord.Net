using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestRole : RestEntity<ulong>, IRole
    {
        internal IGuild Guild { get; }
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

        public async Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null)
        { 
            var model = await RoleHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }
        public Task DeleteAsync(RequestOptions options = null)
            => RoleHelper.DeleteAsync(this, Discord, options);

        public int CompareTo(IRole role) => RoleUtils.Compare(this, role);

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";

        //IRole
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
