using Discord.Models.Json;

namespace Discord;

public sealed class CreateDMProperties : IEntityProperties<CreateDMChannelParams>
{
    public required EntityOrId<ulong, IUser> Recipient { get; set; }
    
    public CreateDMChannelParams ToApiModel(CreateDMChannelParams? existing = default)
    {
        return new CreateDMChannelParams() {RecipientId = Recipient.Id};
    }
}
