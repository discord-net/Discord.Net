using Discord.Net.Rest;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Discord.API.Rest;

internal class ModifyChannelInviteTargetUsersMultipartParams
{
    public ulong[] UserIds { get; }

    public ModifyChannelInviteTargetUsersMultipartParams(IEnumerable<ulong> userIds)
    {
        UserIds = userIds.ToArray();
    }

    public IReadOnlyDictionary<string, object> ToDictionary()
    {
        var payload = new Dictionary<string, object>();
        var ms = new MemoryStream(Encoding.UTF8.GetBytes("Users\n" + string.Join('\n', UserIds)));
        payload["target_users_file"] = new MultipartFile(ms, "file.csv", "text/csv");
        return payload;
    }
}
