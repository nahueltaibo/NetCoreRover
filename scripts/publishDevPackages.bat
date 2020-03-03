echo *****Building DEV Packages*****

REM Go to solution dir...
cd ..\

dotnet pack .\MessageBus\Robot.MessageBus.csproj --force --no-build


echo *****DONE Building DEV Packages*****

