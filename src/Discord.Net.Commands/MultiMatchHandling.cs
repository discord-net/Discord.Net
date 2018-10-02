namespace Discord.Commands
{
    /// <summary>
    /// Specifies the behavior when multiple matches are found during the command parsing stage.
    /// </summary>
    public enum MultiMatchHandling
    {
        /// <summary> Indicates that when multiple results are found, an exception should be thrown. </summary>
        Exception,
        /// <summary> Indicates that when multiple results are found, the best result should be chosen. </summary>
        Best
    }
}
