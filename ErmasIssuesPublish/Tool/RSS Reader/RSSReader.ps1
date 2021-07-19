param (
    [string]$RSSUrl = "https://www.eba.europa.eu/news-press/news/rss.xml"
 )

 $Rss = Invoke-RestMethod -Uri $RSSUrl -

 Write-Host "File to update:" $Rss

