namespace Discord
{
    public interface IIntegrationAccount : IEntity<string>
    {
        string Name { get; }
    }
}
