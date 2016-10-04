namespace Discord
{
    public interface IBan
    {
        IUser User { get; }
        string Reason { get; }
    }
}
