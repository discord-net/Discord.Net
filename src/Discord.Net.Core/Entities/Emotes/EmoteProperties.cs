using System.Collections.Generic;

namespace Discord
{
    public class EmoteProperties
    {
        public Optional<string> Name { get; set; }
        public Optional<IEnumerable<IRole>> Roles { get; set; }
    }
}
