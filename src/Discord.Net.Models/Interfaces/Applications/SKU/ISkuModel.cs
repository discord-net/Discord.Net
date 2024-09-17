namespace Discord.Models;

[ModelEquality]
public partial interface ISkuModel : IEntityModel<ulong>
{
    int Type { get; }
    ulong ApplicationId { get; }
    string Name { get; }
    string Slug { get; }
    int Flags { get; }
}