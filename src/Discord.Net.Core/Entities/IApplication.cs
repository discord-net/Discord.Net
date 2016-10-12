namespace Discord
{
    public interface IApplication : ISnowflakeEntity
    {
        string Name { get; }
        string Description { get; }
        string[] RPCOrigins { get; }
        ulong Flags { get; }
        string IconUrl  { get; }

        IUser Owner { get; }
    }
}
