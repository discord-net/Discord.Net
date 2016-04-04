using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.Emoji;

namespace Discord
{
    public struct Emoji
    {
        private readonly Guild _guild;
        private readonly ImmutableArray<Role> _roles;

        public ulong Id { get; }
        public string Name { get; }
        public bool IsManaged { get; }
        public bool RequireColons { get; }

        internal Emoji(Model model, Guild guild)
        {
            Id = model.Id;
            _guild = guild;

            Name = model.Name;
            IsManaged = model.Managed;
            RequireColons = model.RequireColons;
            _roles = model.Roles.Select(x => guild.GetRole(x)).ToImmutableArray();
        }
    }
}
