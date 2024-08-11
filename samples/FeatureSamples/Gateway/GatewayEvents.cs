using Discord.Gateway;

namespace FeatureSamples.Gateway;

public class GatewayEvents
{
    public static async Task RunAsync(DiscordGatewayClient client)
    {
        client.UserUpdated += OnUserUpdateAsync;
        client.UserUpdatedEvent.Subscribe(OnUserUpdate);


    }

    public static async ValueTask OnUserUpdateAsync(GatewayCurrentUser user)
    {

    }

    public static void OnUserUpdate(ulong user)
    {

    }
}
