param([string] $Version, [string] $Source = "github")

if (-not($Version)) 
{ 
    Throw "Parameter -Version is required";
}

if (-not($Source -eq "github") -and -not($Source -eq "appveyor"))
{
	Throw "Parameter -Source must be either 'github' or 'appveyor'";
}

Set-Location $PSScriptRoot

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$ExtractRootPath = '../references'
$AssetToDownloadName = 'GitExtensions-' + $Version + '.zip';
$AssetToDownloadUrl = $null;

$DownloadName = [System.IO.Path]::GetFileName($AssetToDownloadName);
$DownloadFilePath = [System.IO.Path]::Combine($ExtractRootPath, $DownloadName);
if (Test-Path $DownloadFilePath)
{
	Write-Host ('Download "' + $DownloadName + '" already exists.');
	Pop-Location
	exit;
}

Write-Host ('Searching for GitExtensions release on "' + $Source + '".');

if ($Source -eq "github")
{
	# Find release and asset.
	$Releases = Invoke-RestMethod -Uri 'https://api.github.com/repos/gitextensions/gitextensions/releases'
	foreach ($Release in $Releases)
	{
		if ($Release.tag_name -eq $Version)
		{
			Write-Host ('Selected release "' + $Release.name + '".');
			foreach ($Asset in $Release.assets)
			{
				if ($Asset.content_type -eq "application/zip" -and $Asset.name.Contains('Portable'))
				{
					Write-Host ('Selected asset "' + $Asset.name + '".');
					$AssetToDownloadUrl = $Asset.browser_download_url;
					break;
				}
			}

			if (!($null -eq $AssetToDownloadUrl))
			{
				break;
			}
		}
	}
}
elseif ($Source -eq "appveyor")
{
	$UrlVersion = $Version;
	if ($UrlVersion.StartsWith("v")) 
	{
		$UrlVersion = $UrlVersion.Substring(1);
	}

	$UrlBase = "https://ci.appveyor.com/api";

	$BuildInfo = Invoke-RestMethod -Uri "$UrlBase/projects/gitextensions/gitextensions/build/$UrlVersion";
	$Job = $BuildInfo.build.jobs[0];
	if ($Job.Status -eq "success") 
	{
		$JobId = $Job.jobId;
		Write-Host ('Selected build job "' + $JobId + '".');

		$AssetsUrl = "$UrlBase/buildjobs/$JobId/artifacts";
		$Assets = Invoke-RestMethod -Method Get -Uri $AssetsUrl
		foreach ($Asset in $Assets)
		{
			if ($Asset.type -eq "zip" -and $Asset.FileName.Contains('Portable')) 
			{
				Write-Host ('Selected asset "' + $Asset.FileName + '".');
				$AssetToDownloadUrl = $AssetsUrl + "/" + $Asset.FileName;
				break;
			}
		}
	}
}

# Download and extract zip.
if (!($null -eq $AssetToDownloadUrl))
{
    $ExtractPath = $ExtractRootPath;

    if (!(Test-Path $ExtractRootPath))
    {
        New-Item -ItemType directory -Path $ExtractRootPath | Out-Null;
    }

    if (!(Test-Path $ExtractPath))
    {
        New-Item -ItemType directory -Path $ExtractPath | Out-Null;
    }

    Write-Host ('Downloading "' + $DownloadName + '" from URL "' + $AssetToDownloadUrl + '".');

    Invoke-WebRequest -Uri $AssetToDownloadUrl -OutFile $DownloadFilePath;
	Expand-Archive $DownloadFilePath -DestinationPath $ExtractPath -Force
	
	Write-Host ('Extraction at "' + $ExtractPath + '" completed.');
}
else
{
	Write-Host ('Download for version "' + $Version + '" not found.');
}

Pop-Location