namespace Discord.Commands
{
    public enum MultiMatchHandling
    {
        /// <summary> Indicates that when multiple results are found, an exception should be thrown. </summary>
        Exception,
        /// <summary> Indicates that when multiple results are found, the best result should be chosen. </summary>
        Best
    }
}
