using Discord.Models;
using Discord.Rest;

namespace Discord;

[
    FetchableOfMany(nameof(Routes.ListSKUs))
]
public partial interface ISku :
    ISnowflakeEntity<ISkuModel>,
    ISkuActor
{
    SkuType Type { get; }
    string Name { get; }
    string Slug { get; }
    SkuFlags Flags { get; }
}