using Discord.Models;

namespace Discord.Rest;

public partial class RestSoundboardSoundActor(
    DiscordRestClient client,
    SoundboardSoundIdentity identity
) :
    RestActor<RestSoundboardSoundActor, ulong, RestSoundboardSound, ISoundboardSoundModel>(client, identity),
    ISoundboardSoundActor
{
    internal override SoundboardSoundIdentity Identity { get; } = identity;
}

public partial class RestSoundboardSound :
    RestEntity<ulong, ISoundboardSoundModel>,
    ISoundboardSound,
    IRestConstructable<RestSoundboardSound, RestSoundboardSoundActor, ISoundboardSoundModel>
{
    public string Name => Model.Name;

    public double Volume => Model.Volume;

    public DiscordEmojiId? Emoji { get; private set; }

    public bool IsAvailable => Model.IsAvailable;
    
    [ProxyInterface(typeof(ISoundboardSoundActor))]
    internal virtual RestSoundboardSoundActor Actor { get; }
    internal override ISoundboardSoundModel Model { get; }

    internal RestSoundboardSound(
        DiscordRestClient client,
        ISoundboardSoundModel model,
        RestSoundboardSoundActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        Model = model;

        Emoji = model.EmojiId is not null || model.EmojiName is not null
            ? new DiscordEmojiId(model.EmojiName, model.EmojiId, null)
            : (DiscordEmojiId?)null;
    }

    public static RestSoundboardSound Construct(
        DiscordRestClient client,
        RestSoundboardSoundActor actor,
        ISoundboardSoundModel model)
    {
        if (model is IGuildSoundboardSoundModel guildSound)
        {
            return RestGuildSoundboardSound.Construct(
                client,
                actor as RestGuildSoundboardSoundActor ?? client.Guilds[guildSound.GuildId].Sounds[model.Id],
                guildSound
            );
        }

        return new(client, model, actor);
    }
    
    public virtual ISoundboardSoundModel GetModel() => Model;
}