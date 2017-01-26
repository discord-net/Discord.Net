appveyor-retry dotnet restore Discord.Net.sln -v Minimal /p:BuildNumber="$Env:BUILD"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet build Discord.Net.sln -c "Release" /p:BuildNumber="$Env:BUILD"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }