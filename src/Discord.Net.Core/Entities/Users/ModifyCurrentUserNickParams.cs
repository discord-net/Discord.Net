namespace Discord
{
    public class ModifyCurrentUserNickParams
    {
        public string Nickname { get; }

        public ModifyCurrentUserNickParams(string nickname)
        {
            Nickname = nickname;
        }
    }
}
