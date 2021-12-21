param (
    [string]$projectFolder= "C:\Projects\Ermas5\AlmProSuite\Source\ALMProCommon\CommonPatterns",
    [string]$projectFile = "Prometeia.ALMPro.CommonPatterns.csproj",
    [string]$version = "1.0.0",
    [string]$suffix = "beta",
    [string]$author = "piern",
    [string]$Description = "SHL Common Patterns",
    [string]$PackageDirectory = "C:\Projects\Others\NugetWork\"
 )

 Set-Location $projectFolder

 nuget pack $projectFile `
    -Symbols `
    -SymbolPackageFormat snupkg `
    -Version $version `
    -Suffix $suffix `
    -PackagesDirectory $PackageDirectory `
    -properties "author=$author;description=$Description" `
    -Force 



