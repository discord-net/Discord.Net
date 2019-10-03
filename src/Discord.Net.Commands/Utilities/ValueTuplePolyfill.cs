#if !NetStandard20

namespace System
{
    internal struct ValueTuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
    }
}

namespace System.Runtime.CompilerServices
{
    using System.Collections.Generic;
    internal class TupleElementNamesAttribute : Attribute
    {
        public IList<string> TransformNames { get; }
        public TupleElementNamesAttribute(string[] transformNames)
        {
            TransformNames = transformNames;
        }
    }
}
#endif
