using Model = Discord.API.User;

namespace Discord.Rest
{
    public class DMUser : User, IDMUser
    {
        /// <inheritdoc />
        public DMChannel Channel { get; }

        internal override DiscordClient Discord => Channel.Discord;

        internal DMUser(DMChannel channel, Model model)
            : base(model)
        {
            Channel = channel;
        }

        IDMChannel IDMUser.Channel => Channel;
    }
}
