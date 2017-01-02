namespace Discord.WebSocket
{
    internal static class EntityExtensions
    {
        public static Game ToEntity(this API.Game model)
        {
            return new Game(model.Name,
                model.StreamUrl.GetValueOrDefault(null),
                model.StreamType.GetValueOrDefault(null) ?? StreamType.NotStreaming);
        }
    }
}
