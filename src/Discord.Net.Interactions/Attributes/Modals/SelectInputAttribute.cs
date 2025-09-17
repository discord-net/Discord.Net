namespace Discord.Interactions.Attributes.Modals;

public abstract class SelectInputAttribute : ModalInputAttribute
{
    public int MinValues { get; set; } = 1;

    public int MaxValues { get; set; } = 1;

    public string Placeholder { get; set; }

    public SelectInputAttribute(string customId) : base(customId)
    {
    }
}
