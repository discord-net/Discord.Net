using System.Collections.Immutable;
using System.Linq;

namespace Discord.Rest;

internal static class PollExtensions
{
    public static API.Rest.CreatePollParams ToModel(this PollProperties poll)
        => new()
        {
            AllowMultiselect = poll.AllowMultiselect,
            Duration = poll.Duration,
            LayoutType = poll.LayoutType,
            Answers = poll.Answers.Select(x => new API.PollAnswer
            {
                PollMedia = new API.PollMedia
                {
                    Emoji = x.Emoji is not null
                        ? new API.Emoji
                        {
                            Id = x.Emoji is Emote emote ? emote.Id : null,
                            Name = x.Emoji is Emoji emoji ? emoji.Name : null,
                        } : Optional<API.Emoji>.Unspecified,
                    Text = x.Text,
                }
            }).ToArray(),
            Question = new API.PollMedia
            {
                Emoji = poll.Question.Emoji is not null
                    ? new API.Emoji
                    {
                        Id = poll.Question.Emoji is Emote emote ? emote.Id : null,
                        Name = poll.Question.Emoji is Emoji emoji ? emoji.Name : null,
                    }
                    : Optional<API.Emoji>.Unspecified,
                Text = poll.Question.Text,
            },
        };

    public static Poll ToEntity(this API.Poll poll)
        => new(
            new PollMedia(poll.Question.Text,
                poll.Question.Emoji.IsSpecified
                    ? poll.Question.Emoji.Value.ToIEmote()
                    : null),
            poll.Answers.Select(x =>
                new PollAnswer(
                    x.AnswerId,
                    new PollMedia(x.PollMedia.Text, x.PollMedia.Emoji.IsSpecified
                        ? x.PollMedia.Emoji.Value.ToIEmote()
                        : null))).ToImmutableArray(),
            poll.Expiry,
            poll.AllowMultiselect,
            poll.LayoutType,
            poll.PollResults.IsSpecified
                ? new PollResults(
                    poll.PollResults.Value.IsFinalized,
                    poll.PollResults.Value.AnswerCounts.Select(x => new PollAnswerCounts(x.Id, x.Count, x.MeVoted)).ToImmutableArray())
                : null
            );
}
