using System;

namespace Discord.Net.ED25519.Ed25519Ref10
{
    internal static partial class GroupOperations
    {
        public static void ge_precomp_0(out GroupElementPreComp h)
        {
            FieldOperations.fe_1(out h.yplusx);
            FieldOperations.fe_1(out h.yminusx);
            FieldOperations.fe_0(out h.xy2d);
        }
    }
}
