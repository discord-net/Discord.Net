namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public class ConstructorRequirements(Dictionary<string, string>? membersWhoNeedInitialization = null)
{
    public Dictionary<string, string> MembersWhoNeedInitialization { get; } = membersWhoNeedInitialization ?? new();

    public ConstructorRequirements Require(string name, string type)
    {
        if (!MembersWhoNeedInitialization.ContainsKey(name))
            MembersWhoNeedInitialization[name] = type;
        
        return this;
    }

    public static ConstructorRequirements operator +(ConstructorRequirements? a, ConstructorRequirements b)
    {
        var dict = new Dictionary<string, string>(b.MembersWhoNeedInitialization);

        if (a is not null)
        {
            foreach (var item in a.MembersWhoNeedInitialization)
            {
                if(dict.ContainsKey(item.Key)) continue;
                dict[item.Key] = item.Value;
            }
        }
        
        return new ConstructorRequirements(dict);
    }
}