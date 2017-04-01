dotnet pack "src\Discord.Net.Core\Discord.Net.Core.csproj" -c "Release" -o "../../artifacts" --no-build /p:BuildNumber="$Env:BUILD" /p:IsTagBuild="$Env:APPVEYOR_REPO_TAG"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Rest\Discord.Net.Rest.csproj" -c "Release" -o "../../artifacts" --no-build /p:BuildNumber="$Env:BUILD" /p:IsTagBuild="$Env:APPVEYOR_REPO_TAG"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.WebSocket\Discord.Net.WebSocket.csproj" -c "Release" -o "../../artifacts" --no-build /p:BuildNumber="$Env:BUILD" /p:IsTagBuild="$Env:APPVEYOR_REPO_TAG"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Rpc\Discord.Net.Rpc.csproj" -c "Release" -o "../../artifacts" --no-build /p:BuildNumber="$Env:BUILD" /p:IsTagBuild="$Env:APPVEYOR_REPO_TAG"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Commands\Discord.Net.Commands.csproj" -c "Release" -o "../../artifacts" --no-build /p:BuildNumber="$Env:BUILD" /p:IsTagBuild="$Env:APPVEYOR_REPO_TAG"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Webhook\Discord.Net.Webhook.csproj" -c "Release" -o "../../artifacts" --no-build /p:BuildNumber="$Env:BUILD" /p:IsTagBuild="$Env:APPVEYOR_REPO_TAG"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Providers.WS4Net\Discord.Net.Providers.WS4Net.csproj" -c "Release" -o "../../artifacts" --no-build /p:BuildNumber="$Env:BUILD" /p:IsTagBuild="$Env:APPVEYOR_REPO_TAG"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Providers.UdpClient\Discord.Net.Providers.UdpClient.csproj" -c "Release" -o "../../artifacts" --no-build /p:BuildNumber="$Env:BUILD" /p:IsTagBuild="$Env:APPVEYOR_REPO_TAG"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }

if ($Env:APPVEYOR_REPO_TAG -eq "true") {
    nuget pack src\Discord.Net\Discord.Net.nuspec -OutputDirectory "artifacts" -properties suffix=""
}
else {
    nuget pack src\Discord.Net\Discord.Net.nuspec -OutputDirectory "artifacts" -properties suffix="-$Env:BUILD"
}
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }