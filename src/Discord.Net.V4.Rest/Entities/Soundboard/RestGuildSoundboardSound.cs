using Discord.Models;

namespace Discord.Rest;

public partial class RestGuildSoundboardSoundActor :
    RestSoundboardSoundActor,
    IGuildSoundboardSoundActor,
    IRestActor<RestGuildSoundboardSoundActor, ulong, RestGuildSoundboardSound, IGuildSoundboardSoundModel>
{
    [SourceOfTruth]
    public RestGuildActor Guild { get; }

    [SourceOfTruth] internal override GuildSoundboardSoundIdentity Identity { get; }

    [TypeFactory]
    public RestGuildSoundboardSoundActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildSoundboardSoundIdentity sound
    ) : base(client, sound)
    {
        Identity = sound;
        Guild = client.Guilds[guild];
    }

    [CovariantOverride, SourceOfTruth]
    internal RestGuildSoundboardSound CreateEntity(IGuildSoundboardSoundModel model)
        => RestGuildSoundboardSound.Construct(Client, this, model);
}

public partial class RestGuildSoundboardSound :
    RestSoundboardSound,
    IGuildSoundboardSound,
    IRestConstructable<RestGuildSoundboardSound, RestGuildSoundboardSoundActor, IGuildSoundboardSoundModel>
{
    [SourceOfTruth]
    public RestUserActor? Creator => Model.UserId.HasValue
        ? Client.Users[Model.UserId.Value]
        : null;
    
    [ProxyInterface(typeof(IGuildSoundboardSoundActor))]
    internal override RestGuildSoundboardSoundActor Actor { get; }

    internal override IGuildSoundboardSoundModel Model => _model;

    private IGuildSoundboardSoundModel _model;

    public RestGuildSoundboardSound(
        DiscordRestClient client,
        IGuildSoundboardSoundModel model,
        RestGuildSoundboardSoundActor actor
    ) : base(client, model, actor)
    {
        Actor = actor;
        _model = model;
    }

    public static RestGuildSoundboardSound Construct(
        DiscordRestClient client,
        RestGuildSoundboardSoundActor actor,
        IGuildSoundboardSoundModel model
    ) => new(client, model, actor);
    
    public ValueTask UpdateAsync(IGuildSoundboardSoundModel model, CancellationToken token = default)
    {
        _model = model;
        return ValueTask.CompletedTask;
    }

    public override IGuildSoundboardSoundModel GetModel() => Model;
}