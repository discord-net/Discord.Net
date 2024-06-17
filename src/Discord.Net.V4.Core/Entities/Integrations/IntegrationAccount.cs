namespace Discord;

public readonly struct IntegrationAccount(string id, string name) :
    IEntityProperties<Models.Json.IntegrationAccount>,
    IConstructable<IntegrationAccount, Models.Json.IntegrationAccount>
{
    public readonly string Id = id;
    public readonly string Name = name;

    public Models.Json.IntegrationAccount ToApiModel(Models.Json.IntegrationAccount? existing = default)
        => new() {Id = Id, Name = Name};

    public static IntegrationAccount Construct(IDiscordClient client, Models.Json.IntegrationAccount model)
        => new(model.Id, model.Name);
}
