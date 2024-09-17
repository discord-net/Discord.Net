using Discord.Rest;

namespace Discord;

public partial interface IInteractionWithTokenActor :
    IAbstractionActor<string>,
    ITokenPathProvider,
    IPathIdProvider<ulong>
{
    IInteractionMessageActor.Indexable Responses { get; }

    ulong IIdentifiable<ulong>.Id => Client.Applications.Current.Id;
    string IIdentifiable<string>.Id => Token;
}