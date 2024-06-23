namespace Discord.Models;

public interface ISelectMenuComponentModel : IMessageComponentModel
{
    string CustomId { get; }
    IEnumerable<ISelectMenuOptionModel> Options { get; }
    int[]? ChannelTypes { get; }
    string? Placeholder { get; }
    IEnumerable<ISelectMenuDefaultValueModel> DefaultValues { get; }
    int? MinValues { get; }
    int? MaxValues { get; }
    bool? IsDisabled { get; }
}
