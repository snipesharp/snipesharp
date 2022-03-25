#!/bin/bash

cd src
dotnet publish -c Release -r linux-arm64 -p:PublishSingleFile=true --self-contained true

mv bin/Release/net6.0/linux-arm64/publish/snipesharp /home/demented/repos/snipesharp-repos/website/devbuild/snipesharp_arm64

echo ""
echo "snipesharp installed in /home/demented/repos/snipesharp-repos/website/devbuild/snipesharp_arm64"
