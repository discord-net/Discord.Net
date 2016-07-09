namespace Discord.Commands
{
    public enum CommandError
    {
        //Search
        UnknownCommand,

        //Parse
        ParseFailed,
        BadArgCount,

        //Parse (Type Reader)
        CastFailed,
        ObjectNotFound,
        MultipleMatches,

        //Execute
        Exception,
    }
}
