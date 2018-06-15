// (Note: This precondition is obsolete, it is recommended to use the
// RequireOwnerAttribute that is bundled with Discord.Commands)

using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

// Inherit from PreconditionAttribute
public class RequireOwnerAttribute : PreconditionAttribute
{
    // Override the CheckPermissions method
    public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        // Get the client via Depedency Injection
        var client = services.GetRequiredService<DiscordSocketClient>();
        // Get the ID of the bot's owner
        var appInfo = await client.GetApplicationInfoAsync().ConfigureAwait(false);
        var ownerId = appInfo.Owner.Id;
        // If this command was executed by that user, return a success
        if (context.User.Id == ownerId)
            return PreconditionResult.FromSuccess();
        // Since it wasn't, fail
        else
            return PreconditionResult.FromError("You must be the owner of the bot to run this command.");
    }
}
