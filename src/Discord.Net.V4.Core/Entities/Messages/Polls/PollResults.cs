using Discord.Models;

namespace Discord;

public readonly struct PollResults(
    bool isFinalized,
    IReadOnlyCollection<PollAnswerCount> answerCounts
) : IModelConstructable<PollResults, IPollResultModel>
{
    public bool IsFinalized { get; } = isFinalized;
    public IReadOnlyCollection<PollAnswerCount> AnswerCounts { get; } = answerCounts;

    public static PollResults Construct(IDiscordClient client, IPollResultModel model)
        => new(
            model.IsFinalized,
            model.AnswerCounts
                .Select(x => PollAnswerCount
                    .Construct(client, x)
                )
                .ToList()
                .AsReadOnly()
        );
}