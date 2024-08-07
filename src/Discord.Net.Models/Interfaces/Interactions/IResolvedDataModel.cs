namespace Discord.Models;

public interface IResolvedDataModel
{
    IEnumerable<string>? Users { get; }
    IEnumerable<string>? Members { get; }
    IEnumerable<string>? Roles { get; }
    IEnumerable<string>? Channels { get; }
    IEnumerable<string>? Messages { get; }
    IEnumerable<string>? Attachments { get; }
}
