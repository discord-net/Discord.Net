_client.Log += (msg) =>
{
    Console.WriteLine(msg.ToString());
    return Task.CompletedTask;
}