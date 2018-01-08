using System;

namespace Discord.Net
{
    public class FatalException : Exception
    {
        public FatalException(string reason, Exception inner)
            : base(reason, inner)
        {
        }
    }
}
