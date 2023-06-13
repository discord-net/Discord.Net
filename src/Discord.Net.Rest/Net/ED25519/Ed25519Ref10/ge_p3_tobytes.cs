using System;

namespace Discord.Net.ED25519.Ed25519Ref10
{
    internal static partial class GroupOperations
    {
        public static void ge_p3_tobytes(byte[] s, int offset, ref GroupElementP3 h)
        {
            FieldElement recip;
            FieldElement x, y;

            FieldOperations.fe_invert(out recip, ref h.Z);
            FieldOperations.fe_mul(out x, ref h.X, ref recip);
            FieldOperations.fe_mul(out y, ref h.Y, ref recip);
            FieldOperations.fe_tobytes(s, offset, ref y);
            s[offset + 31] ^= (byte)(FieldOperations.fe_isnegative(ref x) << 7);
        }
    }
}
