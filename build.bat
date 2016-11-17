@echo Off
dotnet restore
dotnet build
dotnet pack "src\Discord.Net\Discord.Net.csproj" -c "%Configuration%" -o "artifacts" --version-suffix "%PrereleaseTag%" --no-build
dotnet pack "src\Discord.Net.Core\Discord.Net.Core.csproj" -c "%Configuration%" -o "artifacts" --version-suffix "%PrereleaseTag%" --no-build
dotnet pack "src\Discord.Net.Rest\Discord.Net.Rest.csproj" -c "%Configuration%" -o "artifacts" --version-suffix "%PrereleaseTag%" --no-build
dotnet pack "src\Discord.Net.WebSocket\Discord.Net.WebSocket.csproj" -c "%Configuration%" -o "artifacts" --version-suffix "%PrereleaseTag%" --no-build
dotnet pack "src\Discord.Net.Rpc\Discord.Net.Rpc.csproj" -c "%Configuration%" -o "artifacts" --version-suffix "%PrereleaseTag%" --no-build
dotnet pack "src\Discord.Net.Commands\Discord.Net.Commands.csproj" -c "%Configuration%" -o "artifacts" --version-suffix "%PrereleaseTag%" --no-build