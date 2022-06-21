param (
[string]$SendEmailsTo = "pierluigi.nanni@prometeia.com",
[string]$SVNUser = "piern",
[string]$SVNbranchRoot = "http://erm-codever-p01.prometeia.lan/svn/ErmasNet/branches/releases/5.30.0",
[string]$SvnPath = "C:\Program Files\TortoiseSVN\bin\svn.exe",
[string]$ExcelFilePath = "C:\Tmp\Ermas530Logs.xlsx",
[string]$sheetName = "Ermas530Commit",
[string]$revisionFrom = "157734",
[string]$revisionTo = "HEAD"
)

Function GetSVNLogs($revRange)
{

$securePass = Read-Host "What is your svn password for user ${SVNUser} ?" -AsSecureString

$pass = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePass))

$jiraRegex = "JIRA:\w+-\d+"

([xml](&$SvnPath log $SVNbranchRoot -r $revRange --limit 500 --xml --username $SVNUser --password $pass)).log.logentry | % {
    $entry = $_;
    $_.paths.path | Foreach-Object {
      $obj = 1 | Select-Object -Property Revision,Author,Date,Message,Action,FilePath,JiraRef;
      $obj.Revision = [int]$entry.Revision;
      $obj.Author = $entry.Author;
      $obj.Date = $entry.Date;
      $obj.Message = $entry.msg;
      $obj.Action = $_.action;
      $obj.FilePath = $_.InnerText;
      $obj.JiraRef = [regex]::matches($_.InnerText, $jiraRegex).Value
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

