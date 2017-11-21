dotnet build -c Release
dotnet pack -c Release -o ../../package
dotnet nuget push .\package\*.nupkg --source https://www.nuget.org/api/v2/package
