ECHO clone docs-static
git clone git@github.com:Discord-Net-Labs/docs-static.git || EXIT /B 1

ECHO remove old 'latest'
ECHO Y | RMDIR /S docs-static\latest || EXIT /B 1

ECHO build docs

TYPE D:\a\1\s\src\Discord.Net.Commands\Discord.Net.Commands.csproj || EXIT /B 1

docfx.console\tools\docfx.exe docs/docfx.json -o docs-staging || EXIT /B 1
ROBOCOPY docs-staging\_site docs-static\latest /MIR

ECHO commit and deploy
git config --global user.name "Discord.Net Labs CI Robot" && git config --global user.email "robot@discord-net-labs.com"
git -C docs-static add -A || EXIT /B 1
git -C docs-static commit -m "[ci deploy] %date% %time%: %Build.BuildId%" || EXIT /B 1
git -C docs-static push --force || EXIT /B 1
