namespace Discord.Models;

[ModelEquality]
public partial interface IActivityModel : IEntityModel
{
    string? Url { get; }
    string Name { get; }
    int Type { get; }
    string? Details { get; }
    string? State { get; }
    int? Flags { get; }
    DateTimeOffset CreatedAt { get; }
    IEmojiModel? Emoji { get; } // TODO: should not be this kind of model
    ulong? ApplicationId { get; }


    #region Assets
    string? LargeImage { get; }
    string? LargeText { get; }
    string? SmallImage { get; }
    string? SmallText { get; }
    #endregion

    #region Party
    string? PartyId { get; }
    long[]? PartySize { get; }
    #endregion

    #region Secrets
    string? JoinSecret { get; }
    string? SpectateSecret { get; }
    string? MatchSecret { get; }
    #endregion

    #region Timestamps
    DateTimeOffset? TimestampStart { get; }
    DateTimeOffset? TimestampEnd { get; }
    #endregion

}
