namespace Discord;

public interface IApplicationsLink :
    ApplicationLink.Indexable
{
    ICurrentApplicationActor Current { get; }
}