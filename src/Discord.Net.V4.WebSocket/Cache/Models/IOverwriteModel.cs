using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IOverwriteModel
    {
        ulong Target { get; }
        PermissionTarget Type { get; }
        ulong Allow { get; }
        ulong Deny { get; }
    }
}
