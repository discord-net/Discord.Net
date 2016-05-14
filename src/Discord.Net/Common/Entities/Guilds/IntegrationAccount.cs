namespace Discord
{
    public struct IntegrationAccount
    {
        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string Name { get; private set; }

        public override string ToString() => Name ?? Id.ToString();
    }
}
