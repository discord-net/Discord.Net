using Model = Discord.API.User;

namespace Discord
{
    public class SelfUser : User
    {
        public override DiscordClient Discord { get; }

        public string Email { get; private set; }
        public bool IsVerified { get; private set; }

        internal SelfUser(ulong id, DiscordClient discord)
            : base(id)
        {
            Discord = discord;
        }

        internal override void Update(Model model)
        {
            base.Update(model);

            Email = model.Email;
            IsVerified = model.IsVerified;
        }
    }
}
