Push-Location $PSScriptRoot;

$targetPath = '..\';

If (!($True -eq $env:APPVEYOR) -Or ($True -eq $env:APPVEYOR_REPO_TAG))
{
    $targetPath = Join-Path $targetPath 'artifacts';
    $versionSuffix = $null;
}
else 
{
    $buildNumber = "build{0:D4}" -f [convert]::ToInt32($env:APPVEYOR_BUILD_NUMBER, 10);
    $commitHash = ($env:APPVEYOR_REPO_COMMIT).Substring(0, 10);
    $versionSuffix = $buildNumber + "+" + $commitHash;
}

dotnet restore ..\GitExtensions.PluginManager.sln

msbuild ..\GitExtensions.PluginManager.sln /p:Configuration=Release -property:VersionSuffix=$versionSuffix -verbosity:minimal

$packPath = Join-Path ".." $targetPath;
dotnet pack ..\src\PackageManager.UI -c Release -o $packPath --no-build /p:VersionSuffix=$versionSuffix
dotnet pack ..\src\GitExtensions.PluginManager -c Release -o $packPath --no-build /p:VersionSuffix=$versionSuffix

Copy-Item ..\src\PackageManager.UI\bin\Release\*.zip $targetPath
Copy-Item ..\src\GitExtensions.PluginManager\bin\Release\*.zip $targetPath

Pop-Location;