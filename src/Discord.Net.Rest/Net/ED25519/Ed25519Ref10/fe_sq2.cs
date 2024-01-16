using System;

namespace Discord.Net.ED25519.Ed25519Ref10
{
    internal static partial class FieldOperations
    {
        /*
h = 2 * f * f
Can overlap h with f.

Preconditions:
   |f| bounded by 1.65*2^26,1.65*2^25,1.65*2^26,1.65*2^25,etc.

Postconditions:
   |h| bounded by 1.01*2^25,1.01*2^24,1.01*2^25,1.01*2^24,etc.
*/

        /*
		See fe_mul.c for discussion of implementation strategy.
		*/
        internal static void fe_sq2(out FieldElement h, ref FieldElement f)
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

            int f0_2 = 2 * f0;
            int f1_2 = 2 * f1;
            int f2_2 = 2 * f2;
            int f3_2 = 2 * f3;
            int f4_2 = 2 * f4;
            int f5_2 = 2 * f5;
            int f6_2 = 2 * f6;
            int f7_2 = 2 * f7;
            int f5_38 = 38 * f5; /* 1.959375*2^30 */
            int f6_19 = 19 * f6; /* 1.959375*2^30 */
            int f7_38 = 38 * f7; /* 1.959375*2^30 */
            int f8_19 = 19 * f8; /* 1.959375*2^30 */
            int f9_38 = 38 * f9; /* 1.959375*2^30 */

            var f0f0 = f0 * (long)f0;
            var f0f1_2 = f0_2 * (long)f1;
            var f0f2_2 = f0_2 * (long)f2;
            var f0f3_2 = f0_2 * (long)f3;
            var f0f4_2 = f0_2 * (long)f4;
            var f0f5_2 = f0_2 * (long)f5;
            var f0f6_2 = f0_2 * (long)f6;
            var f0f7_2 = f0_2 * (long)f7;
            var f0f8_2 = f0_2 * (long)f8;
            var f0f9_2 = f0_2 * (long)f9;
            var f1f1_2 = f1_2 * (long)f1;
            var f1f2_2 = f1_2 * (long)f2;
            var f1f3_4 = f1_2 * (long)f3_2;
            var f1f4_2 = f1_2 * (long)f4;
            var f1f5_4 = f1_2 * (long)f5_2;
            var f1f6_2 = f1_2 * (long)f6;
            var f1f7_4 = f1_2 * (long)f7_2;
            var f1f8_2 = f1_2 * (long)f8;
            var f1f9_76 = f1_2 * (long)f9_38;
            var f2f2 = f2 * (long)f2;
            var f2f3_2 = f2_2 * (long)f3;
            var f2f4_2 = f2_2 * (long)f4;
            var f2f5_2 = f2_2 * (long)f5;
            var f2f6_2 = f2_2 * (long)f6;
            var f2f7_2 = f2_2 * (long)f7;
            var f2f8_38 = f2_2 * (long)f8_19;
            var f2f9_38 = f2 * (long)f9_38;
            var f3f3_2 = f3_2 * (long)f3;
            var f3f4_2 = f3_2 * (long)f4;
            var f3f5_4 = f3_2 * (long)f5_2;
            var f3f6_2 = f3_2 * (long)f6;
            var f3f7_76 = f3_2 * (long)f7_38;
            var f3f8_38 = f3_2 * (long)f8_19;
            var f3f9_76 = f3_2 * (long)f9_38;
            var f4f4 = f4 * (long)f4;
            var f4f5_2 = f4_2 * (long)f5;
            var f4f6_38 = f4_2 * (long)f6_19;
            var f4f7_38 = f4 * (long)f7_38;
            var f4f8_38 = f4_2 * (long)f8_19;
            var f4f9_38 = f4 * (long)f9_38;
            var f5f5_38 = f5 * (long)f5_38;
            var f5f6_38 = f5_2 * (long)f6_19;
            var f5f7_76 = f5_2 * (long)f7_38;
            var f5f8_38 = f5_2 * (long)f8_19;
            var f5f9_76 = f5_2 * (long)f9_38;
            var f6f6_19 = f6 * (long)f6_19;
            var f6f7_38 = f6 * (long)f7_38;
            var f6f8_38 = f6_2 * (long)f8_19;
            var f6f9_38 = f6 * (long)f9_38;
            var f7f7_38 = f7 * (long)f7_38;
            var f7f8_38 = f7_2 * (long)f8_19;
            var f7f9_76 = f7_2 * (long)f9_38;
            var f8f8_19 = f8 * (long)f8_19;
            var f8f9_38 = f8 * (long)f9_38;
            var f9f9_38 = f9 * (long)f9_38;

