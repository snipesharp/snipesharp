cd src
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
explorer.exe "bin\Release\net6.0\win-x64\publish"