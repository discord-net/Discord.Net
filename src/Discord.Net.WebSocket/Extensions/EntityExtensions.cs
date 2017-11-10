namespace Discord.WebSocket
{
    internal static class EntityExtensions
    {
        public static IActivity ToEntity(this API.Game model)
        {
            // Rich Game
            if (model.Details.IsSpecified)
            {
                var appId = model.ApplicationId.ToNullable();
                return new RichGame
                {
                    ApplicationId = appId,
                    Name = model.Name,
                    Details = model.Details.GetValueOrDefault(),
                    State = model.State.GetValueOrDefault(),
                    
                    Assets = model.Assets.GetValueOrDefault()?.ToEntity(appId ?? 0),
                    Party = model.Party.GetValueOrDefault()?.ToEntity(),
                    Secrets = model.Secrets.GetValueOrDefault()?.ToEntity(),
                    Timestamps = model.Timestamps.GetValueOrDefault()?.ToEntity()
                    
                };
            }
            // Stream Game
            if (model.StreamUrl.IsSpecified)
            {
                return new StreamingGame(
                    model.Name, 
                    model.StreamUrl.Value, 
                    model.StreamType.Value.GetValueOrDefault());
            }
            // Normal Game
            return new Game(model.Name);
        }

        public static GameAssets ToEntity(this API.GameAssets model, ulong appId)
        {
            return new GameAssets
            {
                Large = new GameAsset
                {
                    ApplicationId = appId,
                    ImageId = model.LargeImage.GetValueOrDefault(),
                    Text = model.LargeText.GetValueOrDefault()
                },
                Small = new GameAsset
                {
                    ApplicationId = appId,
                    ImageId = model.LargeImage.GetValueOrDefault(),
                    Text = model.LargeText.GetValueOrDefault()
                },
            };
        }

        public static GameParty ToEntity(this API.GameParty model)
        {
            return new GameParty
            {
                Id = model.Id,
                Size = model.Size
            };
        }

        public static GameSecrets ToEntity(this API.GameSecrets model)
        {
            return new GameSecrets(model.Match, model.Join, model.Spectate);
        }

        public static GameTimestamps ToEntity(this API.GameTimestamps model)
        {
            return new GameTimestamps(model.Start.ToNullable(), model.End.ToNullable());
        }
    }
}
