using Model = Discord.API.MemberPresence;

namespace Discord
{
    public class Presence
    {
        public string CurrentGame { get; private set; }
        public UserStatus Status { get; private set; }

        internal Presence() { }

        internal virtual void Update(Model model)
        {
            CurrentGame = model.Game?.Name;
            Status = model.Status;
        }
    }
}
