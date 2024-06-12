namespace Discord;

public readonly struct UserConnection(
    string id,
    string name,
    string type,
    bool? isRevoked,
    bool isVerified,
    bool isFriendSynced,
    bool showActivity,
    bool isTwoWayLink,
    ConnectionVisibility visibility) : IEntityProperties<Models.Json.UserConnection>, IConstructable<UserConnection, Models.Json.UserConnection>
{
    public readonly string Id = id;
    public readonly string Name = name;
    public readonly string Type = type;
    public readonly bool? IsRevoked = isRevoked;
    //public readonly IReadOnlyCollection<Integration> Integrations;
    public readonly bool IsVerified = isVerified;
    public readonly bool IsFriendSynced = isFriendSynced;
    public readonly bool ShowActivity = showActivity;
    public readonly bool IsTwoWayLink = isTwoWayLink;
    public readonly ConnectionVisibility Visibility = visibility;

    public Models.Json.UserConnection ToApiModel(Models.Json.UserConnection? existing = default)
    {
        return new Models.Json.UserConnection()
        {
            Id = Id,
            FriendSync = IsFriendSynced,
            Name = Name,
            Type = Type,
            IsRevoked = Optional.FromNullable(IsRevoked),
            IsVerified = IsVerified,
            Visibility = (int)Visibility,
            ShowActivity = ShowActivity,
            IsTwoWayLink = IsTwoWayLink,
            //TODO: Integrations =
        };
    }

    public static UserConnection Construct(IDiscordClient client, Models.Json.UserConnection model)
    {
        return new UserConnection(
            model.Id,
            model.Name,
            model.Type,
            model.IsRevoked.ToNullable(),
            model.IsVerified,
            model.FriendSync,
            model.ShowActivity,
            model.IsTwoWayLink,
            (ConnectionVisibility)model.Visibility
        );
    }
}
