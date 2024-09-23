using Discord.Models;

namespace Discord;

public partial interface IPollAnswer :
    IEntity<int, IPollAnswerModel>,
    IPollAnswerActor
{
    PollMedia Media { get; }
}