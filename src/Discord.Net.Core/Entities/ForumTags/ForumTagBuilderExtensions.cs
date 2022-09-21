namespace Discord;

public static class ForumTagBuilderExtensions
{
    public static ForumTagBuilder ToForumTagBuilder(this ForumTag tag)
        => new ForumTagBuilder(tag.Name, tag.Id, tag.IsModerated, tag.Emoji);

    public static ForumTagBuilder ToForumTagBuilder(this ForumTagProperties tag)
        => new ForumTagBuilder(tag.Name, tag.Id, tag.IsModerated, tag.Emoji);

}
