cd src
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
mkdir "%AppData%\.snipesharp\"
move /Y bin\Release\net6.0\win-x64\publish\snipesharp.exe %AppData%\.snipesharp\

set SCRIPT="%TEMP%\%RANDOM%-%RANDOM%-%RANDOM%-%RANDOM%.vbs"

echo Set oWS = WScript.CreateObject("WScript.Shell") >> %SCRIPT%
echo sLinkFile = "%USERPROFILE%\Desktop\snipesharp.lnk" >> %SCRIPT%
echo Set oLink = oWS.CreateShortcut(sLinkFile) >> %SCRIPT%
echo oLink.TargetPath = "%AppData%\.snipesharp\snipesharp.exe" >> %SCRIPT%
echo oLink.Save >> %SCRIPT%

cscript /nologo %SCRIPT%
del %SCRIPT%