            var h0 = f0f0 + f1f9_76 + f2f8_38 + f3f7_76 + f4f6_38 + f5f5_38;
            var h1 = f0f1_2 + f2f9_38 + f3f8_38 + f4f7_38 + f5f6_38;
            var h2 = f0f2_2 + f1f1_2 + f3f9_76 + f4f8_38 + f5f7_76 + f6f6_19;
            var h3 = f0f3_2 + f1f2_2 + f4f9_38 + f5f8_38 + f6f7_38;
            var h4 = f0f4_2 + f1f3_4 + f2f2 + f5f9_76 + f6f8_38 + f7f7_38;
            var h5 = f0f5_2 + f1f4_2 + f2f3_2 + f6f9_38 + f7f8_38;
            var h6 = f0f6_2 + f1f5_4 + f2f4_2 + f3f3_2 + f7f9_76 + f8f8_19;
            var h7 = f0f7_2 + f1f6_2 + f2f5_2 + f3f4_2 + f8f9_38;
            var h8 = f0f8_2 + f1f7_4 + f2f6_2 + f3f5_4 + f4f4 + f9f9_38;
            var h9 = f0f9_2 + f1f8_2 + f2f7_2 + f3f6_2 + f4f5_2;

            h0 += h0;
            h1 += h1;
            h2 += h2;
            h3 += h3;
            h4 += h4;
            h5 += h5;
            h6 += h6;
            h7 += h7;
            h8 += h8;
            h9 += h9;

            var carry0 = (h0 + (1 << 25)) >> 26;
            h1 += carry0;
            h0 -= carry0 << 26;
            var carry4 = (h4 + (1 << 25)) >> 26;
            h5 += carry4;
            h4 -= carry4 << 26;
            var carry1 = (h1 + (1 << 24)) >> 25;
            h2 += carry1;
            h1 -= carry1 << 25;
            var carry5 = (h5 + (1 << 24)) >> 25;
            h6 += carry5;
            h5 -= carry5 << 25;
            var carry2 = (h2 + (1 << 25)) >> 26;
            h3 += carry2;
            h2 -= carry2 << 26;
            var carry6 = (h6 + (1 << 25)) >> 26;
            h7 += carry6;
            h6 -= carry6 << 26;
            var carry3 = (h3 + (1 << 24)) >> 25;
            h4 += carry3;
            h3 -= carry3 << 25;
            var carry7 = (h7 + (1 << 24)) >> 25;
            h8 += carry7;
            h7 -= carry7 << 25;

            carry4 = (h4 + (1 << 25)) >> 26;
            h5 += carry4;
            h4 -= carry4 << 26;

            var carry8 = (h8 + (1 << 25)) >> 26;
            h9 += carry8;
            h8 -= carry8 << 26;
            var carry9 = (h9 + (1 << 24)) >> 25;
            h0 += carry9 * 19;
            h9 -= carry9 << 25;

            carry0 = (h0 + (1 << 25)) >> 26;
            h1 += carry0;
            h0 -= carry0 << 26;

            h.x0 = (int)h0;
            h.x1 = (int)h1;
            h.x2 = (int)h2;
            h.x3 = (int)h3;
            h.x4 = (int)h4;
            h.x5 = (int)h5;
            h.x6 = (int)h6;
            h.x7 = (int)h7;
            h.x8 = (int)h8;
            h.x9 = (int)h9;
        }
    }
}
