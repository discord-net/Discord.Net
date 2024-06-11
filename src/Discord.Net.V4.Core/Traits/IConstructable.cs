using Discord.Models;

namespace Discord;

public interface IConstructable<out TSelf, in TModel>
    where TSelf : IConstructable<TSelf, TModel>
{
    // see https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2252#when-to-suppress-warnings
#pragma warning disable CA2252
    static abstract TSelf Construct(IDiscordClient client, TModel model);
#pragma warning restore CA2252
}

public interface IConstructable<out TSelf, in TModel, in TContext>
    where TSelf : IConstructable<TSelf, TModel, TContext>
{
    // see https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2252#when-to-suppress-warnings
#pragma warning disable CA2252
    static abstract TSelf Construct(IDiscordClient client, TModel model, TContext context);
#pragma warning restore CA2252
}
