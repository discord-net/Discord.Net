using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public enum EditMode : byte
	{
		Set,
		Add,
		Remove
	}

	internal static class Extensions
	{
		public static IEnumerable<T>  Modify<T>(this IEnumerable<T> original, IEnumerable<T> modified, EditMode mode)
		{
			if (original == null) return null;
			switch (mode)
			{
				case EditMode.Set:
				default:
					return modified;
				case EditMode.Add:
					return original.Concat(modified);
				case EditMode.Remove:
					return original.Except(modified);
			}
        }
    }
}
