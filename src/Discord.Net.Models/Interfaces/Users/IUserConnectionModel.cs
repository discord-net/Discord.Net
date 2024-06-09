namespace Discord.Models;

public interface IUserConnectionModel : IEntityModel<string>
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
