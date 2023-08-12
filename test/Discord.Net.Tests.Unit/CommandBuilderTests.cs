using Discord;
using System;
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
            choices: new[]
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

    [Fact]
    public void BuildSubSlashCommand()
    {
        var command = new SlashCommandBuilder()
            .WithName("command").WithDescription("Command desc.")
            .AddOptions(new SlashCommandOptionBuilder()
                .WithType(ApplicationCommandOptionType.SubCommand)
                .WithName("subcommand").WithDescription("Subcommand desc.")
                .AddOptions(
                    new SlashCommandOptionBuilder()
                        .WithType(ApplicationCommandOptionType.String)
                        .WithName("name1").WithDescription("desc1"),
                    new SlashCommandOptionBuilder()
                        .WithType(ApplicationCommandOptionType.String)
                        .WithName("name2").WithDescription("desc2")));
        command.Build();
    }
}
