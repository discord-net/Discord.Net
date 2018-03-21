namespace Discord.Commands
{
    public interface IResult
    {
        /// <summary> Describes the error type that may have occurred during the operation. </summary>
        CommandError? Error { get; }
        /// <summary> Describes the reason for the error. </summary>
        string ErrorReason { get; }
        /// <summary> Indicates whether the operation was successful or not. </summary>
        bool IsSuccess { get; }
    }
}
