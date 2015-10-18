using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
    internal static class CollectionHelper
    {
		public static IEnumerable<string> FlattenChannels(IEnumerable<object> channels)
		{
			if (channels == null)
				return new string[0];

			return channels.Select(x =>
			{
				if (x is string)
					return x as string;
				else if (x is Channel)
					return (x as Channel).Id;
				else
					throw new ArgumentException("Collection may only contain string or Channel.", nameof(channels));
			});
		}
		public static IEnumerable<string> FlattenUsers(IEnumerable<object> users)
		{
			if (users == null)
				return new string[0];

			return users.Select(x =>
			{
				if (x is string)
					return x as string;
				else if (x is User)
					return (x as User).Id;
				else
					throw new ArgumentException("Collection may only contain string or User.", nameof(users));
			});
		}
		public static IEnumerable<string> FlattenRoles(IEnumerable<object> roles)
		{
			if (roles == null)
				return new string[0];

			return roles.Select(x =>
			{
				if (x is string)
					return x as string;
				else if (x is Role)
					return (x as Role).Id;
				else
					throw new ArgumentException("Collection may only contain string or Role.", nameof(roles));
			});
		}
	}
}
