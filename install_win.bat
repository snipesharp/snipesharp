@echo off
cd src

echo Compiling...
@echo.
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
@echo.
echo Making folder "%AppData%\.snipesharp\"
mkdir %AppData%\.snipesharp\
@echo.
echo Moving snipesharp.exe to "%AppData%\.snipesharp\"
move /Y bin\Release\net6.0\win-x64\publish\snipesharp.exe %AppData%\.snipesharp\
@echo.
echo Creating desktop shortcut for snipesharp.exe
set SCRIPT="%TEMP%\%RANDOM%-%RANDOM%-%RANDOM%-%RANDOM%.vbs"

echo Set oWS = WScript.CreateObject("WScript.Shell") >> %SCRIPT%
echo sLinkFile = "%USERPROFILE%\Desktop\snipesharp.lnk" >> %SCRIPT%
echo Set oLink = oWS.CreateShortcut(sLinkFile) >> %SCRIPT%
echo oLink.TargetPath = "%AppData%\.snipesharp\snipesharp.exe" >> %SCRIPT%
echo oLink.WorkingDirectory = "%AppData%\.snipesharp\" >> %SCRIPT%
echo oLink.Save >> %SCRIPT%

cscript /nologo %SCRIPT%
del %SCRIPT%
@echo.
pause