namespace Discord.Models
{
    public interface IOverwriteModel
    {
        ulong Target { get; }
        int Type { get; }
        ulong Allow { get; }
        ulong Deny { get; }
    }
}
