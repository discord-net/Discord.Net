namespace Discord.Models;

[ModelEquality, HasPartialVariant]
public partial interface IGuildEmoteModel : IEmoteModel, IEntityModel<ulong>
{
    ulong[] Roles { get; }
    bool RequireColons { get; }
    bool IsManaged { get; }
    bool IsAnimated { get; }
    bool IsAvailable { get; }
    ulong? UserId { get; }
}
