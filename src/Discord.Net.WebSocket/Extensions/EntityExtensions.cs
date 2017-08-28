namespace Discord.WebSocket
{
    internal static class EntityExtensions
    {
        public static Game ToEntity(this API.Game model)
        {
            return new Game(model.Name,
                model.StreamUrl.GetValueOrDefault(null),
                model.StreamType.GetValueOrDefault(null) ?? StreamType.NotStreaming,
                model.Details.GetValueOrDefault(),
                model.State.GetValueOrDefault(),
                model.ApplicationId.ToNullable(),
                model.Assets.GetValueOrDefault(null)?.ToEntity(),
                model.Party.GetValueOrDefault(null)?.ToEntity(),
                model.Secrets.GetValueOrDefault(null)?.ToEntity(),
                model.Timestamps.GetValueOrDefault(null)?.ToEntity()
                );
        }

        public static GameAssets ToEntity(this API.GameAssets model)
        {
            return new GameAssets(model.SmallText.GetValueOrDefault(),
                model.SmallImage.GetValueOrDefault(),
                model.LargeText.GetValueOrDefault(),
                model.LargeImage.GetValueOrDefault());
        }

        public static GameParty ToEntity(this API.GameParty model)
        {
            return new GameParty(model.Size, model.Id);
        }

        public static GameSecrets ToEntity(this API.GameSecrets model)
        {
            return new GameSecrets(model.Match, model.Join, model.Spectate);
        }

        public static GameTimestamps ToEntity(this API.GameTimestamps model)
        {
            return new GameTimestamps(model.Start, model.End);
        }
    }
}
