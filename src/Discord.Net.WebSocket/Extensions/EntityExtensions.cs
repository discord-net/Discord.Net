using Discord.Rest;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.WebSocket
{
    internal static class EntityExtensions
    {
        #region Emotes
        public static IEmote ToEntity(this IEmojiModel model)
        {
            if (model.Id.HasValue)
                return new Emote(model.Id.Value, model.Name, model.IsAnimated);
            else
                return new Emoji(model.Name);
        }
        #endregion

        #region Activity
        public static IActivity ToEntity(this IActivityModel model)
        {
            #region  Custom Status Game
            if (model.Id != null && model.Id == "custom")
            {
                return new CustomStatusGame()
                {
                    Type = ActivityType.CustomStatus,
                    Name = model.Name,
                    State = model.State,
                    Emote = model.Emoji?.ToIEmote(),
                    CreatedAt = model.CreatedAt,
                };
            }
            #endregion

            #region Spotify Game
            if (model.SyncId != null)
            {
                string albumText = model.LargeText;
                string albumArtId = model.LargeImage?.Replace("spotify:", "");
                return new SpotifyGame
                {
                    Name = model.Name,
                    SessionId = model.SessionId,
                    TrackId = model.SyncId,
                    TrackUrl = CDN.GetSpotifyDirectUrl(model.SyncId),
                    AlbumTitle = albumText,
                    TrackTitle = model.Details,
                    Artists = model.State?.Split(';').Select(x => x?.Trim()).ToImmutableArray(),
                    StartedAt = model.TimestampStart,
                    EndsAt = model.TimestampEnd,
                    Duration = model.TimestampEnd - model.TimestampStart,
                    AlbumArtUrl = albumArtId != null ? CDN.GetSpotifyAlbumArtUrl(albumArtId) : null,
                    Type = ActivityType.Listening,
                    Flags = model.Flags,
                    AlbumArt = model.LargeImage,
                };
            }
            #endregion

            #region Rich Game
            if (model.ApplicationId.HasValue)
            {
                ulong appId = model.ApplicationId.Value;
                return new RichGame
                {
                    ApplicationId = appId,
                    Name = model.Name,
                    Details = model.Details,
                    State = model.State,
                    SmallAsset = new GameAsset
                    {
                        Text = model.SmallText,
                        ImageId = model.SmallImage,
                        ApplicationId = appId,
                    },
                    LargeAsset = new GameAsset
                    {
                        Text = model.LargeText,
                        ApplicationId = appId,
                        ImageId = model.LargeImage
                    },
                    Party = model.PartyId != null ? new GameParty
                    {
                        Id = model.PartyId,
                        Capacity = model.PartySize?.Length > 1 ? model.PartySize[1] : 0,
                        Members = model.PartySize?.Length > 0 ? model.PartySize[0] : 0
                    } : null,
                    Secrets = model.JoinSecret != null || model.SpectateSecret != null || model.MatchSecret != null ? new GameSecrets(model.MatchSecret, model.JoinSecret, model.SpectateSecret) : null,
                    Timestamps = model.TimestampStart.HasValue || model.TimestampEnd.HasValue ? new GameTimestamps(model.TimestampStart, model.TimestampEnd) : null,
                    Flags = model.Flags
                };
            }
            #endregion

            #region  Stream Game
            if (model.Url != null)
            {
                return new StreamingGame(
                    model.Name,
                    model.Url)
                {
                    Flags = model.Flags,
                    Details = model.Details
                };
            }
            #endregion

            #region  Normal Game
            return new Game(model.Name, model.Type, model.Flags, model.Details);
            #endregion
        }

        // (Small, Large)
        public static GameAsset[] ToEntity(this API.GameAssets model, ulong? appId = null)
        {
            return new GameAsset[]
            {
                model.SmallImage.IsSpecified ? new GameAsset
                {
                    ApplicationId = appId,
                    ImageId = model.SmallImage.GetValueOrDefault(),
                    Text = model.SmallText.GetValueOrDefault()
                } : null,
                model.LargeImage.IsSpecified ? new GameAsset
                {
                    ApplicationId = appId,
                    ImageId = model.LargeImage.GetValueOrDefault(),
                    Text = model.LargeText.GetValueOrDefault()
                } : null,
            };
        }

        public static GameParty ToEntity(this API.GameParty model)
        {
            // Discord will probably send bad data since they don't validate anything
            long current = 0, cap = 0;
            if (model.Size?.Length == 2)
            {
                current = model.Size[0];
                cap = model.Size[1];
            }
            return new GameParty
            {
                Id = model.Id,
                Members = current,
                Capacity = cap,
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
        #endregion
    }
}
