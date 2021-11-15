using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest
{
    public class BadSignatureException : Exception
    {
        internal BadSignatureException() : base("Failed to verify authenticity of message: public key doesnt match signature")
        {

        }
    }
}
