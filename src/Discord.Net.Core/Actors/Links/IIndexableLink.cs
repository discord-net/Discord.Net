namespace Discord;

public interface IIndexableLink<in TId, out TActor>
{ 
    TActor this[TId id] { get; }
}