public async Task MessageReceivedHandler(SocketMessage msg)
{
   // Option 1:
   // Using the `as` keyword, which will return `null` if the object isn't the desired type.
   var usermsg = msg as SocketUserMessage;
   // We bail when the message isn't the desired type.
   if (msg == null) return;
   
   // Option 2:
   // Using the `is` keyword to cast (C#7 or above only)
   if (msg is SocketUserMessage usermsg) 
   {
      // Do things
   }
}