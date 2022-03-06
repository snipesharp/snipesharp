#!/bin/bash

cd src
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained true

sudo mv bin/Release/net6.0/linux-x64/publish/snipesharp /usr/bin/
sudo chmod +x /usr/bin/snipesharp

sudo bash -c "echo $'[Desktop Entry]\nName=snipesharp\nGenericName=Minecraft Name Sniper\nExec=/usr/bin/snipesharp\nTerminal=true\nType=Application\nCategories=Utility;\nIcon=snipesharp\nPath=/usr/bin' > /usr/share/applications/snipesharp.desktop"

echo ""
echo "snipesharp installed in /usr/bin/snipesharp"
