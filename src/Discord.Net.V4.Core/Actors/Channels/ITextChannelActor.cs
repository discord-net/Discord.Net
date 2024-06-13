using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableTextChannelActor<TTextChannel> :
    ITextChannelActor<TTextChannel>,
    ILoadableThreadableGuildChannelActor<TTextChannel>,
    ILoadableMessageChannelActor<TTextChannel>
    where TTextChannel : class, ITextChannel<TTextChannel>;

public interface ITextChannelActor<out TTextChannel> :
    IMessageChannelActor<TTextChannel>,
    IThreadableGuildChannelActor<TTextChannel>,
    IModifiable<ulong, ITextChannelActor<TTextChannel>, ModifyTextChannelProperties, ModifyGuildChannelParams>
    where TTextChannel : ITextChannel<TTextChannel>
{
    static ApiBodyRoute<ModifyGuildChannelParams> IModifiable<ulong, ITextChannelActor<TTextChannel>, ModifyTextChannelProperties, ModifyGuildChannelParams>.ModifyRoute(IPathable path, ulong id,
        ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
