namespace Discord.Models
{
    public interface IOverwriteModel
    {
        ulong Target { get; }
        PermissionTarget Type { get; }
        ulong Allow { get; }
        ulong Deny { get; }
    }
}
