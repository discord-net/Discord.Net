//we would run our commands with ~do greet X and ~do bye X
commands.CreateGroup("do", cgb =>
        {
            cgb.CreateCommand("greet")
                    .Alias(new string[] { "gr", "hi" })
                    .Description("Greets a person.")
                    .Parameter("GreetedPerson", ParameterType.Required)
                    .Do(async e =>
                    {
                        await client.SendMessage(e.Channel, e.User.Name + " greets " + e.GetArg("GreetedPerson"));
                    });

            cgb.CreateCommand("bye")
                    .Alias(new string[] { "bb", "gb" })
                    .Description("Greets a person.")
                    .Parameter("GreetedPerson", ParameterType.Required)
                    .Do(async e =>
                    {
                        await client.SendMessage(e.Channel, e.User.Name + " says goodbye to " + e.GetArg("GreetedPerson"));
                    });
        });