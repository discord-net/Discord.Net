git clone https://github.com/discord-net/docs-static.git || EXIT /B 1
RMDIR /S docs-static\latest || EXIT /B 1
docfx.console/tools/docfx.exe docs/docfx.json -o docs-static/latest/ || EXIT /B 1
git config --global user.name "Discord.Net CI Robot" && git config --global user.email "robot@foxbot.me"
git -C docs-static add -A || EXIT /B 1
git -C docs-static commit -m "[ci deploy] %date% %time%: %Build.BuildId%" || EXIT /B 1
git -C docs-static push --force || EXIT /B 1
