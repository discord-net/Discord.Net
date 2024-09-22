using Discord.Models;

namespace Discord.Rest;

public interface IRestHasThreadsTrait<out TProvider, out TThreadActor, out TThreadLink> :
    IRestTraitProvider<TProvider>,
    IHasThreadsTrait<TThreadActor, TThreadLink>
    where TProvider : IRestTraitProvider<TProvider>, IRestTraitProvider
    where TThreadActor : class, IThreadChannelActor
    where TThreadLink : class,  ILinkType<TThreadActor, ulong, IThreadChannel, IThreadChannelModel>.Indexable
{
    
}