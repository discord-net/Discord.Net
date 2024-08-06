using Discord.Models;

namespace Discord;

public readonly struct IntegrationAccount(
    string id,
    string name
):
    IEntityProperties<IIntegrationAccountModel>,
    IConstructable<IntegrationAccount, IIntegrationAccountModel>
{
    public readonly string Id = id;
    public readonly string Name = name;

    public IIntegrationAccountModel ToApiModel(IIntegrationAccountModel? existing = default)
        => new Models.Json.IntegrationAccount {Id = Id, Name = Name};

    public static IntegrationAccount Construct(IDiscordClient client, IIntegrationAccountModel model)
        => new(model.Id, model.Name);
}
