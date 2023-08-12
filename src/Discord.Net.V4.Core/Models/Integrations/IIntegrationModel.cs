namespace Discord.Models;

public interface IIntegrationModel : IEntityModel<ulong>
{
    string Name { get; }
    string Type { get; }
    bool IsEnabled { get; }
    bool? IsSyncing { get; }
    ulong? RoleId { get; }
    bool? EnableEmoticons { get; }
    IntegrationExpireBehavior? ExpireBehavior { get; }
    int? ExpireGracePeriod { get; }
    IUserModel? User { get; }
    IIntegrationAccountModel Account { get; }
    DateTimeOffset? SyncedAt { get; }
    int? SubscriberCount { get; }
    bool? IsRevoked { get; }
    IApplicationModel? Application { get; }
    ICollection<string>? Scopes { get; }
}

public interface IIntegrationAccountModel
{
    ulong Id { get; }
    string Name { get; }
}
