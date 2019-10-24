ECHO clone docs-static
git clone git@github.com:discord-net/docs-static.git || EXIT /B 1

ECHO remove old 'latest'
ECHO Y | RMDIR /S docs-static\latest || EXIT /B 1

ECHO build docs
docfx.console\tools\docfx.exe docs/docfx.json -o docs-staging || EXIT /B 1
ROBOCOPY docs-staging\_site docs-static\latest /MIR

ECHO commit and deploy
git config --global user.name "Discord.Net CI Robot" && git config --global user.email "robot@foxbot.me"
git -C docs-static add -A || EXIT /B 1
git -C docs-static commit -m "[ci deploy] %date% %time%: %Build.BuildId%" || EXIT /B 1
git -C docs-static push --force || EXIT /B 1
