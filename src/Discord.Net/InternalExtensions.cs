using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Discord
{
    internal static class InternalExtensions
    {
        internal static readonly IFormatProvider _format = CultureInfo.InvariantCulture;

        public static ulong ToId(this string value) => ulong.Parse(value, NumberStyles.None, _format);
        public static ulong? ToNullableId(this string value) => value == null ? (ulong?)null : ulong.Parse(value, NumberStyles.None, _format);
        public static bool TryToId(this string value, out ulong result) => ulong.TryParse(value, NumberStyles.None, _format, out result);

        public static string ToIdString(this ulong value) => value.ToString(_format);
        public static string ToIdString(this ulong? value) => value?.ToString(_format);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasBit(this uint rawValue, byte bit) => ((rawValue >> bit) & 1U) == 1;

        public static bool TryGetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> d,
            TKey key, Func<TKey, TValue> factory, out TValue result)
        {
            bool created = false;
            TValue newValue = default(TValue);
            while (true)
            {
                if (d.TryGetValue(key, out result))
                    return false;
                if (!created)
                {
                    newValue = factory(key);
                    created = true;
                }
                if (d.TryAdd(key, newValue))
                {
                    result = newValue;
                    return true;
                }
            }
        }
        public static bool TryGetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> d,
            TKey key, TValue value, out TValue result)
        {
            while (true)
            {
                if (d.TryGetValue(key, out result))
                    return false;
                if (d.TryAdd(key, value))
                {
                    result = value;
                    return true;
                }
            }
        }

        public static IEnumerable<Channel> Find(this IEnumerable<Channel> channels, string name, ChannelType type = null, bool exactMatch = false)
        {
            //Search by name
            var query = channels
                .Where(x => string.Equals(x.Name, name, exactMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));

            if (!exactMatch)
            {
                if (name.Length >= 2 && name[0] == '<' && name[1] == '#' && name[name.Length - 1] == '>') //Search by raw mention
                {
                    ulong id;
                    if (name.Substring(2, name.Length - 3).TryToId(out id))
                    {
                        var channel = channels.Where(x => x.Id == id).FirstOrDefault();
                        if (channel != null)
                            query = query.Concat(new Channel[] { channel });
                    }
                }
                if (name.Length >= 1 && name[0] == '#' && (type == null || type == ChannelType.Text)) //Search by clean mention
                {
                    string name2 = name.Substring(1);
                    query = query.Concat(channels
                        .Where(x => x.Type == ChannelType.Text && string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase)));
                }
            }

            if (type != null)
                query = query.Where(x => x.Type == type);
            return query;
        }

        public static IEnumerable<User> Find(this IEnumerable<User> users,
            string name, ushort? discriminator = null, bool exactMatch = false)
        {
            //Search by name
            var query = users
                .Where(x => string.Equals(x.Name, name, exactMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));

            if (!exactMatch)
            {
                if (name.Length >= 3 && name[0] == '<' && name[1] == '@' && name[2] == '!' && name[name.Length - 1] == '>') //Search by nickname'd mention
                {
                    ulong id;
                    if (name.Substring(3, name.Length - 4).TryToId(out id))
                    {
                        var user = users.Where(x => x.Id == id).FirstOrDefault();
                        if (user != null)
                            query = query.Concat(new User[] { user });
                    }
                }
                if (name.Length >= 2 && name[0] == '<' && name[1] == '@' && name[name.Length - 1] == '>') //Search by raw mention
                {
                    ulong id;
                    if (name.Substring(2, name.Length - 3).TryToId(out id))
                    {
                        var user = users.Where(x => x.Id == id).FirstOrDefault();
                        if (user != null)
                            query = query.Concat(new User[] { user });
                    }
                }
                if (name.Length >= 1 && name[0] == '@') //Search by clean mention
                {
                    string name2 = name.Substring(1);
                    query = query.Concat(users
                        .Where(x => string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase)));
                }
            }

            if (discriminator != null)
                query = query.Where(x => x.Discriminator == discriminator.Value);
            return query;
        }

        public static IEnumerable<Role> Find(this IEnumerable<Role> roles, string name, bool exactMatch = false)
        {
            // if (name.StartsWith("@"))
            // {
            // 	string name2 = name.Substring(1);
            // 	return _roles.Where(x => x.Server.Id == server.Id &&
            // 		string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) || 
            // 		string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
            // }
            // else
            return roles.Where(x => string.Equals(x.Name, name, exactMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<Server> Find(this IEnumerable<Server> servers, string name, bool exactMatch = false)
            => servers.Where(x => string.Equals(x.Name, name, exactMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));

        public static string Base64(this Stream stream, ImageType type, string existingId)
        {
            if (type == ImageType.None)
                return null;
            else if (stream != null)
            {
                byte[] bytes = new byte[stream.Length - stream.Position];
                stream.Read(bytes, 0, bytes.Length);

                string base64 = Convert.ToBase64String(bytes);
                string imageType = type == ImageType.Jpeg ? "image/jpeg;base64" : "image/png;base64";
                return $"data:{imageType},{base64}";
            }
            return existingId;
        }
    }
}
