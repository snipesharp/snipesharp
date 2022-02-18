#!/bin/bash

cd src
dotnet publish -c Release -r linux-arm64 -p:PublishSingleFile=true --self-contained true

echo ""
echo ""
echo "Run the following commands to finish the installation"
echo ""
echo "sudo mv src/bin/Release/net6.0/linux-arm64/publish/snipesharp /usr/bin/"
echo "sudo chmod +x /usr/bin/snipesharp"

echo "printf \"[Desktop Entry]\nName=snipesharp\nGenericName=Minecraft Name Sniper\nExec=/usr/bin/snipesharp\nTerminal=true\nType=Application\nCategories=Utility;\nIcon=snipesharp\nPath=/usr/bin\" > /usr/share/applications/snipesharp.desktop"
