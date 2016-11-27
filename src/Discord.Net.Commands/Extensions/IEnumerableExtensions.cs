using System;
using System.Collections.Generic;

namespace Discord.Commands
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TResult> Permutate<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> set,
            IEnumerable<TSecond> others,
            Func<TFirst, TSecond, TResult> func)
        {
            foreach (TFirst elem in set)
            {
                foreach (TSecond elem2 in others)
                {
                    yield return func(elem, elem2);
                }
            }
        }
    }
}