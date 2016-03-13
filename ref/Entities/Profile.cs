using System.Threading.Tasks;

namespace Discord
{
    public class Profile : IEntity<ulong>
    {        
        public ulong Id { get; }
        public DiscordClient Discord { get; }
        public EntityState State { get; }

        public string AvatarId { get; }
        public string AvatarUrl { get; }
        public ushort Discriminator { get; }
        public string CurrentGame { get; }
        public UserStatus Status { get; }
        public string Mention { get; }        
        public string Email { get; }
		public bool? IsVerified { get; }

        public string Name { get; set; }        

        public Task Update() => null;
        public Task Delete() => null;
    }
}
