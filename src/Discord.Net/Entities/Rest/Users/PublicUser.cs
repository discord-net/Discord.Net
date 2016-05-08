using Model = Discord.API.User;

namespace Discord.Rest
{
    public class PublicUser : User
    {
        internal override DiscordRestClient Discord { get; }

        internal PublicUser(DiscordRestClient discord, Model model)
            : base(model)
        {
            Discord = discord;
        }
    }
}
