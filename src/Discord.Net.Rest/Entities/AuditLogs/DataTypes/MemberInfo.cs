namespace Discord.Rest
{
    public struct MemberInfo
    {
        internal MemberInfo(string nick, bool? deaf, bool? mute, string avatar_hash)
        {
            Nickname = nick;
            Deaf = deaf;
            Mute = mute;
            AvatarHash = avatar_hash;
        }

        public string Nickname { get; }
        public bool? Deaf { get; }
        public bool? Mute { get; }
        public string AvatarHash { get; }
    }
}
