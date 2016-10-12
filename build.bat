@echo Off
dotnet restore
dotnet pack "src\Discord.Net" -c "%Configuration%" -o "artifacts"
dotnet pack "src\Discord.Net.Core" -c "%Configuration%" -o "artifacts"
dotnet pack "src\Discord.Net.Rest" -c "%Configuration%" -o "artifacts"
dotnet pack "src\Discord.Net.WebSocket" -c "%Configuration%" -o "artifacts"
dotnet pack "src\Discord.Net.Rpc" -c "%Configuration%" -o "artifacts"
dotnet pack "src\Discord.Net.Commands" -c "%Configuration%" -o "artifacts"