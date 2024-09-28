namespace Discord.Net.Hanz.Tasks.Actors.V3;

public class LinkRelationships
{
    public static string GenerateRelationship(LinksV3.Target target)
    {
        if (target.LinkTarget.Assembly is not LinkActorTargets.AssemblyTarget.Core) return string.Empty;

        var bases = new List<string>()
        {
            $"Discord.IRelationship<{target.LinkTarget.Actor}, {target.LinkTarget.Id}, {target.LinkTarget.Entity}>"
        };

        var relationshipName = GetRelationshipName(target);

        if (target.Ancestors.Count > 0)
        {
            var oldestAncestor = target.Ancestors.First(x => x.Ancestors.Count == 0);

            relationshipName = GetRelationshipName(oldestAncestor);
        }

        var members = new List<string>()
        {
            $"{(target.Ancestors.Count > 0 ? "new " : string.Empty)}{target.LinkTarget.Actor} {relationshipName} {{ get; }}",
            $"{target.LinkTarget.Actor} {bases[0]}.RelationshipActor => {relationshipName};"
        };

        foreach (var ancestor in target.Ancestors)
        {
            bases.Add($"{ancestor.LinkTarget.Actor}.Relationship");
            members.Add(
                $"{ancestor.LinkTarget.Actor} {ancestor.LinkTarget.Actor}.Relationship.{relationshipName} => {relationshipName};"
            );
        }

        return
            $$"""
              public interface Relationship{{(
                  bases.Count > 0
                      ? $" :{Environment.NewLine}    {string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}"
                      : string.Empty
              )}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    private static string GetRelationshipName(LinksV3.Target target)
        => LinksV3.GetFriendlyName(target.LinkTarget.Actor);
}