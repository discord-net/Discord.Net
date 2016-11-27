using System;
using System.Collections.Generic;
using System.Text;

namespace Discord
{
    public class ModifyMessageParams
    {
        public Optional<string> Content { get; set; }
        public Optional<EmbedBuilder> Embed { get; set; }
    }
}
