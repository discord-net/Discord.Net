using Discord.Net.Hanz.Utils.Bakery;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;

public interface ITypePath
{
    ImmutableEquatableArray<string> Parts { get; }
}