param([switch] $NoBuild)

function Build-Solution {
    $sln = ls ..\*.sln
    msbuild $sln -p:Configuration=Release
}

function Ensure-Path {
    param ($path)
    If(!(test-path $path))
    {
        New-Item -ItemType Directory -Force -Path $path | Out-Null
    }
}

function Ensure-Artifacts {
    Ensure-Path "..\artifacts";
}

function Copy-File {
    param ($sourcePath, $targetPath)
    If(test-path $sourcePath) 
    {
        Copy-Item $sourcePath -Destination $targetPath
    }
}

function Copy-PackageManager {
    $sourcePath = "..\src\PackageManager.UI\bin\Release"
    $targetPath = "..\artifacts\PackageManager"

    Ensure-Path $targetPath;

    Copy-Item ($sourcePath + "\Neptuo*.dll") -Destination $targetPath
    Copy-File ($sourcePath + "\Newtonsoft.Json.dll") $targetPath
    Copy-Item ($sourcePath + "\NuGet*.dll") -Destination $targetPath
    Copy-Item ($sourcePath + "\PackageManager*.dll") -Destination $targetPath
    Copy-Item ($sourcePath + "\PackageManager*.exe") -Destination $targetPath
    Copy-File ($sourcePath + "\System.Net.Http.dll") $targetPath

    return $targetPath;
}

function Copy-PluginManager {
    $sourcePath = "..\src\GitExtensions.PluginManager\bin\Release"
    $targetPath = "..\artifacts\GitExtensions.PluginManager"
    
    $soourcePackageManagerPath = Copy-PackageManager
    $targetPackageManagerPath = $targetPath + "\PluginManager";

    Ensure-Path $targetPath;
    Ensure-Path $targetPackageManagerPath;

    Copy-Item ($sourcePath + "\net461\Neptuo*.dll") -Destination $targetPath
    Copy-Item ($sourcePath + "\net461\GitExtensions.PluginManager*.dll") -Destination $targetPath
    Copy-Item ($soourcePackageManagerPath + "\*") -Destination $targetPackageManagerPath
}

if (!$NoBuild) {
    Build-Solution
}

Copy-PackageManager | Out-Null
Copy-PluginManager | Out-Null