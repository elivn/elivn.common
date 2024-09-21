# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$slnPath = Join-Path $packFolder "../"
$srcPath = Join-Path $slnPath "Plugs/"

# List of projects
$projects = (
    "Kasca.OrmPlug.SqlServer"
)

# Rebuild solution
Set-Location $slnPath
& dotnet restore

# Copy all nuget packages to the pack folder
foreach($project in $projects) {
    
    $projectFolder = Join-Path $srcPath $project

    # Create nuget pack
    Set-Location $projectFolder
	$projectBinFolder = Join-Path $projectFolder "bin/Release"
    $isExistsBinFolder = Test-Path $projectBinFolder
	if($isExistsBinFolder -eq $True){
		Remove-Item -Recurse $projectBinFolder
	}
    & dotnet msbuild /p:Configuration=Release /p:SourceLinkCreate=true
    & dotnet msbuild /t:pack /p:Configuration=Release /p:SourceLinkCreate=true

    # Copy nuget package
    $projectPackPath = Join-Path $projectBinFolder ($project + ".*.nupkg")
    Move-Item $projectPackPath $packFolder
}

# Go back to the pack folder
Set-Location $packFolder

Write-Host ""
Write-Host "是否发布至：http://ai2.kasijia.com/nugut ?"
Write-Host ""
$user_input = Read-Host '请输入 y 或 n '
if ($user_input -eq 'y') {
    foreach ($packfile in Get-ChildItem -Path $packFolder -Recurse -Include *.nupkg) {
        ..\tools\nuget\nuget.exe push $packfile 123456 -Source http://ai2.kasijia.com/nugut/nuget
    }
    pause
}
del *.nupkg
exit