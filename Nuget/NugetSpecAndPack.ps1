param (
    [string]$projectFolder= "C:\Projects\Ermas5\AlmProSuite\Source\ALMProCommon\CommonPatterns",
    [string]$projectFile = "Prometeia.ALMPro.CommonPatterns.csproj",
    [string]$OutputDirectory = "C:\Projects\Others\NugetWork\",
    [string]$PackageFile = "C:\Projects\Others\NugetWork\Prometeia.ALMPro.CommonPatterns.1.0.0-beta.nupkg",
    [string]$Token = "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",
    [string]$PackageRepo = "https://nuget.pkg.github.com/piern68/index.json"

 )


 Set-Location $projectFolder

 nuget spec $projectFile `
    -Force 


