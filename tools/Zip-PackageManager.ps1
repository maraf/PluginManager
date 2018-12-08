param([string] $Version, [string] $Configuration = 'Release')

If (-not($Version)) { 
    Throw "Parameter -Version is required";
}

Push-Location $PSScriptRoot;

$pmSource = "..\src\PackageManager.UI\bin\" + $Configuration + "\net461\PackageManager.UI.exe";
$pmTarget = "..\src\PackageManager.UI\bin\" + $Configuration + "\PackageManager-" + $Version + ".zip";
Compress-Archive -Path $pmSource -DestinationPath $pmTarget -Force;

Pop-Location;