using Discord.Rest;

namespace Discord;

public partial interface IInteractionWithTokenActor :
    IAbstractionActor<string>,
    ITokenPathProvider
{
    IInteractionMessageActor.Indexable Responses { get; }

    string IIdentifiable<string>.Id => Token;
}