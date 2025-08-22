using System.Text.Json;
using Discord.Models.Models;
using Discord.Models.Models.Json;

var ctx = new DiscordJsonContext(null);

var result = JsonSerializer.Deserialize(
    """
    {
        "id": "259053800755691520",
        "username": "quinchs",
        "avatar": "4c66b8598cc4bb320f70801b5c7f68f7",
        "discriminator": "0",
        "public_flags": 4194560,
        "flags": 4194560,
        "banner": "2fe5b3ede49a77ca82614bae447632a1",
        "accent_color": 2904788,
        "global_name": "quinchs",
        "avatar_decoration_data": null,
        "collectibles": null,
        "display_name_styles": null,
        "banner_color": "#2c52d4",
        "clan": {
            "identity_guild_id": "848176216011046962",
            "identity_enabled": true,
            "tag": "DNET",
            "badge": "659bc6e3c9351eedf4e221ac0da0d534"
        },
        "primary_guild": {
            "identity_guild_id": "848176216011046962",
            "identity_enabled": true,
            "tag": "DNET",
            "badge": "659bc6e3c9351eedf4e221ac0da0d534"
        }
    }
    """,
    ctx.UserModel
);

Console.WriteLine(result);

Console.WriteLine(
    JsonSerializer.Serialize(
        result, typeof(IUserModel), ctx
        )
    );