using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(ThreadChannelBase)),
    Modifiable<ModifyThreadChannelProperties>(nameof(Routes.ModifyChannel)),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IThreadChannelActor :
    IMessageChannelTrait,
    IActor<ulong, IThreadChannel>
{
    IThreadMembersLink Members { get; }

    Task JoinAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(Routes.JoinThread(Id), options ?? Client.DefaultRequestOptions, token);

    Task LeaveAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(Routes.LeaveThread(Id), options ?? Client.DefaultRequestOptions, token);
}