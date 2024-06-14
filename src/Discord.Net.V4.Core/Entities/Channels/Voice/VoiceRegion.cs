namespace Discord;

public readonly struct VoiceRegion(string id, string name, bool isOptimal, bool isDeprecated, bool isCustom) :
    IEntityProperties<Models.Json.VoiceRegion>,
    IConstructable<VoiceRegion, Models.Json.VoiceRegion>,
    IIdentifiable<string>
{
    public readonly string Id = id;
    public readonly string Name = name;
    public readonly bool IsOptimal = isOptimal;
    public readonly bool IsDeprecated = isDeprecated;
    public readonly bool IsCustom = isCustom;

    public Models.Json.VoiceRegion ToApiModel(Models.Json.VoiceRegion? existing = default)
    {
        return existing ??= new Models.Json.VoiceRegion()
        {
            Id = Id,
            Name = Name,
            IsCustom = IsCustom,
            IsDeprecated = IsDeprecated,
            IsOptimal = IsOptimal
        };
    }

    public static VoiceRegion Construct(IDiscordClient client, Models.Json.VoiceRegion model)
    {
        return new VoiceRegion(
            model.Id,
            model.Name,
            model.IsOptimal,
            model.IsDeprecated,
            model.IsCustom
        );
    }

    string IIdentifiable<string>.Id => Id;
}
