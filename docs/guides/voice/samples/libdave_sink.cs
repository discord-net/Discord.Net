Discord.LibDave.Dave.SetLogSink(MyLogSink);

void MyLogSink(
    Discord.LibDave.LoggingSeverity severity, 
    string filePath,
    int lineNumber,
    string message
)
{
    // do what you want with the logs
    Console.WriteLine($"[{severity} | LIBDAVE @ {file}#{line}]: {message}");
}