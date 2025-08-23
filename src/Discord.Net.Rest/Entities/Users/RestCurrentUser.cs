using System.Globalization;
using Discord.Models;
using Discord.Rest.Api;

namespace Discord.Rest;

public sealed class RestCurrentUser :
    RestUser,
    ICurrentUser,
    IRestEntity<RestCurrentUser, Snowflake, ICurrentUserModel>
{
    public override ICurrentUserModel Model => _model;
    
    protected override RestCurrentUserActor Actor { get; }

    private ICurrentUserModel _model;
    
    private RestCurrentUser(
        DiscordRestClient client,
        ICurrentUserModel model,
        RestCurrentUserActor? actor = null
    ) : base(client, model, actor)
    {
        Actor = actor ?? client.Users.Current;
        _model = model;
    }

    public static RestCurrentUser Create(DiscordRestClient client, ICurrentUserModel model)
        => new(client, model);

    public async Task<ICurrentUser> ModifyAsync(
        IModifyCurrentUserParams properties,
        RequestOptions options = default
    )
    {
        _model = await Routes.UpdateMyUser.Instance
            .AsPipeline(properties)
            .RunAsync(Client, options);

        return this;
    }

    ValueTask<ICurrentUser> ICurrentUserActor.GetAsync(RequestOptions options) => ValueTask.FromResult<ICurrentUser>(this);
}