using System.Diagnostics;
using Model = Discord.API.Presence;

namespace Discord.WebSocket
{
    //TODO: C#7 Candidate for record type
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct SocketPresence : IPresence
    {
        public UserStatus Status { get; }
        public Activity? Activity { get; }

        internal SocketPresence(UserStatus status, Activity? game)
        {
            Status = status;
            Activity = game;
        }
        internal static SocketPresence Create(Model model)
        {
            return new SocketPresence(model.Status, model.Activity != null ? model.Activity.ToEntity() : (Activity?)null);
        }

        public override string ToString() => Status.ToString();
        private string DebuggerDisplay => $"{Status}{(Activity != null ? $", {Activity.Value.Name} ({Activity.Value.Type})" : "")}";

        internal SocketPresence Clone() => this;
    }
}
