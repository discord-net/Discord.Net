namespace Discord;

public enum UnfurledMediaItemLoadingState
{
    /// <summary>
    ///     The state of the media item is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     The media item is currently loading.
    /// </summary>
    Loading = 1,

    /// <summary>
    ///     The media item was successfully loaded.
    /// </summary>
    LoadingSuccess = 2,

    /// <summary>
    ///     The media item was not found.
    /// </summary>
    LoadingNotFound = 3
}
