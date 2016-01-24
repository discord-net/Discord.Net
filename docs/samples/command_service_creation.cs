//create command service
var commandService = new CommandService(new CommandServiceConfig
{
    CommandChar = '~', // prefix char for commands
    HelpMode = HelpMode.Public
});

//add command service
var commands = client.AddService(commandService);