namespace Discord.Models;

public interface IActivityModel
{
    string Id { get; }
    string Url { get; }
    string Name { get; }
    ActivityType Type { get; }
    string Details { get; }
    string State { get; }
    ActivityProperties Flags { get; }
    DateTimeOffset CreatedAt { get; }
    IEmojiModel Emoji { get; }
    ulong? ApplicationId { get; }
    string SyncId { get; }
    string SessionId { get; }


    #region Assets
    string LargeImage { get; }
    string LargeText { get; }
    string SmallImage { get; }
    string SmallText { get; }
    #endregion

    #region Party
    string PartyId { get; }
    ICollection<long> PartySize { get; }
    #endregion

    #region Secrets
    string JoinSecret { get; }
    string SpectateSecret { get; }
    string MatchSecret { get; }
    #endregion

    #region Timestamps
    DateTimeOffset? TimestampStart { get; }
    DateTimeOffset? TimestampEnd { get; }
    #endregion

}
