cls
@echo off
echo.
echo **************************************
echo          Building DEV Packages...

REM Go to solution dir...
PUSHD ..\
set PackageVersionSuffix=%DATE:~2,2%%DATE:~5,2%%DATE:~8,2%%TIME:~0,2%%TIME:~3,2%%TIME:~6,2%


echo **************************************
echo          Robot.MessageBus

set PackageName=Robot.MessageBus.1.0.0-%PackageVersionSuffix%.nupkg
echo Packing and Publishing %PackageName%...
REM Pack the project..
dotnet pack .\Robot.MessageBus\Robot.MessageBus.csproj --force --no-build --version-suffix %PackageVersionSuffix%
REM Publish to Github...
dotnet nuget push "Robot.MessageBus\bin\Debug\%PackageName%" --source "github"
REM Remove the local package
del /Q ".\Robot.MessageBus\bin\debug\*.nupkg"


echo **************************************
echo          Robot.Utils
set PackageName=Robot.Utils.1.0.0-%PackageVersionSuffix%.nupkg
echo Packing and Publishing %PackageName%...
REM Pack the project..
dotnet pack .\Robot.Utils\Robot.Utils.csproj --force --no-build --version-suffix %PackageVersionSuffix%
REM Publish to Github...
dotnet nuget push "Robot.Utils\bin\Debug\%PackageName%" --source "github"
REM Remove the local package
del /Q ".\Robot.Utils\bin\debug\*.nupkg"


echo **************************************
echo          Robot.Drivers
set PackageName=Robot.Drivers.1.0.0-%PackageVersionSuffix%.nupkg
echo Packing and Publishing %PackageName%...
REM Pack the project..
dotnet pack .\Robot.Drivers\Robot.Drivers.csproj --force --no-build --version-suffix %PackageVersionSuffix%
REM Publish to Github...
dotnet nuget push "Robot.Drivers\bin\Debug\%PackageName%" --source "github"
REM Remove the local package
del /Q ".\Robot.Drivers\bin\debug\*.nupkg"

echo **************************************
echo          Robot.Controllers
set PackageName=Robot.Controllers.1.0.0-%PackageVersionSuffix%.nupkg
echo Packing and Publishing %PackageName%...
REM Pack the project..
dotnet pack .\Robot.Controllers\Robot.Controllers.csproj --force --no-build --version-suffix %PackageVersionSuffix%
REM Publish to Github...
dotnet nuget push "Robot.Controllers\bin\Debug\%PackageName%" --source "github"
REM Remove the local package
del /Q ".\Robot.Controllers\bin\debug\*.nupkg"

POPD
echo *****DONE Building DEV Packages*****
echo.
@echo on
