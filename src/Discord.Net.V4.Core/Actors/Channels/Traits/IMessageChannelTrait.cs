using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Trait]
[Loadable(nameof(Routes.GetChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IMessageChannelTrait :
    IChannelActor,
    IActorTrait<ulong, IMessageChannel>
{
    [return: TypeHeuristic(nameof(Messages))]
    IMessageActor Message(ulong id) => Messages[id];
    IPagedIndexableActor<IMessageActor, ulong, IMessage, PageChannelMessagesParams> Messages { get; }

    internal new IMessageChannel CreateEntity(IChannelModel model);

    internal new IMessageChannel? CreateNullableEntity(IChannelModel? model)
        => model is null ? null : CreateEntity(model);

    IChannel IEntityProvider<IChannel, IChannelModel>.CreateEntity(IChannelModel model)
        => CreateEntity(model);
    IChannel? IEntityProvider<IChannel, IChannelModel>.CreateNullableEntity(IChannelModel? model)
        => CreateNullableEntity(model);

}
