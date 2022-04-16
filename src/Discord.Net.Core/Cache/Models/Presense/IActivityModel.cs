using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IActivityModel
    {
        string Id { get; }
        string Url { get; }
        string Name { get; }
        ActivityType Type { get; }
        string Details { get; }
        string State { get; }
        ActivityProperties Flags { get; }
        DateTimeOffset CreatedAt { get; }
        IEmojiModel Emoji { get; }
        ulong? ApplicationId { get; }
        string SyncId { get; }
        string SessionId { get; }


        #region Assets
        string LargeImage { get; }
        string LargeText { get; }
        string SmallImage { get; }
        string SmallText { get; }
        #endregion

        #region Party
        string PartyId { get; }
        long[] PartySize { get; }
        #endregion

        #region Secrets
        string JoinSecret { get; }
        string SpectateSecret { get; }
        string MatchSecret { get; }
        #endregion

        #region Timestamps
        DateTimeOffset? TimestampStart { get; }
        DateTimeOffset? TimestampEnd { get; }
        #endregion
    }

    internal class WritableActivityModel : IActivityModel
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public ActivityType Type { get; set; }
        public string Details { get; set; }
        public string State { get; set; }
        public ActivityProperties Flags { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public IEmojiModel Emoji { get; set; }
        public ulong? ApplicationId { get; set; }
        public string SyncId { get; set; }
        public string SessionId { get; set; }


        #region Assets
        public string LargeImage { get; set; }
        public string LargeText { get; set; }
        public string SmallImage { get; set; }
        public string SmallText { get; set; }
        #endregion

        #region Party
        public string PartyId { get; set; }
        public long[] PartySize { get; set; }
        #endregion

        #region Secrets
        public string JoinSecret { get; set; }
        public string SpectateSecret { get; set; }
        public string MatchSecret { get; set; }
        #endregion

        #region Timestamps
        public DateTimeOffset? TimestampStart { get; set; }
        public DateTimeOffset? TimestampEnd { get; set; }
        #endregion
    }
}
