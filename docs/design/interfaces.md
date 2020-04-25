# Discord.Net 3.0 Interface Design #

This is mostly just a collection of notes for interface design for 3.0. As
such, don't expect them to be comprehensive or up to date.

## Why no `ITextChannel`, `IGuildChannel`, etc? ##

Usually, it's better to design types in terms of the fields they have, rather
than using a discriminated union like the current `IChannel` interface.

However, the design chosen in 1.0 led to [traits][wiki-traits], which in C# was
*notoriously* hard to work with, debug and expand. Most notably, in
`1.0.0-beta2`, the internal implementation details were exposed in form of
`SocketUserMessage`, `SocketTextChannel` etc. simply to make consumption
easier. The effect of this has been that writing portable, re-usable code is
very hard, and has also caused us to be much slower when implementing updates
from Discord.

To simplify this, the current design has gone back to a centralised `IChannel`
type, a la [D#+'s DiscordChannel][dsharpplus-channel] implementation. However,
we are going to be using [Nullable Reference Types][nrts] to help reduce some
of the inevitable errors which will come around with a discriminated union
approach. Furthermore, this design allows in the future a traits-like system to
be implemented on top of it, once the design flaws have been worked out.

## Future Plans ##

Ideally, [shapes] will become a thing, making the previous design *much* easier
to implement and maintain, on top of the current design. To give a small
example:

```cs
shape SChannel<T>
{
    public ulong Id { get; }
}

shape SGuildChannel<T> : SChannel<T>
{
    public ulong GuildId { get; }
    public int? Position { get; }
    public IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }
    public string Name { get; }
}

shape STextChannel<T> : SChannel<T>
{
    public ulong? LastMessageId { get; }
    public DateTimeOffset LastPinTimestamp { get; }
}

shape SGuildTextChannel<T> : SGuildChannel<T>, STextChannel<T>
{
    public string? Topic { get; }
    public bool IsNsfw { get; }
    public int? RateLimit { get; }
}

public ValueTask<bool> DeleteLastMessageAsync<T>(T channel)
    where T : STextChannel<T>
{
    return channel.LastMessageId switch {
        null => new ValueTask<bool>(false),
        0 => new ValueTask<bool>(false),
        _ => new ValueTask<bool>(DeleteMessageAsync(channel.LastMessageId))
    };

    static async Task<bool> DeleteMessage(ulong messageId)
    {
        var message = await GetMessageAsync(messageId);

        if (message == null)
            return false;

        await message?.DeleteAsync();
        return true;
    }
}
```

Internally, `DeleteLastMessageAsync` will work on any channel, but as long
as that channel exposes the correct APIs, the above method will function as the
user expects.

Additionally, this provides a huge advantage in that the above code is
extremely easy to unit test; any type which fulfils the contract of
`STextChannel<T>` can be used, even if it doesn't implement the `IChannel`
interface.


[wiki-traits]: https://en.wikipedia.org/wiki/Trait_(computer_programming)
[dsharpplus-channel]: https://dsharpplus.emzi0767.com/api/DSharpPlus.Entities.DiscordChannel.html
[nrts]: https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references
[shapes]: https://github.com/dotnet/csharplang/issues/164
