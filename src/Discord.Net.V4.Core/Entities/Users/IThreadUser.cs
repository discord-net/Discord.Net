namespace Discord;

public interface IThreadUser : ISnowflakeEntity
{
    ILoadableEntity<ulong, IUser> User { get; }
    ILoadableEntity<ulong, IGuildUser> Member { get; }
    ILoadableEntity<ulong, IThreadChannel> Thread { get; }

    DateTimeOffset JoinedAt { get; }

    ulong IEntity<ulong>.Id => User.Id;
}
