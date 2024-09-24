using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public partial class RestPollAnswerActor :
    RestActor<RestPollAnswerActor, int, RestPollAnswer, IPollAnswerModel>,
    IPollAnswerActor
{
    [SourceOfTruth] public RestPollActor Poll => Message.Poll;

    [SourceOfTruth] public RestMessageActor Message { get; }

    [SourceOfTruth] public IRestMessageChannelTrait Channel { get; }

    [SourceOfTruth] public RestUserActor.Paged<PagePollVotersParams> Voters { get; }

    internal override PollAnswerIdentity Identity { get; }

    [TypeFactory]
    public RestPollAnswerActor(
        DiscordRestClient client,
        MessageChannelIdentity channel,
        MessageIdentity message,
        PollAnswerIdentity answer
    ) : base(client, answer)
    {
        Identity = answer;

        Channel = channel.Actor ?? IRestMessageChannelTrait.GetContainerized(
            client.Channels[channel.Id]
        );

        Message = Channel.Messages[message];

        Voters = new(
            client,
            client.Users,
            new RestPagingProvider<IUserModel, PollVoters, PagePollVotersParams, RestUser>(
                client,
                m => m.Users,
                (model, _) => client.Users[model.Id].CreateEntity(model),
                this
            )
        );
    }
}

public partial class RestPollAnswer :
    RestEntity<int>,
    IPollAnswer,
    IRestConstructable<RestPollAnswer, RestPollAnswerActor, IPollAnswerModel>
{
    public PollMedia Media { get; }
    
    [ProxyInterface(typeof(IPollAnswerActor))]
    internal RestPollAnswerActor Actor { get; }

    internal IPollAnswerModel Model { get; }

    internal RestPollAnswer(
        DiscordRestClient client,
        IPollAnswerModel model,
        RestPollAnswerActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        Model = model;

        Media = PollMedia.Construct(client, model.Media);
    }

    public static RestPollAnswer Construct(DiscordRestClient client, RestPollAnswerActor actor, IPollAnswerModel model)
        => new(client, model, actor);
    
    public IPollAnswerModel GetModel() => Model;
}