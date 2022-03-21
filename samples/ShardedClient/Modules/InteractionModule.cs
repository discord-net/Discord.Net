using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace ShardedClient.Modules
{
    // A display of portability, which shows how minimal the difference between the 2 frameworks is.
    public class InteractionModule : InteractionModuleBase<ShardedInteractionContext>
    {
        [SlashCommand("info", "Information about this shard.")]
        public async Task InfoAsync()
        {
            var msg = $@"Hi {Context.User}! There are currently {Context.Client.Shards.Count} shards!
                This guild is being served by shard number {Context.Client.GetShardFor(Context.Guild).ShardId}";
            await RespondAsync(msg);
        }
    }
}
