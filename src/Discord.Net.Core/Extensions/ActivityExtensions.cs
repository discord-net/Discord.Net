using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Extension methods for the <see cref="IActivity"/> types.
    /// </summary>
    public static class ActivityExtensions
    {
        public static bool CheckFlag(this IActivity activity, ActivityFlag flag)
            => (activity.Flags & (int)flag) >= (int)flag;
    }
}
