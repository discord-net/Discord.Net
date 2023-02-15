using System;

namespace Discord.Net.ED25519.Ed25519Ref10
{
    internal static partial class FieldOperations
    {
        /*
		Replace (f,g) with (g,g) if b == 1;
		replace (f,g) with (f,g) if b == 0.

		Preconditions: b in {0,1}.
		*/

        //void fe_cmov(fe f,const fe g,unsigned int b)
        internal static void fe_cmov(ref FieldElement f, ref FieldElement g, int b)
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

            b = -b;
            x0 &= b;
            x1 &= b;
            x2 &= b;
            x3 &= b;
            x4 &= b;
            x5 &= b;
            x6 &= b;
            x7 &= b;
            x8 &= b;
            x9 &= b;
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
        }
    }
}
