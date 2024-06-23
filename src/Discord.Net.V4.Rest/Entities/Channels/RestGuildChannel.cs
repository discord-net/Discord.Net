using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;
using PropertyChanged;
using System.Collections.Immutable;
using System.ComponentModel;

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

    public IEnumerableIndexableActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites => throw new NotImplementedException();

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
}

public partial class RestGuildChannel(DiscordRestClient client, ulong guildId, IGuildChannelModel model, RestGuildChannelActor? actor = null) :
    RestChannel(client, model),
    IGuildChannel,
    IContextConstructable<RestGuildChannel, IGuildChannelModel, ulong, DiscordRestClient>,
    INotifyPropertyChanged
{
    [OnChangedMethod(nameof(OnModelChanged))]
    internal new IGuildChannelModel Model { get; set; } = model;

    [ProxyInterface(typeof(IGuildChannelActor), typeof(IGuildRelationship))]
    internal override RestGuildChannelActor Actor { get; } = actor ?? new RestGuildChannelActor(client, guildId, model.Id);

    public static RestGuildChannel Construct(DiscordRestClient client, IGuildChannelModel model, ulong context)
    {
        return model switch
        {
            IGuildForumChannelModel guildForumChannelModel => RestForumChannel.Construct(client, guildForumChannelModel,
                context),
            IGuildMediaChannelModel guildMediaChannelModel => RestMediaChannel.Construct(client, guildMediaChannelModel,
                context),
            IGuildNewsChannelModel guildNewsChannelModel => RestNewsChannel.Construct(client, guildNewsChannelModel,
                context),
            IGuildVoiceChannelModel guildVoiceChannelModel => RestVoiceChannel.Construct(client, guildVoiceChannelModel,
                context),
            IThreadChannelModel threadChannelModel => RestThreadChannel.Construct(client, threadChannelModel, context),
            IGuildTextChannelModel guildTextChannelModel => RestTextChannel.Construct(client, guildTextChannelModel,
                context),
            _ => new(client, context, model)
        };
    }

    private void OnModelChanged()
    {
        PermissionOverwrites = Model.Permissions
            .Select(x => Overwrite.Construct(Client, x))
            .ToImmutableArray();
    }

    public int Position => Model.Position;

    public ChannelFlags Flags => (ChannelFlags?)Model.Flags ?? ChannelFlags.None;

    public IReadOnlyCollection<Overwrite> PermissionOverwrites { get; private set; } =
        model.Permissions.Select(x => Overwrite.Construct(client, x)).ToImmutableArray();

}
