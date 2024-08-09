using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetCurrentUserVoiceState))]
[Modifiable<ModifyCurrentUserVoiceStateProperties>(nameof(Routes.ModifyCurrentUserVoiceState))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface ICurrentUserVoiceStateActor : IVoiceStateActor, IActor<ulong, ICurrentUserVoiceState>;
