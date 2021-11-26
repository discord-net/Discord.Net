using System.Threading.Tasks;
using Discord.Commands;

namespace _03_sharded_client.Modules
{
    // Remember to make your module reference the ShardedCommandContext
    public class PublicModule : ModuleBase<ShardedCommandContext>
    {
        [Command("info")]
        public async Task InfoAsync()
        {
            var msg = $@"Hi {Context.User}! There are currently {Context.Client.Shards.Count} shards!
                This guild is being served by shard number {Context.Client.GetShardFor(Context.Guild).ShardId}";
            await ReplyAsync(msg);
        }
    }
}
