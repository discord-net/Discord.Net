using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Extensions;
using Discord.Rest;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestGuildChannelActor :
    RestChannelActor,
    IGuildChannelActor,
    IRestActor<ulong, RestGuildChannel, GuildChannelIdentity, IGuildChannelModel>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public GuildChannelInviteLink.Enumerable.Indexable.BackLink<RestGuildChannelActor> Invites { get; }

    [SourceOfTruth] internal override GuildChannelIdentity Identity { get; }

    [TypeFactory]
    public RestGuildChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel
    ) : base(client, channel)
    {
        Identity = channel | this;

        Guild = guild.Actor ?? client.Guilds[guild.Id];

        Invites = new RestGuildChannelInviteLink.Enumerable.Indexable.BackLink<RestGuildChannelActor>(
            this,
            client,
            new RestActorProvider<string, RestGuildChannelInviteActor>(
                (client, id) => new RestGuildChannelInviteActor(
                    client,
                    guild,
                    Identity,
                    GuildChannelInviteIdentity.Of(id)
                )
            ),
            Routes.GetChannelInvites(Id).AsRequiredProvider()
        );
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestGuildChannel CreateEntity(IGuildChannelModel model)
        => RestGuildChannel.Construct(Client, this, model);
}

public partial class RestGuildChannel :
    RestChannel,
    IGuildChannel,
    IRestConstructable<RestGuildChannel, RestGuildChannelActor, IGuildChannelModel>
{
    public string Name => Model.Name;

    public int Position => Model.Position;

    public ChannelFlags Flags => (ChannelFlags?) Model.Flags ?? ChannelFlags.None;

    public IReadOnlyCollection<Overwrite> PermissionOverwrites { get; private set; }

    [ProxyInterface(
        typeof(IGuildChannelActor)
    )]
    internal override RestGuildChannelActor Actor { get; }

    internal override IGuildChannelModel Model => _model;

    private IGuildChannelModel _model;

    internal RestGuildChannel(
        DiscordRestClient client,
        IGuildChannelModel model,
        RestGuildChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;

        PermissionOverwrites = model.Permissions.Select(x => Overwrite.Construct(client, x)).ToImmutableArray();
    }

    public static RestGuildChannel Construct(
        DiscordRestClient client,
        RestGuildChannelActor actor,
        IGuildChannelModel model)
    {
        switch (model)
        {
            case IGuildCategoryChannelModel guildCategoryChannelModel:
                return RestCategoryChannel.Construct(
                    client,
                    actor as RestCategoryChannelActor ?? actor.Guild.CategoryChannels[model.Id],
                    guildCategoryChannelModel
                );
            case IGuildForumChannelModel guildForumChannelModel:
                return RestForumChannel.Construct(
                    client,
                    actor as RestForumChannelActor ?? actor.Guild.ForumChannels[model.Id],
                    guildForumChannelModel
                );
            case IGuildMediaChannelModel guildMediaChannelModel:
                return RestMediaChannel.Construct(
                    client,
                    actor as RestMediaChannelActor ?? actor.Guild.MediaChannels[model.Id],
                    guildMediaChannelModel
                );
            case IGuildNewsChannelModel guildNewsChannelModel:
                return RestNewsChannel.Construct(
                    client,
                    actor as RestNewsChannelActor ?? actor.Guild.AnnouncementChannels[model.Id],
                    guildNewsChannelModel
                );
            case IGuildStageChannelModel guildStageChannelModel:
                return RestStageChannel.Construct(
                    client,
                    actor as RestStageChannelActor ?? actor.Guild.StageChannels[model.Id],
                    guildStageChannelModel
                );
            case IGuildTextChannelModel guildTextChannelModel:
                return RestTextChannel.Construct(
                    client,
                    actor as RestTextChannelActor ?? actor.Guild.TextChannels[model.Id],
                    guildTextChannelModel
                );
            case IGuildVoiceChannelModel guildVoiceChannelModel:
                return RestVoiceChannel.Construct(
                    client,
                    actor as RestVoiceChannelActor ?? actor.Guild.VoiceChannels[model.Id],
                    guildVoiceChannelModel
                );
            case IThreadableChannelModel threadableChannelModel:
                return RestThreadableChannel.Construct(
                    client,
                    actor as RestThreadableChannelActor ?? new RestThreadableChannelActor(
                        client,
                        actor.Guild.Identity,
                        ThreadableChannelIdentity.Of(model.Id)
                    ),
                    threadableChannelModel
                );
            case IThreadChannelModel threadChannelModel:
                return RestThreadChannel.Construct(
                    client,
                    actor as RestThreadableChannelActor ?? new RestThreadableChannelActor(
                        client,
                        actor.Guild.Identity,
                        ThreadableChannelIdentity.Of(model.Id)
                    ),
                    threadableChannelModel
                );
            default:
                return new(client, model, actor);
        }
    }

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(IGuildChannelModel model, CancellationToken token = default)
    {
        if (!_model.Permissions.SequenceEqual(model.Permissions))
        {
            PermissionOverwrites = model.Permissions
                .Select(x => Overwrite.Construct(Client, x))
                .ToImmutableArray();
        }

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildChannelModel GetModel() => Model;
}