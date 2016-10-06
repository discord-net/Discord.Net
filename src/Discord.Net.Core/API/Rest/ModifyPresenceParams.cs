#pragma warning disable CS1591
using System;

namespace Discord.API.Rest
{
    public class ModifyPresenceParams
    {
        public Optional<UserStatus> Status { get; set; }
        public Optional<Game> Game { get; set; }
    }
}
