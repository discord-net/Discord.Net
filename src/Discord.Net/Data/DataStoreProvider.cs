namespace Discord.Data
{
    public delegate DataStore DataStoreProvider(int shardId, int totalShards, int guildCount, int dmCount);
}
