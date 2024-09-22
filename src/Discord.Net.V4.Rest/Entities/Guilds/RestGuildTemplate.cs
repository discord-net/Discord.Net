using Discord.Models;

namespace Discord.Rest;

public partial class RestGuildTemplateActor :
    RestActor<RestGuildTemplateActor, string, RestGuildTemplate, IGuildTemplateModel>,
    IGuildTemplateActor
{
    internal override GuildTemplateIdentity Identity { get; }

    public RestGuildTemplateActor(
        DiscordRestClient client,
        GuildTemplateIdentity tempalte
    ) : base(client, tempalte)
    {
        Identity = tempalte;
    }


    [SourceOfTruth]
    internal override RestGuildTemplate CreateEntity(IGuildTemplateModel model)
        => RestGuildTemplate.Construct(Client, this, model);
}

public partial class RestGuildTemplateFromGuildActor :
    RestGuildTemplateActor,
    IGuildTemplateFromGuildActor
{
    [SourceOfTruth]
    public RestGuildActor Guild { get; }
    
    public RestGuildTemplateFromGuildActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildTemplateIdentity tempalte
    ) : base(client, tempalte)
    {
        Guild = client.Guilds[guild];
    }
}

public sealed partial class RestGuildTemplate :
    RestEntity<string>,
    IGuildTemplate,
    IRestConstructable<RestGuildTemplate, RestGuildTemplateActor, IGuildTemplateModel>,
    IRestConstructable<RestGuildTemplate, RestGuildTemplateFromGuildActor, IGuildTemplateModel>
{
    [SourceOfTruth] public RestGuildActor SourceGuild { get; }

    [SourceOfTruth] public RestUserActor Creator { get; }

    public string Name => Model.Name;

    public string? Description => Model.Description;

    public int UsageCount => Model.UsageCount;

    public DateTimeOffset CreatedAt => Model.CreatedAt;

    public DateTimeOffset UpdatedAt => Model.UpdatedAt;

    public bool IsDirty => Model.IsDirty ?? false;

    [ProxyInterface(typeof(IGuildTemplateFromGuildActor))]
    internal RestGuildTemplateFromGuildActor Actor { get; }

    internal IGuildTemplateModel Model { get; private set; }

    internal RestGuildTemplate(
        DiscordRestClient client,
        IGuildTemplateModel model,
        RestGuildTemplateFromGuildActor actor
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor;

        SourceGuild = client.Guilds[model.SourceGuildId];
        Creator = client.Users[model.CreatorId];
    }

    public static RestGuildTemplate Construct(
        DiscordRestClient client,
        RestGuildTemplateActor actor,
        IGuildTemplateModel model
    ) => Construct(
        client,
        actor as RestGuildTemplateFromGuildActor ?? client.Guilds[model.SourceGuildId].Templates[model.Id],
        model
    );
    
    public static RestGuildTemplate Construct(
        DiscordRestClient client,
        RestGuildTemplateFromGuildActor actor,
        IGuildTemplateModel model
    ) => new(client, model, actor);

    public ValueTask UpdateAsync(IGuildTemplateModel model, CancellationToken token = default)
    {
        Model = model;
        return ValueTask.CompletedTask;
    }

    public IGuildTemplateModel GetModel()
        => Model;
}