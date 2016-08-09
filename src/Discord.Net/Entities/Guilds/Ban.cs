using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Ban
    {
        public IUser User { get; }
        public string Reason { get; }

        public Ban(IUser user, string reason)
        {
            User = user;
            Reason = reason;
        }

        public override string ToString() => User.ToString();
        private string DebuggerDisplay => $"{User}: {Reason}";
    }
}
