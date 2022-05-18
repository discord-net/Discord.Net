using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IActivityModel
    {
        string Id { get; set; }
        string Url { get; set; }
        string Name { get; set; }
        ActivityType Type { get; set; }
        string Details { get; set; }
        string State { get; set; }
        ActivityProperties Flags { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        IEmojiModel Emoji { get; set; }
        ulong? ApplicationId { get; set; }
        string SyncId { get; set; }
        string SessionId { get; set; }


        #region Assets
        string LargeImage { get; set; }
        string LargeText { get; set; }
        string SmallImage { get; set; }
        string SmallText { get; set; }
        #endregion

        #region Party
        string PartyId { get; set; }
        long[] PartySize { get; set; }
        #endregion

        #region Secrets
        string JoinSecret { get; set; }
        string SpectateSecret { get; set; }
        string MatchSecret { get; set; }
        #endregion

        #region Timestamps
        DateTimeOffset? TimestampStart { get; set; }
        DateTimeOffset? TimestampEnd { get; set; }
        #endregion
    }
}
