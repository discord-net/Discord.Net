namespace Discord
{
    public interface IRelationship
    {
        RelationshipType Type { get; }

        IUser User { get; }
    }
}
