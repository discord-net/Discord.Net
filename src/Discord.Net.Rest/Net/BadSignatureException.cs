using System;

namespace Discord.Rest
{
    public class BadSignatureException : Exception
    {
        internal BadSignatureException() : base("Failed to verify authenticity of message: public key doesnt match signature")
        {

        }
    }
}
