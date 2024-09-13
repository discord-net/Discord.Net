using Discord.Rest;

namespace Discord;

public partial interface IApplicationCommandActor :
    IActor<ulong, IApplicationCommand>,
    IApplicationRelationship
{
    
}