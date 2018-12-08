param([string] $Version, [string] $Configuration = 'Release')

If (-not($Version)) { 
    Throw "Parameter -Version is required";
}

Push-Location $PSScriptRoot;

$sourceBasePath = "..\src\GitExtensions.PluginManager\bin\" + $Configuration;

$name = "GitExtensions.PluginManager-" + $Version;
$target = $sourceBasePath + "\GitExtensions.PluginManager-" + $Version + ".zip";

$tempPath = Join-Path $env:TEMP -ChildPath $name;
$tempPmPath = Join-Path $tempPath -ChildPath "PackageMananger";
New-Item -Force -ItemType Directory $tempPath | Out-Null;
New-Item -Force -ItemType Directory $tempPmPath | Out-Null;

Copy-Item -Force ($sourceBasePath + "\net461\GitExtensions.PluginManager.dll") $tempPath | Out-Null;
Copy-Item -Force ($sourceBasePath + "\net461\PackageManager\PackageManager.UI.exe") $tempPmPath | Out-Null;

Compress-Archive -Path ($tempPath + "\*") -DestinationPath $target -Force;

Remove-Item -Force -Recurse $tempPath;

Pop-Location;