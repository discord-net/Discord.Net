private void MyFunction(IMessage message)
{
    // Here we do the reverse as in the previous examples, and let it continue the code below if it IS an IUserMessage
    if (message is not IUserMessage userMessage)
        return;

    // Because we do the above check inline (don't give the statement a body),
    // the code will still declare `userMessage` as available outside of the above statement.
    Console.WriteLine(userMessage.Author);
}
