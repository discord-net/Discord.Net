using Discord.Models.Json;

namespace Discord.Channels.Invites;

public sealed class CreateChannelInviteProperties : IEntityProperties<CreateChannelInviteParams>
{
    public Optional<int> MaxAge { get; set; }
    public Optional<int> MaxUses { get; set; }
    public Optional<bool> Temporary { get; set; }
    public Optional<bool> Unique { get; set; }
    public Optional<InviteTargetType> TargetType { get; set; }
    public Optional<EntityOrId<ulong, IUserActor>> TargetUser { get; set; }
    public Optional<ulong> TargetApplicationId { get; set; }
    
    public CreateChannelInviteParams ToApiModel(CreateChannelInviteParams? existing = default)
    {
        return new CreateChannelInviteParams()
        {
            MaxAge = MaxAge,
            MaxUses = MaxUses,
            TargetType = TargetType.MapToInt(),
            TargetApplicationId = TargetApplicationId,
            IsTemporary = Temporary,
            IsUnique = Unique,
            TargetUserId = TargetUser.MapToId(),
        };
    }
}