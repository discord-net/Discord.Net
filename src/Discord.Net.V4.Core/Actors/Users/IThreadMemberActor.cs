using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetThreadMember))]
[Deletable(nameof(Routes.RemoveThreadMember))]
public partial interface IThreadMemberActor :
    IActor<ulong, IThreadMember>,
    IThreadRelationship,
    IMemberRelationship,
    IUserRelationship;
