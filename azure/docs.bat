nuget install docfx.console
git clone https://github.com/discord-net/docs-static.git
RMDIR /S docs-static/latest
docfx.console/tools/docfx.exe docs/docfx.json -o docs-static/latest/
git -C docs-static add -A
git -C docs-static commit -m "[ci deploy] %date% %time%: %Build.BuildId%"
git -C docs-static push --force
