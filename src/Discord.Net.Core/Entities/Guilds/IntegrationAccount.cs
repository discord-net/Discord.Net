using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct IntegrationAccount
    {
        /// <summary> Gets the ID of the account. </summary>
        /// <returns> Gets the ID of the account. </returns>
        public string Id { get; }
        /// <summary> Gets the name of the account. </summary>
        /// <returns> Gets the name of the account. </returns>
        public string Name { get; private set; }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
