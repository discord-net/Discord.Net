public class MyCustomResult : RuntimeResult
{
    public MyCustomResult(CommandError? error, string reason) : base(error, reason)
    {
    }
}