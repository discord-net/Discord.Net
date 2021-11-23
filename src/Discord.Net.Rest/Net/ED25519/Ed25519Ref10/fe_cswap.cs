using System;

namespace Discord.Net.ED25519.Ed25519Ref10
{
    internal static partial class FieldOperations
    {
        /*
        Replace (f,g) with (g,f) if b == 1;
        replace (f,g) with (f,g) if b == 0.

        Preconditions: b in {0,1}.
        */
        public static void fe_cswap(ref FieldElement f, ref FieldElement g, uint b)
        {
            int f0 = f.x0;
            int f1 = f.x1;
            int f2 = f.x2;
            int f3 = f.x3;
            int f4 = f.x4;
            int f5 = f.x5;
            int f6 = f.x6;
            int f7 = f.x7;
            int f8 = f.x8;
            int f9 = f.x9;
            int g0 = g.x0;
            int g1 = g.x1;
            int g2 = g.x2;
            int g3 = g.x3;
            int g4 = g.x4;
            int g5 = g.x5;
            int g6 = g.x6;
            int g7 = g.x7;
            int g8 = g.x8;
            int g9 = g.x9;
            int x0 = f0 ^ g0;
            int x1 = f1 ^ g1;
            int x2 = f2 ^ g2;
            int x3 = f3 ^ g3;
            int x4 = f4 ^ g4;
            int x5 = f5 ^ g5;
            int x6 = f6 ^ g6;
            int x7 = f7 ^ g7;
            int x8 = f8 ^ g8;
            int x9 = f9 ^ g9;

            int negb = unchecked((int)-b);
            x0 &= negb;
            x1 &= negb;
            x2 &= negb;
            x3 &= negb;
            x4 &= negb;
            x5 &= negb;
            x6 &= negb;
            x7 &= negb;
            x8 &= negb;
            x9 &= negb;
            f.x0 = f0 ^ x0;
            f.x1 = f1 ^ x1;
            f.x2 = f2 ^ x2;
            f.x3 = f3 ^ x3;
            f.x4 = f4 ^ x4;
            f.x5 = f5 ^ x5;
            f.x6 = f6 ^ x6;
            f.x7 = f7 ^ x7;
            f.x8 = f8 ^ x8;
            f.x9 = f9 ^ x9;
            g.x0 = g0 ^ x0;
            g.x1 = g1 ^ x1;
            g.x2 = g2 ^ x2;
            g.x3 = g3 ^ x3;
            g.x4 = g4 ^ x4;
            g.x5 = g5 ^ x5;
            g.x6 = g6 ^ x6;
            g.x7 = g7 ^ x7;
            g.x8 = g8 ^ x8;
            g.x9 = g9 ^ x9;
        }
    }
}
