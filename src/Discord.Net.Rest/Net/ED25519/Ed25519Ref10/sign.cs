using System;

namespace Discord.Net.ED25519.Ed25519Ref10
{
    internal static partial class Ed25519Operations
    {
        public static void crypto_sign(
            byte[] sig, int sigoffset,
            byte[] m, int moffset, int mlen,
            byte[] sk, int skoffset)
        {
            byte[] az, r, hram;
            GroupElementP3 R;
            var hasher = new Sha512();
            {
                hasher.Update(sk, skoffset, 32);
                az = hasher.Finalize();
                ScalarOperations.sc_clamp(az, 0);

                hasher.Init();
                hasher.Update(az, 32, 32);
                hasher.Update(m, moffset, mlen);
                r = hasher.Finalize();

                ScalarOperations.sc_reduce(r);
                GroupOperations.ge_scalarmult_base(out R, r, 0);
                GroupOperations.ge_p3_tobytes(sig, sigoffset, ref R);

                hasher.Init();
                hasher.Update(sig, sigoffset, 32);
                hasher.Update(sk, skoffset + 32, 32);
                hasher.Update(m, moffset, mlen);
                hram = hasher.Finalize();

                ScalarOperations.sc_reduce(hram);
                var s = new byte[32];//todo: remove allocation
                Array.Copy(sig, sigoffset + 32, s, 0, 32);
                ScalarOperations.sc_muladd(s, hram, az, r);
                Array.Copy(s, 0, sig, sigoffset + 32, 32);
                CryptoBytes.Wipe(s);
            }
        }
    }
}
