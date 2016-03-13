using System.Threading.Tasks;

namespace Discord
{
	public class BasicInvite : IEntity<string>
    {
        public class TargetInfo
        {
            public ulong Id { get; }
            public string Name { get; }
        }
        public class InviterInfo
        {
            public ulong Id { get; }
            public string Name { get; }
            public ushort Discriminator { get; }
            public string AvatarId { get; }
            public string AvatarUrl { get; }
        }

        string IEntity<string>.Id => Code;
        public DiscordClient Discord { get; }
        public EntityState State { get; }

        public string Code { get; }
        public string XkcdCode { get; }

        public TargetInfo Server { get; }
        public TargetInfo Channel { get; }

        public string Url { get; }

        public Task Accept() => null;

        public virtual Task Update() => null;
    }
}
