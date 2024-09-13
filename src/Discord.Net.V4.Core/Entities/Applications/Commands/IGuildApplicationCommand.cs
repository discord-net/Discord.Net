using Discord.Rest;

namespace Discord;

[
    Refreshable(nameof(Routes.GetGuildApplicationCommand)),
    FetchableOfMany(nameof(Routes.GetGuildApplicationCommands))
]
public partial interface IGuildApplicationCommand :
    IApplicationCommand,
    IGuildApplicationCommandActor
{
    [SourceOfTruth]
    new IGuildActor Guild { get; }
}