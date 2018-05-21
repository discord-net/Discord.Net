//	Input: 
//		!echo Coffee Cake

//	Output:
//		Coffee Cake
[Command("echo")]
public Task EchoRemainderAsync([Remainder]string text) => ReplyAsync(text);  

//	Output:
//		CommandError.BadArgCount
[Command("echo-hassle")]
public Task EchoAsync(string text) => ReplyAsync(text);

//	The message would be seen as having multiple parameters, 
//	while the method only accepts one. 
//	Wrapping the message in quotes solves this.
//	This way, the system knows the entire message is to be parsed as a 
//	single String.
//	e.g., 
//		!echo "Coffee Cake"