using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.MemberPresence;

namespace Discord
{
    public class GuildPresence : Presence
    {
        public Guild Guild { get; }
        public ulong UserId { get; }

        /// <inheritdoc />
        public IReadOnlyList<Role> Roles { get; private set; }

        internal GuildPresence(ulong userId, Guild guild)
        {
            UserId = userId;
            Guild = guild;
        }
        internal override void Update(Model model)
        {
            base.Update(model);
            Roles = model.Roles.Select(x => Guild.GetRole(x)).ToImmutableArray();
        }

        public bool HasRole(Role role) => false;

        //TODO: Unsure about these
        /*public Task AddRoles(params Role[] roles) => xxx;
        public Task RemoveRoles(params Role[] roles) => xxx;*/
    }
}