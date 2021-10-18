param (
    [string]$msbuildpath= "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe",
    [string]$projectFolder= "C:\Projects\Ermas5\AlmProSuite\Source\ALMProCommon\GeneralUtilities",
    [string]$projectFile = "Prometeia.ALMPro.GeneralUtilities.csproj",
    [string]$author = "Prometeia spa",
    [string]$Description = "general utilities"
 )

 Set-Location $projectFolder

 & $msbuildpath $projectFile -t:pack /p:IncludeSymbols=true /p:SymbolPackageFormat=snupkg
