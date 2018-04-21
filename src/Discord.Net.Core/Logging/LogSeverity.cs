namespace Discord
{
    /// <summary>
    ///     Specifies the severity of the log message.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        ///     Logs that contain the most severe level of error. This type of error indicate that immediate attention
        ///     may be required.
        /// </summary>
        Critical = 0,
        /// <summary>
        ///     Logs that highlight when the flow of execution is stopped due to a failure.
        /// </summary>
        Error = 1,
        /// <summary>
        ///     Logs that highlight an abnormal activity in the flow of execution.
        /// </summary>
        Warning = 2,
        /// <summary>
        ///     Logs that track the general flow of the application.
        /// </summary>
        Info = 3,
        /// <summary>
        ///     Logs that are used for interactive investigation during development.
        /// </summary>
        Verbose = 4,
        /// <summary>
        ///     Logs that contain the most detailed messages.
        /// </summary>
        Debug = 5
    }
}
