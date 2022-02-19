#!/bin/bash

cd src
dotnet publish -c Release -r linux-arm64 -p:PublishSingleFile=true --self-contained true
sudo mv bin/Release/net6.0/linux-arm64/publish/snipesharp /usr/bin/
sudo chmod +x /usr/bin/snipesharp

printf "[Desktop Entry]\nName=snipesharp\nGenericName=Minecraft Name Sniper\nExec=/usr/bin/snipesharp\nTerminal=true\nType=Application\nCategories=Utility;\nIcon=snipesharp\nPath=/usr/bin" > /usr/share/applications/snipesharp.desktop"