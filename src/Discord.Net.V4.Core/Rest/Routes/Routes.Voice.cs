using Discord.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly APIRoute<VoiceRegion> ListVoiceRegions
        = new(nameof(ListVoiceRegions), RequestMethod.Get, "voice/regions");
}
