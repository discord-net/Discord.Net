using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;
using System.Collections.Immutable;

namespace Discord.Rest.Channels;

public sealed partial class RestLoadableGuildChannel(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildChannelActor(client, guildId, id),
    ILoadableGuildChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildChannel>))]
    public RestLoadable<ulong, RestGuildChannel, IGuildChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            (_, model) =>
            {
                if (model is null)
                    return null;

                if (model is not GuildChannelBase guildChannelModel)
                    throw new DiscordException($"Channel type is not a guild channel ({(ChannelType)model.Type})");

                return RestGuildChannel.Construct(client, guildChannelModel, guildId);
            }
        );
}

[ExtendInterfaceDefaults(
    typeof(IGuildChannelActor),
    typeof(IDeletable<ulong, IGuildChannelActor>),
    typeof(IModifiable<ulong, IGuildChannelActor, ModifyGuildChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestGuildChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestChannelActor(client, id),
    IGuildChannelActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guildId);

    public ILoadableRootActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites => throw new NotImplementedException();

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
}

public partial class RestGuildChannel(DiscordRestClient client, ulong guildId, IGuildChannelModel model, RestGuildChannelActor? actor = null) :
    RestChannel(client, model),
    IGuildChannel,
    IContextConstructable<RestGuildChannel, IGuildChannelModel, ulong, DiscordRestClient>
{
    internal override IGuildChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IGuildChannelActor), typeof(IGuildRelationship))]
    internal override RestGuildChannelActor Actor { get; } = actor ?? new RestGuildChannelActor(client, guildId, model.Id);


    public int Position => Model.Position;

    public ChannelFlags Flags => (ChannelFlags?)Model.Flags ?? ChannelFlags.None;

    public IReadOnlyCollection<Overwrite> PermissionOverwrites
        => Model.Permissions.Select(x => Overwrite.Construct(Client, x)).ToImmutableArray();

    public static RestGuildChannel Construct(DiscordRestClient client, IGuildChannelModel model, ulong context)
        => new(client, context, model);
}
