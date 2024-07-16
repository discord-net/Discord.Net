using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IMessageChannelActor :
    IChannelActor,
    IActor<ulong, IMessageChannel>
{
    [SourceOfTruth]
    internal new IMessageChannel CreateEntity(IChannelModel model);

    [SourceOfTruth]
    internal new IMessageChannel? CreateNullableEntity(IChannelModel? model)
        => model is null ? null : CreateEntity(model);

    IIndexableActor<IMessageActor, ulong, IMessage> Messages { get; }
    IMessageActor Message(ulong id) => Messages[id];
}
