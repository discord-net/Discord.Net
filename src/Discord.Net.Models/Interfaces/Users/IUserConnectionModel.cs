namespace Discord.Models;

public interface IUserConnectionModel : IEntityModel<ulong>
{
    string Name { get; }
    string Type { get; }
    bool? Revoked { get; }
    IEnumerable<IIntegrationModel>? Integrations { get; }
    bool IsVerified { get; }
    bool FriendSyncEnabled { get; }
    bool ShowActivity { get; }
    bool IsTwoWayLink { get; }
    int Visibility { get; }
}
