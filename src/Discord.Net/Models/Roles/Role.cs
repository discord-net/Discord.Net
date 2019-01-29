using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wumpus.Entities;
using Model = Wumpus.Entities.Role;

namespace Discord
{
    internal class Role : SnowflakeEntity, IRole
    {
        public Role(Model model, IGuild guild, IDiscordClient discord) : base(discord)
        {
            Guild = guild;

            Color = model.Color;
            IsHoisted = model.IsHoisted;
            IsManaged = model.Managed;
            IsMentionable = model.IsMentionable;
            Name = model.Name.ToString();
            Permissions = model.Permissions;
            Position = model.Position;
        }

        public IGuild Guild { get; set; }

        public Color Color { get; set; }
        public bool IsHoisted { get; set; }
        public bool IsManaged { get; set; }
        public bool IsMentionable { get; set; }
        public string Name { get; set; }
        public GuildPermissions Permissions { get; set; }
        public int Position { get; set; }
        public string Mention => throw new NotImplementedException(); // TODO: MentionUtils

        public Task DeleteAsync()
            => Discord.Rest.DeleteGuildRoleAsync(Guild.Id, Id);

        public Task ModifyAsync()
        {
            throw new NotImplementedException();
        }

        // IComparable
        public int CompareTo(IRole other)
        {
            return Id.CompareTo(other.Id);
        }
    }
}
