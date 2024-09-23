using Discord.Models;

namespace Discord;

public readonly struct PollAnswerCount(
    int answerId,
    int count,
    bool meVoted
) : IModelConstructable<PollAnswerCount, IPollAnswerCountModel>
{
    public int AnswerId { get; } = answerId;
    public int Votes { get; } = count;
    public bool HasVoted { get; } = meVoted;

    public static PollAnswerCount Construct(IDiscordClient client, IPollAnswerCountModel model)
        => new(model.Id, model.Count, model.MeVoted);
}