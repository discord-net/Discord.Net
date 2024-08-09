using Discord.Gateway;

namespace FeatureSamples.Gateway;

public class GatewayEvents
{
    public static async Task RunAsync(DiscordGatewayClient client)
    {
        client.UserUpdated.Subscribe(OnUserUpdateAsync);
        client.UserUpdated.Subscribe(OnUserUpdate);
    }

    public static async Task OnUserUpdateAsync(GatewayCurrentUser user)
    {

    }

    public static void OnUserUpdate(ulong user)
    {

    }
}
