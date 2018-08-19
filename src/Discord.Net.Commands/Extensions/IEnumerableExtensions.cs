using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Commands
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TResult> Permutate<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> set,
            IEnumerable<TSecond> others,
            Func<TFirst, TSecond, TResult> func)
        {
            return from elem in set from elem2 in others select func(elem, elem2);
        }
    }
}
