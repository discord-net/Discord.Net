namespace Discord.API;

internal interface IResolvable
{
    Optional<ApplicationCommandInteractionDataResolved> Resolved { get; }
}
