using System;

namespace Discord.Serialization
{
    public class SerializationException : Exception
    {
        public SerializationException()
            : base("Serialization failed")
        {
        }
        public SerializationException(string message) 
            : base(message)
        {
        }
    }
}
