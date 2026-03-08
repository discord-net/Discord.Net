using Discord.Net.Converters;
using Discord.Net.Rest;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Discord.API.Rest;

internal class CreateChannelInviteMultipartParams
{
    private static readonly JsonSerializer _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };

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
        var d = new Dictionary<string, object>();

        var payload = new Dictionary<string, object>();

        if (MaxAge.IsSpecified)
            payload["max_age"] = MaxAge.Value;
        if (MaxUses.IsSpecified)
            payload["max_uses"] = MaxUses.Value;
        if (IsTemporary.IsSpecified)
            payload["temporary"] = IsTemporary.Value;
        if (IsUnique.IsSpecified)
            payload["unique"] = IsUnique.Value;
        if (TargetType.IsSpecified)
            payload["target_type"] = TargetType.Value;
        if (TargetUserId.IsSpecified)
            payload["target_user_id"] = TargetUserId.Value;
        if (TargetApplicationId.IsSpecified)
            payload["target_application_id"] = TargetApplicationId.Value;
        if (RoleIds.IsSpecified)
            payload["role_ids"] = RoleIds.Value;

        var json = new StringBuilder();
        using (var text = new StringWriter(json))
        using (var writer = new JsonTextWriter(text))
            _serializer.Serialize(writer, payload);

        d["payload_json"] = json.ToString();

        var ms = new MemoryStream(Encoding.UTF8.GetBytes(string.Join('\n', UserIds)));
        d["target_users_file"] = new MultipartFile(ms, "file.csv", "text/csv");

        return d;
    }
}
