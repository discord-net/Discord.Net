using System.Threading.Tasks;

namespace Discord
{
    public class Profile : IModel<ulong>
    {
        public DiscordClient Client { get; }
        
        public ulong Id { get; }
        public string AvatarId { get; }
        public string AvatarUrl { get; }
        public ushort Discriminator { get; }
        public string CurrentGame { get; }
        public UserStatus Status { get; }
        public string Mention { get; }        
        public string Email { get; }
		public bool? IsVerified { get; }

        public string Name { get; set; }

        public Task Save() => null;
    }
}
