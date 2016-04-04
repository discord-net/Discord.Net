namespace Discord
{
    public class RoleUpdatedEventArgs : RoleEventArgs
    {
        public Role Before { get; }
        public Role After => Role;

        public RoleUpdatedEventArgs(Role before, Role after)
            : base(after)
        {
            Before = before;
        }
    }
}
