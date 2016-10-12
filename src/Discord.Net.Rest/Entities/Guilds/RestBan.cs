using System.Diagnostics;
using Model = Discord.API.Ban;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestBan : IBan
    {
        public RestUser User { get; }
        public string Reason { get; }

        internal RestBan(RestUser user, string reason)
        {
            User = user;
            Reason = reason;
        }
        internal static RestBan Create(BaseDiscordClient client, Model model)
        {
            return new RestBan(RestUser.Create(client, model.User), model.Reason);
        }

        public override string ToString() => User.ToString();
        private string DebuggerDisplay => $"{User}: {Reason}";

        //IBan
        IUser IBan.User => User;
    }
}
