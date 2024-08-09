using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Refreshable(nameof(Routes.GetCurrentUserGuildMember))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface ICurrentMember :
    IMember,
    ICurrentMemberActor;
