namespace Discord.WebSocket.Data
{
    public delegate IDataStore DataStoreProvider(int shardId, int totalShards, int guildCount, int dmCount);
}
