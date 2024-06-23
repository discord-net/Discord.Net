using Discord.Models.Json;

namespace Discord;

public sealed class ModifyWebhookProperties : IEntityProperties<ModifyWebhookParams>
{
    public Optional<string> Name { get; set; }
    public Optional<Image?> Avatar { get; set; }
    public Optional<EntityOrId<ulong, ITextChannel>> Channel { get; set; }

    public ModifyWebhookParams ToApiModel(ModifyWebhookParams? existing = default) =>
        existing ?? new ModifyWebhookParams
        {
            Avatar = Avatar.Map(v => v?.ToImageData()), Name = Name, ChannelId = Channel.MapToId()
        };
}
