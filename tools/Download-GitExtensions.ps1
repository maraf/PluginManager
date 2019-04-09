Set-Location $PSScriptRoot

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$ExtractRootPath = '../references'
$AssetToDownloadName = 'GitExtensions-Portable-3.0.2.5232.zip';
$AssetToDownloadUrl = 'https://github.com/gitextensions/gitextensions/releases/download/v3.0.2/GitExtensions-Portable-3.0.2.5232.zip';

if (!($null -eq $AssetToDownloadUrl))
{
    $DownloadName = [System.IO.Path]::GetFileName($AssetToDownloadName);
    $DownloadFilePath = [System.IO.Path]::Combine($ExtractRootPath, $DownloadName);
    $ExtractPath = $ExtractRootPath;

    if (!(Test-Path $DownloadFilePath))
    {
        if (!(Test-Path $ExtractRootPath))
        {
            New-Item -ItemType directory -Path $ExtractRootPath | Out-Null;
        }

        if (!(Test-Path $ExtractPath))
        {
            New-Item -ItemType directory -Path $ExtractPath | Out-Null;
        }

        Write-Host ('Downloading "' + $DownloadName + '".');

        Invoke-WebRequest -Uri $AssetToDownloadUrl -OutFile $DownloadFilePath;
        Expand-Archive $DownloadFilePath -DestinationPath $ExtractPath -Force
    }
    else 
    {
        Write-Host ('Download "' + $DownloadName + '" already exists.');
    }
}

Pop-Location