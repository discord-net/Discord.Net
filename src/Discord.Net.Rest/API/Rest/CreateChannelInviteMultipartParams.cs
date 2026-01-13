using Discord.Net.Rest;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Discord.API.Rest;

internal class CreateChannelInviteMultipartParams
{
    public ulong[] UserIds { get; }

    public Optional<int> MaxAge { get; set; }

    public Optional<int> MaxUses { get; set; }

    public Optional<bool> IsTemporary { get; set; }

    public Optional<bool> IsUnique { get; set; }

    public Optional<TargetUserType> TargetType { get; set; }

    public Optional<ulong> TargetUserId { get; set; }

    public Optional<ulong> TargetApplicationId { get; set; }

    public Optional<ulong[]> RoleIds { get; set; }

    public CreateChannelInviteMultipartParams(IEnumerable<ulong> userIds)
    {
        UserIds = userIds.ToArray();
    }

    public IReadOnlyDictionary<string, object> ToDictionary()
    {
        var payload = new Dictionary<string, object>();

        if (MaxAge.IsSpecified)
            payload["max_age"] = MaxAge.Value.ToString();
        if (MaxUses.IsSpecified)
            payload["max_uses"] = MaxUses.Value.ToString();
        if (IsTemporary.IsSpecified)
            payload["temporary"] = IsTemporary.Value.ToString();
        if (IsUnique.IsSpecified)
            payload["unique"] = IsUnique.Value.ToString();
        if (TargetType.IsSpecified)
            payload["target_type"] = ((int)TargetType.Value).ToString();
        if (TargetUserId.IsSpecified)
            payload["target_user_id"] = TargetUserId.Value.ToString();
        if (TargetApplicationId.IsSpecified)
            payload["target_application_id"] = TargetApplicationId.Value.ToString();
        if (RoleIds.IsSpecified)
            payload["role_ids"] = string.Join(',', RoleIds.Value);

        var ms = new MemoryStream(Encoding.UTF8.GetBytes("Users\n" + string.Join('\n', UserIds)));
        payload["target_users_file"] = new MultipartFile(ms, "file.csv", "text/csv");

        return payload;
    }
}
