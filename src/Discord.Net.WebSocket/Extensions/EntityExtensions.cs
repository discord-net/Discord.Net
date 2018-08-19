using System.Collections.Immutable;
using System.Linq;
using Discord.API;

namespace Discord.WebSocket
{
    internal static class EntityExtensions
    {
        public static IActivity ToEntity(this API.Game model)
        {
            // Spotify Game
            if (model.SyncId.IsSpecified)
            {
                var assets = model.Assets.GetValueOrDefault()?.ToEntity();
                var albumText = assets?[1]?.Text;
                var albumArtId = assets?[1]?.ImageId?.Replace("spotify:", "");
                var timestamps = model.Timestamps.IsSpecified ? model.Timestamps.Value.ToEntity() : null;
                return new SpotifyGame
                {
                    Name = model.Name,
                    SessionId = model.SessionId.GetValueOrDefault(),
                    TrackId = model.SyncId.Value,
                    TrackUrl = CDN.GetSpotifyDirectUrl(model.SyncId.Value),
                    AlbumTitle = albumText,
                    TrackTitle = model.Details.GetValueOrDefault(),
                    Artists = model.State.GetValueOrDefault()?.Split(';').Select(x => x?.Trim()).ToImmutableArray(),
                    Duration = timestamps?.End - timestamps?.Start,
                    AlbumArtUrl = albumArtId != null ? CDN.GetSpotifyAlbumArtUrl(albumArtId) : null,
                    Type = ActivityType.Listening
                };
            }

            // Rich Game
            if (model.ApplicationId.IsSpecified)
            {
                var appId = model.ApplicationId.Value;
                var assets = model.Assets.GetValueOrDefault()?.ToEntity(appId);
                return new RichGame
                {
                    ApplicationId = appId,
                    Name = model.Name,
                    Details = model.Details.GetValueOrDefault(),
                    State = model.State.GetValueOrDefault(),
                    SmallAsset = assets?[0],
                    LargeAsset = assets?[1],
                    Party = model.Party.IsSpecified ? model.Party.Value.ToEntity() : null,
                    Secrets = model.Secrets.IsSpecified ? model.Secrets.Value.ToEntity() : null,
                    Timestamps = model.Timestamps.IsSpecified ? model.Timestamps.Value.ToEntity() : null
                };
            }

            // Stream Game
            if (model.StreamUrl.IsSpecified)
                return new StreamingGame(
                    model.Name,
                    model.StreamUrl.Value);
            // Normal Game
            return new Game(model.Name, model.Type.GetValueOrDefault() ?? ActivityType.Playing);
        }

        // (Small, Large)
        public static GameAsset[] ToEntity(this GameAssets model, ulong? appId = null) => new[]
        {
            model.SmallImage.IsSpecified
                ? new GameAsset
                {
                    ApplicationId = appId,
                    ImageId = model.SmallImage.GetValueOrDefault(),
                    Text = model.SmallText.GetValueOrDefault()
                }
                : null,
            model.LargeImage.IsSpecified
                ? new GameAsset
                {
                    ApplicationId = appId,
                    ImageId = model.LargeImage.GetValueOrDefault(),
                    Text = model.LargeText.GetValueOrDefault()
                }
                : null
        };

        public static GameParty ToEntity(this API.GameParty model)
        {
            // Discord will probably send bad data since they don't validate anything
            long current = 0, cap = 0;
            if (model.Size?.Length != 2)
                return new GameParty
                {
                    Id = model.Id,
                    Members = current,
                    Capacity = cap
                };
            current = model.Size[0];
            cap = model.Size[1];

            return new GameParty
            {
                Id = model.Id,
                Members = current,
                Capacity = cap
            };
        }

        public static GameSecrets ToEntity(this API.GameSecrets model) =>
            new GameSecrets(model.Match, model.Join, model.Spectate);

        public static GameTimestamps ToEntity(this API.GameTimestamps model) =>
            new GameTimestamps(model.Start.ToNullable(), model.End.ToNullable());
    }
}
