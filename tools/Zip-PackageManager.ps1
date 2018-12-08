param([string] $Version, [string] $Configuration = 'Release')

If (-not($Version)) 
{ 
    Throw "Parameter -Version is required";
}

Push-Location $PSScriptRoot;

$source = "..\src\PackageManager.UI\bin\" + $Configuration + "\net461\PackageManager.UI.exe";
$target = "..\src\PackageManager.UI\bin\" + $Configuration + "\PackageManager." + $Version + ".zip";

Compress-Archive -Path $source -DestinationPath $target -Force;
Write-Host ("Created release zip at '" + $target + "'");

Pop-Location;