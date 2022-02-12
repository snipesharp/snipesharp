#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as sudo, \"sudo ./install_linux.sh\""
  exit
fi
cd src
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained true

mv bin/Release/net6.0/linux-x64/publish/snipesharp /usr/bin/
chmod +x /usr/bin/snipesharp

echo ""
echo "snipesharp installed in /usr/bin/snipesharp"
