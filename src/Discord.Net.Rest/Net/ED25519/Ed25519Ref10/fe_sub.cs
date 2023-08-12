using System;

namespace Discord.Net.ED25519.Ed25519Ref10
{
    internal static partial class FieldOperations
    {
        /*
		h = f - g
		Can overlap h with f or g.

		Preconditions:
		   |f| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.
		   |g| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.

		Postconditions:
		   |h| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.
		*/

        internal static void fe_sub(out FieldElement h, ref FieldElement f, ref FieldElement g)
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

            int h0 = f0 - g0;
            int h1 = f1 - g1;
            int h2 = f2 - g2;
            int h3 = f3 - g3;
            int h4 = f4 - g4;
            int h5 = f5 - g5;
            int h6 = f6 - g6;
            int h7 = f7 - g7;
            int h8 = f8 - g8;
            int h9 = f9 - g9;

            h.x0 = h0;
            h.x1 = h1;
            h.x2 = h2;
            h.x3 = h3;
            h.x4 = h4;
            h.x5 = h5;
            h.x6 = h6;
            h.x7 = h7;
            h.x8 = h8;
            h.x9 = h9;
        }
    }
}
