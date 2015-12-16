using System;
using System.Text;

namespace Discord.API
{
    internal static class RestRequestExtensions
    {
        public static void AddQueryParam(this IRestRequest request, StringBuilder builder, string name, string value)
        {
            if (builder.Length == 0)
                builder.Append('?');
            else
                builder.Append('&');
            builder.Append(Uri.EscapeDataString(name));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value));
        }
    }
}
