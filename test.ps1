dotnet test test/Discord.Net.Tests/Discord.Net.Tests.csproj -c "Release" --noBuild /p:BuildNumber="$Env:BUILD"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }