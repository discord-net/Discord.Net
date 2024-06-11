namespace Discord;

public interface ILoadableTextChannelEntitySource<TTextChannel> :
    ITextChannelEntitySource<TTextChannel>,
    ILoadableGuildChannelEntitySource<TTextChannel>,
    ILoadableMessageChannelEntitySource<TTextChannel>
    where TTextChannel : class, ITextChannel;

public interface ITextChannelEntitySource<out TTextChannel> :
    IMessageChannelEntitySource<TTextChannel>,
    IGuildChannelEntitySource<TTextChannel>
    where TTextChannel : ITextChannel;
