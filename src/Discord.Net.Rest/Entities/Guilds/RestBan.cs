using System.Diagnostics;
using Model = Discord.API.Ban;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class RestBan : IBan
    {
        internal RestBan(RestUser user, string reason)
        {
            User = user;
            Reason = reason;
        }

        public RestUser User { get; }
        private string DebuggerDisplay => $"{User}: {Reason}";
        public string Reason { get; }

        //IBan
        IUser IBan.User => User;

        internal static RestBan Create(BaseDiscordClient client, Model model) =>
            new RestBan(RestUser.Create(client, model.User), model.Reason);

        public override string ToString() => User.ToString();
    }
}
