using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestGuildEmoteActor :
    RestActor<RestGuildEmoteActor, ulong, RestGuildEmote, ICustomEmoteModel>,
    IGuildEmoteActor
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    internal override GuildEmoteIdentity Identity { get; }

    [method: TypeFactory]
    public RestGuildEmoteActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildEmoteIdentity emote
    ) : base(client, emote)
    {
        Identity = emote | this;
        Guild = client.Guilds[guild];
    }

    [SourceOfTruth]
    internal override RestGuildEmote CreateEntity(ICustomEmoteModel model)
        => RestGuildEmote.Construct(Client, this, model);
}

public sealed partial class RestGuildEmote :
    RestEntity<ulong>,
    IGuildEmote,
    IRestConstructable<RestGuildEmote, RestGuildEmoteActor, ICustomEmoteModel>
{
    [SourceOfTruth]
    public RestRoleActor.Defined Roles { get; }

    [SourceOfTruth] public RestUserActor? Creator { get; private set; }

    public string Name => Model.Name;

    public bool IsManaged => Model.IsManaged;

    public bool RequireColons => Model.RequireColons;

    public bool IsAnimated => Model.IsAnimated;

    public bool IsAvailable => Model.IsAvailable;

    [ProxyInterface(
        typeof(IGuildEmoteActor),
        typeof(IEntityProvider<IGuildEmote, ICustomEmoteModel>)
    )]
    internal RestGuildEmoteActor Actor { get; }

    internal ICustomEmoteModel Model { get; private set; }

    internal RestGuildEmote(
        DiscordRestClient client,
        ICustomEmoteModel model,
        RestGuildEmoteActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        Model = model;
        
        Roles = new(client, actor.Guild.Roles, model.Roles);
    }

    public static RestGuildEmote Construct(DiscordRestClient client, RestGuildEmoteActor actor, ICustomEmoteModel model)
        => new(client, model, actor);

    public ValueTask UpdateAsync(ICustomEmoteModel model, CancellationToken token = default)
    {
        Creator = Creator.UpdateFrom(
            model.UserId,
            RestUserActor.Factory,
            Client
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public ICustomEmoteModel GetModel() => Model;
}
