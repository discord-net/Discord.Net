dotnet restore test/Discord.Net.Tests/Discord.Net.Tests.csproj
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet test test/Discord.Net.Tests/Discord.Net.Tests.csproj
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }