param (
[string]$SendEmailsTo = "pierluigi.nanni@prometeia.com",
[string]$SVNUser = "piern",
[string]$SVNPass = "******",
[string]$SVNbranchRoot = "http://erm-codever-p01.prometeia.lan/svn/ErmasNet/branches/releases/5.30.0",
[string]$SvnPath = "C:\Program Files\TortoiseSVN\bin\svn.exe",
[string]$ExcelFilePath = "C:\Tmp\Ermas530Logs.xlsx",
[string]$sheetName = "Ermas530Commit",
[string]$revisionFrom = "157734",
[string]$revisionTo = "157808"
)

Function GetSVNLogs($revRange)
{

([xml](&$SvnPath log $SVNbranchRoot -r $revRange --limit 500 --xml --username $SVNUser --password $SVNPass)).log.logentry | % {
    $entry = $_;
    $_.paths.path | Foreach-Object {
      $obj = 1 | Select-Object -Property Revision,Author,Date,Message,Action,FilePath;
      $obj.Revision = [int]$entry.Revision;
      $obj.Author = $entry.Author;
      $obj.Date = $entry.Date;
      $obj.Message = $entry.msg;
      $obj.Action = $_.action;
      $obj.FilePath = $_.InnerText;
      return $obj;
    }
  }
 }


Import-Module PSExcel
 
#get svn logs since the last release
Try {
    
$RevisionRange = "${revisionFrom}:${revisionTo}"; 

$SVNChanges = GetSVNLogs $RevisionRange

    $SVNChanges | Select-Object  Revision, Author, Date, Message  | Export-XLSX -WorksheetName $sheetName -Path $ExcelFilePath -Table -Force -Autofit `
    -Header  Revision, Author, Date, Message
}

Catch {

    Write-Host ("File update -- > failure " + $_.Exception.Message )
    exit 1
}        

