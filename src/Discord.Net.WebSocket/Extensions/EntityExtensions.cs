namespace Discord.WebSocket
{
    internal static class EntityExtensions
    {
        public static Activity ToEntity(this API.Activity model)
        {
            return new Activity(model.Name,
                model.StreamUrl.GetValueOrDefault(null),
                model.Type.GetValueOrDefault(null) ?? ActivityType.Playing);
        }
    }
}
