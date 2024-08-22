using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(DMChannel)), 
    Deletable(nameof(Routes.DeleteChannel)),
    Creatable<CreateDMProperties>(nameof(Routes.CreateDm)),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IDMChannelActor :
    IMessageChannelTrait,
    IUserRelationship,
    IActor<ulong, IDMChannel>
{
    IUserActor Recipient { get; }

    IUserActor IUserRelationship.User => Recipient;
}