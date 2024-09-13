using Discord.Models;

namespace Discord;

[Trait]
public partial interface IHasThreadsTrait<out TThreadActor, out TThreadLink> :
    IHasThreadsTrait
    where TThreadActor : class, IThreadChannelActor
    where TThreadLink : class, 
    ILinkType<TThreadActor, ulong, IThreadChannel, IThreadChannelModel>.Indexable
{
    [SourceOfTruth]
    [return: TypeHeuristic(nameof(Threads))]
    new TThreadActor Thread(ulong id) 
        => Threads[id];
    
    [SourceOfTruth]
    new TThreadLink Threads { get; }
}

[Trait]
public partial interface IHasThreadsTrait
{
    [return: TypeHeuristic(nameof(Threads))]
    IThreadChannelActor Thread(ulong id) => Threads[id];
    ILinkType<IThreadChannelActor, ulong, IThreadChannel, IThreadChannelModel>.Indexable Threads { get; }
}
