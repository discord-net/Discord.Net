namespace Discord
{
    public enum EntityState : byte
    {
        /// <summary> Object is not attached to a cache manager nor receiving live updates. </summary>
        Detached = 0,
        /// <summary> Object is attached to a cache manager and receiving live updates. </summary>
	    Attached,
        /// <summary> Object was deleted. </summary>
        Deleted,
        /// <summary> Object is currently waiting to be created. </summary>
	    Queued,
        /// <summary> Object's creation was aborted. </summary>
        Aborted,
        /// <summary> Object's creation failed. </summary>
	    Failed
    }
}
