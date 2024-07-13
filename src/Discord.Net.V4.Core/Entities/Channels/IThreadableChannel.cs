using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using Modifiable = IModifiable<ulong, IThreadableChannel, ModifyThreadableChannelProperties, ModifyGuildChannelParams, IThreadableChannelModel>;

public partial interface IThreadableChannel :
    IGuildChannel,
    IThreadableChannelActor,
    Modifiable
{
    static IApiInOutRoute<ModifyGuildChannelParams, IEntityModel> Modifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGuildChannelParams args
    ) => Routes.ModifyChannel(id, args);

    int? DefaultThreadSlowmode { get; }

    ThreadArchiveDuration DefaultArchiveDuration { get; }

    [SourceOfTruth]
    new IThreadableChannelModel GetModel();
}
