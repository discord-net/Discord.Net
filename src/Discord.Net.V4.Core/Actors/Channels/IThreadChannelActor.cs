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
    IThreadMemberRelationship,
    IActor<ulong, IThreadChannel>
{
    IThreadMemberActor CurrentThreadMember { get; }

    // TODO: make this paged in api v11: https://discord.com/developers/docs/resources/channel#list-thread-members
    ThreadMemberLink.Enumerable.Indexable.BackLink<IThreadChannelActor> ThreadMembers { get; }

    Task JoinAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(Routes.JoinThread(Id), options ?? Client.DefaultRequestOptions, token);

    Task LeaveAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(Routes.LeaveThread(Id), options ?? Client.DefaultRequestOptions, token);

    IThreadMemberActor IThreadMemberRelationship.ThreadMember => CurrentThreadMember;
}