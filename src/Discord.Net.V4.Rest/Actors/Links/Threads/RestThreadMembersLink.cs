using Discord.Models;

namespace Discord.Rest;

public sealed partial class RestThreadMembersLink :
    RestThreadMemberLink.Enumerable.Indexable.BackLink<IThreadChannelActor>,
    IThreadMembersLink
{
    public RestThreadMember Current { get; }

    public RestThreadMembersLink(
        RestThreadChannelActor channel
    ) : base(
        channel, 
        channel.Client, 
        new RestActorProvider<ulong, RestThreadMemberActor>(
            (client, id) => new RestThreadMemberActor(client, channel.Identity, ThreadMemberIdentity.Of(id))
        ),
        Routes.ListThreadMembers()
        )
    {
    }
}