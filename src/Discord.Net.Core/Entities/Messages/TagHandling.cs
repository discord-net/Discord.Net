namespace Discord
{
    public enum TagHandling
    {
        Ignore = 0,         //<@53905483156684800> -> <@53905483156684800>
        Remove,             //<@53905483156684800> -> 
        Name,               //<@53905483156684800> -> @Voltana
        NameNoPrefix,       //<@53905483156684800> -> Voltana
        FullName,           //<@53905483156684800> -> @Voltana#8252
        FullNameNoPrefix,   //<@53905483156684800> -> Voltana#8252
        Sanitize            //<@53905483156684800> -> <@53905483156684800> (w/ nbsp)
    }
}
