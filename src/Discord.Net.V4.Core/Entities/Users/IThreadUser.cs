namespace Discord;

public interface IThreadUser 
{
    IEntitySource<ulong, IUser> User { get; }
    IEntitySource<ulong, IGuildUser> Member { get; }
    IEntitySource<ulong, IThreadChannel> Thread { get; }

    DateTimeOffset JoinedAt { get; }
}
