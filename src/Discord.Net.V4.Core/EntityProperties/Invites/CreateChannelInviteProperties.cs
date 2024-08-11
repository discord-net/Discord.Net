using Discord.Models.Json;

namespace Discord;

public sealed class CreateChannelInviteProperties : IEntityProperties<CreateChannelInviteParams>
{
    public Optional<TimeSpan> MaxAge { get; set; }
    public Optional<int> MaxUses { get; set; }
    public Optional<bool> IsTemporary { get; set; }
    public Optional<bool> IsUnique { get; set; }
    public Optional<InviteTargetType> TargetType { get; set; }
    public Optional<EntityOrId<ulong, IUser>> TargetUser { get; set; }
    public Optional<ulong> TargetApplicationId { get; set; }


    public CreateChannelInviteParams ToApiModel(CreateChannelInviteParams? existing = default) =>
        existing ??= new CreateChannelInviteParams
        {
            IsTemporary = IsTemporary,
            IsUnique = IsUnique,
            MaxAge = MaxAge.Map(v => (int)v.TotalSeconds),
            MaxUses = MaxUses,
            TargetType = TargetType.MapToInt(),
            TargetApplicationId = TargetApplicationId,
            TargetUserId = TargetUser.MapToId()
        };
}
