namespace Discord
{
    internal static class RoleUtils
    {
        public static int Compare(IRole left, IRole right)
        {
            if (left == null)
                return -1;
            if (right == null)
                return 1;
            var result = left.Position.CompareTo(right.Position);
            // As per Discord's documentation, a tie is broken by ID
            if (result != 0)
                return result;
            return left.Id.CompareTo(right.Id);
        }
    }
}
