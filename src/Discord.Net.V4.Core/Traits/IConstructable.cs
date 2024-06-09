using Discord.Models;

namespace Discord;

public interface IConstructable<in TModel>
    where TModel : IEntityModel?
{
    // see https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2252#when-to-suppress-warnings
#pragma warning disable CA2252
    internal static abstract T Construct<T>(TModel model);
#pragma warning restore CA2252

    static T Create<T>(TModel model)
        where T : IConstructable<TModel> =>
        Construct<T>(model);
}
