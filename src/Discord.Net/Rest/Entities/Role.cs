using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord.Rest
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
            var response = await Discord.ApiClient.ModifyGuildRole(Guild.Id, Id, args).ConfigureAwait(false);
            Update(response);
        }
        /// <summary> Deletes this message. </summary>
        public async Task Delete()
            => await Discord.ApiClient.DeleteGuildRole(Guild.Id, Id).ConfigureAwait(false);

        /// <inheritdoc />
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";

        ulong IRole.GuildId => Guild.Id;
                
        async Task<IEnumerable<IGuildUser>> IRole.GetUsers()
        {
            //TODO: Rethink this, it isn't paginated or anything...
            var models = await Discord.ApiClient.GetGuildMembers(Guild.Id, new GetGuildMembersParams()).ConfigureAwait(false);
            return models.Where(x => x.Roles.Contains(Id)).Select(x => new GuildUser(Guild, x));
        }
    }
}
