namespace Discord.Gateway;

[Flags]
public enum EventParameterDegree : byte
{
    Id = 1 << 0,
    Model = 1 << 1,
    Actor = 1 << 2,
    Handle = 1 << 3,
    Entity = 1 << 4,

    All = Id | Model | Actor | Handle | Entity
}
