namespace Discord.Rest
{
    public class IntegrationAccount : IIntegrationAccount
    {
        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string Name { get; private set; }

        public override string ToString() => Name ?? Id.ToString();
    }
}
