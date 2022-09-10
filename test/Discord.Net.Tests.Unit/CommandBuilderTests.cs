using System;
using Discord;
using Xunit;

namespace Discord;

public class CommandBuilderTests
{
    [Fact]
    public void BuildSimpleSlashCommand()
    {
        var command = new SlashCommandBuilder()
        .WithName("command")
        .WithDescription("description")
        .AddOption(
            "option1",
            ApplicationCommandOptionType.String,
            "option1 description",
            isRequired: true,
            choices: new [] 
            {
                new ApplicationCommandOptionChoiceProperties()
                {
                    Name = "choice1", Value = "1"
                }
            })
        .AddOptions(new SlashCommandOptionBuilder()
            .WithName("option2")
            .WithDescription("option2 description")
            .WithType(ApplicationCommandOptionType.String)
            .WithRequired(true)
            .AddChannelType(ChannelType.Text)
            .AddChoice("choice1", "1")
            .AddChoice("choice2", "2"));
        command.Build();
    }
}
