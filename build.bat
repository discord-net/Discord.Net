@echo Off
dotnet restore
dotnet pack "src\Discord.Net" -c "%Configuration%" -o "artifacts"
dotnet pack "src\Discord.Net.Commands" -c "%Configuration%" -o "artifacts"