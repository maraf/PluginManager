Push-Location $PSScriptRoot;

dotnet test ..\test\PackageManager.NuGet.Tests\PackageManager.NuGet.Tests.csproj -c Release --no-build --test-adapter-path:.. --logger:Appveyor /property:Platform=AnyCPU
dotnet test ..\test\PackageManager.Tests\PackageManager.Tests.csproj -c Release --no-build --test-adapter-path:.. --logger:Appveyor /property:Platform=AnyCPU

Pop-Location;