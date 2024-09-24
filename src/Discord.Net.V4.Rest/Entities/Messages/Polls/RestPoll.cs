using System.Collections.Immutable;
using Discord.Models;

namespace Discord.Rest;

public partial class RestPollActor :
    RestActor<RestPollActor, ulong, RestPoll, IPollModel>,
    IPollActor
{
    [SourceOfTruth] public RestMessageActor Message { get; }

    [SourceOfTruth] public IRestMessageChannelTrait Channel { get; }

    [SourceOfTruth] public RestPollAnswerActor.Indexable Answers { get; }

    internal override PollIdentity Identity { get; }

    public RestPollActor(
        DiscordRestClient client,
        MessageChannelIdentity channel,
        MessageIdentity message,
        PollIdentity poll
    ) : base(client, poll)
    {
        Identity = poll;

        Channel = channel.Actor ?? IRestMessageChannelTrait.GetContainerized(
            client.Channels[channel.Id]
        );

        Message = Channel.Messages[message];

        Answers = new(
            client,
            RestActorProvider.GetOrCreate(
                client,
                Template.Of<PollAnswerIdentity>(),
                channel,
                message
            )
        );
    }
}

public partial class RestPoll :
    RestEntity<ulong>,
    IPoll,
    IRestConstructable<RestPoll, RestPollActor, IPollModel>
{
    public PollMedia Question { get; }

    public IReadOnlyDictionary<int, RestPollAnswer> Answers { get; }

    public DateTimeOffset? ExpiresAt => Model.Expiry;

    public bool AllowsMultipleAnswers => Model.AllowMultiselect;

    public PollLayoutType Layout => (PollLayoutType) Model.LayoutType;

    public PollResults? Results { get; }

    [ProxyInterface(typeof(IPollActor))] internal RestPollActor Actor { get; }

    internal IPollModel Model { get; }

    internal RestPoll(
        DiscordRestClient client,
        IPollModel model,
        RestPollActor actor
    ) : base(client, actor.Id)
    {
        Actor = actor;
        Model = model;

        Question = PollMedia.Construct(client, model.Question);
        Answers = model.Answers
            .ToImmutableDictionary(
                x => x.Id,
                x => RestPollAnswer.Construct(client, actor.Answers[x.Id], x)
            );
        Results = model.Results is not null
            ? PollResults.Construct(client, model.Results)
            : null;
    }

    public static RestPoll Construct(DiscordRestClient client, RestPollActor actor, IPollModel model)
        => new(client, model, actor);

    public IPollModel GetModel() => Model;
    
    IReadOnlyDictionary<int, IPollAnswer> IPoll.Answers
        => Answers.ToImmutableDictionary(
            x => x.Key,
            IPollAnswer (x) => x.Value
        );
}