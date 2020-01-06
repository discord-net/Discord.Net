using System;
using System.Collections.Generic;
using System.Text;
// todo: impl
namespace Discord
{
    public struct Optional<T>
    {
        public bool IsSpecified { get; private set; }
        public T Value { get; set; }
    }
}
