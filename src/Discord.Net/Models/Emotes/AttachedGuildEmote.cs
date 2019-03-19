using System.Collections.Generic;
using System.Threading.Tasks;
using Model = Wumpus.Entities.Emoji;

namespace Discord
{
    internal class AttachedGuildEmote : SnowflakeEntity, IGuildEmote
    {
        public AttachedGuildEmote(Model model, IGuild guild, IDiscordClient discord) : base(discord)
        {
            IsManaged = model.Managed.GetValueOrDefault(false);
            RequireColons = model.RequireColons.GetValueOrDefault(false);

            Wumpus.Snowflake[] roleIds = model.RoleIds.GetValueOrDefault() ?? new Wumpus.Snowflake[0];
            Role[] roles = new Role[roleIds.Length];
            for (int i = 0; i < roleIds.Length; i++)
                roles[i] = null; // TODO guild.GetRole()
            Roles = roles;

            CreatorId = model.User.IsSpecified ? model.User.Value.Id : (ulong?)null; // TODO: EntityOrId this guy
            Name = model.Name.ToString();
            Guild = guild;
        }

        // IGuildEmote
        public bool IsManaged { get; set; }
        public bool RequireColons { get; set; }
        public IReadOnlyList<IRole> Roles { get; set; }
        public ulong? CreatorId { get; set; }
        public string Name { get; set; }
        public IGuild Guild { get; set; }

        // ITaggable
        public string Tag => EmoteUtilities.FormatGuildEmote(Id, Name);

        // IDeleteable
        public Task DeleteAsync()
            => Discord.Rest.DeleteGuildEmojiAsync(Guild.Id, Id);

        // IGuildEmote
        public Task ModifyAsync() // TODO
        {
            throw new System.NotImplementedException();
        }
    }
}
