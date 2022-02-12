param (
    [string]$oldRepo = "http://erm-codever-p01.prometeia.lan/svn/ErmasNet/branches/releases/5.22.0",
    [string]$newRepo = "http://erm-codever-p01.prometeia.lan/svn/ErmasNet/branches/releases/5.27.0",
    [string]$logPath = "c:\tmp\Ermas 522-572-svnDiff.txt"
 )

 & svn diff --old $oldRepo --new $newRepo --summarize > $logPath