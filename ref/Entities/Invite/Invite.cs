using System;
using System.Threading.Tasks;

namespace Discord
{
    public class Invite : BasicInvite
    {
        public int? MaxAge { get; }
        public int Uses { get; }
        public int? MaxUses { get; }
        public bool IsRevoked { get; }
        public bool IsTemporary { get; }
        public DateTime CreatedAt { get; }

        public override Task Update() => null;
        public Task Delete() => null;
    }
}
