param (
    [string]$packagePath= "C:\Projects\Ermas5\AlmProSuite\Source\ALMProCommon\GeneralUtilities\Prometeia.ALMPro.GeneralUtilities.1.0.1.snupkg",
    [string]$source = "https://localhost:44396/nuget",
    [string]$apiKey = "hhfsadsdf-sdfsdf"
 )

 Set-Location $projectFolder

 nuget push $packagePath `
    -SymbolSource $source `
    -SymbolApiKey $apiKey `
    -SkipDuplicate





