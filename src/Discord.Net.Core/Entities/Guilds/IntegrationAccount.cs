using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct IntegrationAccount
    {
        /// <summary> Gets the ID of the account. </summary>
        /// <returns> A <see cref="string"/> unique identifier of this integration account. </returns>
        public string Id { get; }
        /// <summary> Gets the name of the account. </summary>
        /// <returns> A string containing the name of this integration account. </returns>
        public string Name { get; private set; }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
