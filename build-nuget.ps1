dotnet build -c Release
dotnet pack -c Release -o ../../package
dotnet nuget push .\package\*.nupkg -Source https://www.nuget.org/api/v2/package
