using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
  public static class RoleExtensions
  {
    internal static int Compare(this IRole r1, IRole r2) {
      if(r2 == null)
        return 1;
      var result = r1.Position.CompareTo(r2.Position);
      // As per Discord's documentation, a tie is broken by ID
      if(result != 0)
        return result;
      return r1.Id.CompareTo(r2.Id);
    }
  }
}
