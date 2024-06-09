namespace Discord;

public interface IThreadUser : ISnowflakeEntity
{
    IEntitySource<ulong, IUser> User { get; }
    IEntitySource<ulong, IGuildUser> Member { get; }
    IEntitySource<ulong, IThreadChannel> Thread { get; }

    DateTimeOffset JoinedAt { get; }

    ulong IEntity<ulong>.Id => User.Id;
}
