using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestGuildEmoteActor :
    RestActor<ulong, RestGuildEmote, GuildEmoteIdentity>,
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
        Guild = guild.Actor ?? new(client, guild);
    }

    [SourceOfTruth]
    internal RestGuildEmote CreateEntity(IGuildEmoteModel model)
        => RestGuildEmote.Construct(Client, Guild.Identity, model);
}

public sealed partial class RestGuildEmote :
    RestEntity<ulong>,
    IGuildEmote,
    IContextConstructable<RestGuildEmote, IGuildEmoteModel, GuildIdentity, DiscordRestClient>
{
    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    [SourceOfTruth] public RestUserActor? Creator { get; private set; }

    public string? Name => Model.Name;

    public bool IsManaged => Model.IsManaged;

    public bool RequireColons => Model.RequireColons;

    public bool IsAnimated => Model.IsAnimated;

    public bool IsAvailable => Model.IsAvailable;

    [ProxyInterface(
        typeof(IGuildEmoteActor),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IGuildEmote, IGuildEmoteModel>)
    )]
    internal RestGuildEmoteActor Actor { get; }

    internal IGuildEmoteModel Model { get; private set; }

    internal RestGuildEmote(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildEmoteModel model,
        RestGuildEmoteActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(client, guild, GuildEmoteIdentity.Of(this));
        Model = model;
    }

    public static RestGuildEmote Construct(DiscordRestClient client, GuildIdentity guild, IGuildEmoteModel model)
        => new(client, guild, model);

    public ValueTask UpdateAsync(IGuildEmoteModel model, CancellationToken token = default)
    {
        Creator = Creator.UpdateFrom(
            model.UserId,
            RestUserActor.Factory,
            Client
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public IGuildEmoteModel GetModel() => Model;
}
