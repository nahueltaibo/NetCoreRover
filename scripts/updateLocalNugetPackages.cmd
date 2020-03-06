cls
@echo off

set PackagesPath=c:\Dev\Packages

for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /format:list') do set datetime=%%I
set PackageVersionSuffix=prerelease-%datetime:~2,2%%datetime:~4,2%%datetime:~6,2%%datetime:~8,2%%datetime:~10,2%%datetime:~12,2%

PUSHD ..\src\
	set Name=Robot.MessageBus
	echo Packing %Name%
	PUSHD %Name%
		dotnet pack "%Name%.csproj" --force --no-build --version-suffix %PackageVersionSuffix%
		nuget init .\bin\debug\ %PackagesPath%
		del .\bin\debug\*.nupkg
	POPD

	set Name=Robot.Utils
	echo Packing %Name%
	PUSHD %Name%
		dotnet pack "%Name%.csproj" --force --no-build --version-suffix %PackageVersionSuffix%
		nuget init .\bin\debug\ %PackagesPath%
		del .\bin\debug\*.nupkg
	POPD

	set Name=Robot.Drivers
	echo Packing %Name%
	PUSHD %Name%
		dotnet pack "%Name%.csproj" --force --no-build --version-suffix %PackageVersionSuffix%
		nuget init .\bin\debug\ %PackagesPath%
		del .\bin\debug\*.nupkg
	POPD

	set Name=Robot.Controllers
	echo Packing %Name%
	PUSHD %Name%
		dotnet pack "%Name%.csproj" --force --no-build --version-suffix %PackageVersionSuffix%
		nuget init .\bin\debug\ %PackagesPath%
		del .\bin\debug\*.nupkg
	POPD

	set Name=Robot.Model
	echo Packing %Name%
	PUSHD %Name%
		dotnet pack "%Name%.csproj" --force --no-build --version-suffix %PackageVersionSuffix%
		nuget init .\bin\debug\ %PackagesPath%
		del .\bin\debug\*.nupkg
	POPD
POPD
@echo on