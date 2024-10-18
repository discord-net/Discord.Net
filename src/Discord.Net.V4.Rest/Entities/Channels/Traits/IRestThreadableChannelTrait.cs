namespace Discord.Rest;

[Containerized, Trait]
public partial interface IRestThreadableChannelTrait<TLink> :
    IRestTraitProvider<RestGuildChannelActor>,
    IThreadableChannelTrait<TLink>
    where TLink : class, RestThreadChannelActor.Indexable, IThreadChannelActor.Indexable
{
    // [SourceOfTruth]
    // new TLink Threads => GetOrCreateTraitData(nameof(Threads), channel =>
    // {
    //     return RestThreadChannelActor.Indexable.BackLink<IRestThreadableChannelTrait<TLink>>.Create(
    //         this,
    //         channel.Client,
    //         RestThreadChannelActor.GetProvider(channel.Client),
    //         
    //     );
    // });
}