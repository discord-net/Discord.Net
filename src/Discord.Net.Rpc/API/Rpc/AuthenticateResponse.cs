#pragma warning disable CS1591
using Discord.Serialization;
using System;

namespace Discord.API.Rpc
{
    internal class AuthenticateResponse
    {
        [ModelProperty("application")]
        public Application Application { get; set; }
        [ModelProperty("expires")]
        public DateTimeOffset Expires { get; set; }
        [ModelProperty("user")]
        public User User { get; set; }
        [ModelProperty("scopes")]
        public string[] Scopes { get; set; }
    }
}
