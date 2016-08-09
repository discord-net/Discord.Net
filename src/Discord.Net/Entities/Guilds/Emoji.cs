using System.Collections.Immutable;
using System.Diagnostics;
using Model = Discord.API.Emoji;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Emoji
    {
        public ulong Id { get; }
        public string Name { get; }
        public bool IsManaged { get; }
        public bool RequireColons { get; }
        public IImmutableList<ulong> RoleIds { get; }

        public Emoji(Model model)
        {
            Id = model.Id;
            Name = model.Name;
            IsManaged = model.Managed;
            RequireColons = model.RequireColons;
            RoleIds = ImmutableArray.Create(model.Roles);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
