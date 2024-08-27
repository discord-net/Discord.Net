namespace Discord.Models;

[ModelEquality]
public partial interface IIntegrationModel : IEntityModel<ulong>
{
    string Name { get; }
    string Type { get; }
    bool IsEnabled { get; }
    bool? IsSyncing { get; }
    ulong? RoleId { get; }
    bool? EnableEmoticons { get; }
    int? ExpireBehavior { get; }
    int? ExpireGracePeriod { get; }
    ulong? UserId { get; }
    IIntegrationAccountModel? Account { get; }
    DateTimeOffset? SyncedAt { get; }
    int? SubscriberCount { get; }
    bool? IsRevoked { get; }
    IPartialApplicationModel? Application { get; }
    string[]? Scopes { get; }
}

[ModelEquality]
public partial interface IIntegrationAccountModel : IEntityModel<string>
{
    string Name { get; }
}
