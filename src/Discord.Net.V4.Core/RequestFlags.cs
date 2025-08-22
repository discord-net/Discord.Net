namespace Discord.Models;

[Flags]
public enum RequestFlags
{
    AllowFetch = 1 << 0,
    AllowCache = 1 << 1,
    
    CacheResult = 1 << 2,
    
    Default = AllowFetch | AllowCache
}