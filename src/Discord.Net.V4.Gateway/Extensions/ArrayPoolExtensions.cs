using System;
using System.Buffers;

namespace Discord.Gateway
{
    internal static class ArrayPoolExtensions
    {
        public static RentedArray<T> RentHandle<T>(this ArrayPool<T> pool, int size)
        {
            return new RentedArray<T>(
                pool.Rent(size),
                pool
            );
        }

    }
}

