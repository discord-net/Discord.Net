namespace Discord
{
    /// <summary> The severity of a log message </summary>
    public enum LogSeverity
    {
        /// <summary> Used when a critical, non-recoverable error occurs </summary>
        Critical = 0,
        /// <summary> Used when a recoverable error occurs </summary>
        Error = 1,
        /// <summary> Used when a warning occurs </summary>
        Warning = 2,
        /// <summary> Used for general, informative messages </summary>
        Info = 3,
        /// <summary> Used for debugging purposes </summary> 
        Verbose = 4,
        /// <summary> Used for debugging purposes </summary>
        Debug = 5
    }
}
