@echo off
cls

for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /format:list') do set datetime=%%I
set PackageVersionSuffix=dev%datetime:~2,2%%datetime:~4,2%%datetime:~6,2%%datetime:~8,2%%datetime:~10,2%%datetime:~12,2%
set NugetApiKey=oy2moijtqndqlvtar6kaqdlz4q34iggufby6zxbmjxeb3u

CALL :publish_package Robot.MessageBus
CALL :publish_package Robot.Utils
CALL :publish_package Robot.Model
CALL :publish_package Robot.Drivers
CALL :publish_package Robot.Controllers



EXIT /B %ERRORLEVEL%

:publish_package
echo.
echo.
echo Publishing Package %~1...
echo.
PUSHD "..\src\%~1"
	dotnet pack %~1.csproj --version-suffix %PackageVersionSuffix%
	PUSHD ".\bin\Debug"
		for /r %%a in (*.nupkg) do dotnet nuget push %%~nxa -s https://api.nuget.org/v3/index.json -k %NugetApiKey%
		del *.nupkg
	POPD
POPD
EXIT /B 0

@echo on