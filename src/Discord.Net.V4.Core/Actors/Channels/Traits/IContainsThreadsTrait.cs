using Discord.Models;

namespace Discord;

[Trait]
public interface IContainsThreadsTrait<out TThreadActor>
    where TThreadActor : class, IThreadChannelActor
{
    [return: TypeHeuristic(nameof(Threads))]
    TThreadActor Thread(ulong id) => Threads[id];
    ILinkType<TThreadActor, ulong, IThreadChannel, IThreadChannelModel>.Indexable Threads { get; }
}