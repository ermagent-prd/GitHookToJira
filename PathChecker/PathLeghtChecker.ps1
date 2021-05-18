param (
    [string]$pathToScan = "C:\Projects\Ermas5\AlmProSuite\Source",  # The path to scan and the the lengths for (sub-directories will be scanned as well).
    [string]$outputFilePath = "C:\Projects\Others\Processtools\PathChecker\PathLegthChecker.txt", # This must be a file in a directory that exists and does not require admin rights to write to.
    [bool]$writeToConsoleAsWell = $false,   # Writing to the console will be much slower.
    [int]$threshold = 249
 )


# Open a new file stream (nice and fast) and write all the paths and their lengths to it.
$outputFileDirectory = Split-Path $outputFilePath -Parent
if (!(Test-Path $outputFileDirectory)) { New-Item $outputFileDirectory -ItemType Directory }

try {
    $stream = New-Object System.IO.StreamWriter($outputFilePath, $false)
    $stream.WriteLine("Lenght : Path")
    Get-ChildItem -Path $pathToScan -Recurse -Force | Select-Object -Property FullName, @{Name="FullNameLength";Expression={($_.FullName.Length)}} | Sort-Object -Property FullNameLength -Descending | ForEach-Object {
    
        if ( $_.FullNameLength -le $threshold) { continue }
        
        $filePath = $_.FullName
        $length = $_.FullNameLength
        $string = "$length : $filePath"
    
        # Write to the Console.
        if ($writeToConsoleAsWell) { Write-Host $string }
    
        #Write to the file.
        $stream.WriteLine($string)
    }    
}
finally {
    $stream.Close()    
}
