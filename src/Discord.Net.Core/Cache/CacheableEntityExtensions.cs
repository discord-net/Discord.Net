using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    internal static class CacheableEntityExtensions
    {
        public static IActivityModel ToModel<TModel>(this RichGame richGame) where TModel : WritableActivityModel, new()
        {
            return new TModel()
            {
                ApplicationId = richGame.ApplicationId,
                SmallImage = richGame.SmallAsset?.ImageId,
                SmallText = richGame.SmallAsset?.Text,
                LargeImage = richGame.LargeAsset?.ImageId,
                LargeText = richGame.LargeAsset?.Text,
                Details = richGame.Details,
                Flags = richGame.Flags,
                Name = richGame.Name,
                Type = richGame.Type,
                JoinSecret = richGame.Secrets?.Join,
                SpectateSecret = richGame.Secrets?.Spectate,
                MatchSecret = richGame.Secrets?.Match,
                State = richGame.State,
                PartyId = richGame.Party?.Id,
                PartySize = richGame.Party?.Members != null && richGame.Party?.Capacity != null
                                                ? new long[] { richGame.Party.Members, richGame.Party.Capacity }
                                                : null,
                TimestampEnd = richGame.Timestamps?.End,
                TimestampStart = richGame.Timestamps?.Start
            };
        }

        public static IActivityModel ToModel<TModel>(this SpotifyGame spotify) where TModel : WritableActivityModel, new()
        {
            return new TModel()
            {
                Name = spotify.Name,
                SessionId = spotify.SessionId,
                SyncId = spotify.TrackId,
                LargeText = spotify.AlbumTitle,
                Details = spotify.TrackTitle,
                State = string.Join(";", spotify.Artists),
                TimestampEnd = spotify.EndsAt,
                TimestampStart = spotify.StartedAt,
                LargeImage = spotify.AlbumArt,
                Type = ActivityType.Listening,
                Flags = spotify.Flags,
            };
        }

        public static IActivityModel ToModel<TModel, TEmoteModel>(this CustomStatusGame custom)
            where TModel : WritableActivityModel, new()
            where TEmoteModel : WritableEmojiModel, new()
        {
            return new TModel
            {
                Type = ActivityType.CustomStatus,
                Name = custom.Name,
                State = custom.State,
                Emoji = custom.Emote.ToModel<TEmoteModel>(),
                CreatedAt = custom.CreatedAt
            };
        }

        public static IActivityModel ToModel<TModel>(this StreamingGame stream) where TModel : WritableActivityModel, new()
        {
            return new TModel
            {
                Name = stream.Name,
                Url = stream.Url,
                Flags = stream.Flags,
                Details = stream.Details
            };
        }

        public static IEmojiModel ToModel<TModel>(this IEmote emote) where TModel : WritableEmojiModel, new()
        {
            var model = new TModel()
            {
                Name = emote.Name
            };

            if(emote is GuildEmote guildEmote)
            {
                model.Id = guildEmote.Id;
                model.IsAnimated = guildEmote.Animated;
                model.IsAvailable = guildEmote.IsAvailable;
                model.IsManaged = guildEmote.IsManaged;
                model.CreatorId = guildEmote.CreatorId;
                model.RequireColons = guildEmote.RequireColons;
                model.Roles = guildEmote.RoleIds.ToArray();
            }

            if(emote is Emote e)
            {
                model.IsAnimated = e.Animated;
                model.Id = e.Id;
            }

            return model;
        }
    }
}
