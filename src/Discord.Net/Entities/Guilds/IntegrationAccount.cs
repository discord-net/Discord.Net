using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct IntegrationAccount
    {
        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string Name { get; private set; }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
