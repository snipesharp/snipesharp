#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as sudo, \"sudo ./install_linux.sh\""
  exit
fi
cd src
dotnet publish -c Release -r linux-arm64 -p:PublishSingleFile=true --self-contained true

mv bin/Release/net6.0/linux-arm64/publish/snipesharp /usr/bin/
chmod +x /usr/bin/snipesharp

printf "[Desktop Entry]\nName=snipesharp\nGenericName=Minecraft Name Sniper\nExec=/usr/bin/snipesharp\nTerminal=true\nType=Application\nCategories=Utility;\nIcon=snipesharp\nPath=/usr/bin" > /usr/share/applications/snipesharp.desktop

echo ""
echo "snipesharp installed in /usr/bin/snipesharp"
