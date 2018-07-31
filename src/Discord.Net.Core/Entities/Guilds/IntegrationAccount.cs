using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct IntegrationAccount
    {
        /// <summary>
        ///     ID of the account.
        /// </summary>
        public string Id { get; }
        /// <summary>
        ///     Name of the account.
        /// </summary>
        public string Name { get; private set; }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
