param (
    [string]$OutFolder = "C:\Projects\Others\Processtools\Bin\QDTools\FaqEBA\",
    [int]$DaysLeg = 7,
    [string]$FileNamePrefix = "FAQEBA"
 )
    try {

        $today = Get-Date

        $previousDay = $today.AddDays(-$DaysLeg)


        $firstDate = "{0}%2F{1}%2F{2}" -f $previousDay.Day, $previousDay.Month, $previousDay.Year

        $lastDate = "{0}%2F{1}%2F{2}" -f $today.Day, $today.Month, $today.Year


        $url = "https://www.eba.europa.eu/single-rule-book-qa/search?field_isrb_q_a_review_resp=All&field_qa_pub_as_final%5Bdate%5D={0}&field_qa_pub_as_final_1%5Bdate%5D={1}&items_per_page=20&export_all_pdf_qa" `
        -f $firstDate, $lastDate

        $outFileName = "{0}_{1}_TO_{2}.pdf" -f $FileNamePrefix, $previousday.ToString("yyyy-MM-dd"), $today.ToString("yyyy-MM-dd")

        $outPdfPath = $OutFolder + $outFileName

        Invoke-WebRequest -Uri $url -OutFile $outPdfPath

        $messageSubject = “EBA Weekly updates ({0} - {1})” -f $previousday.ToString("yyyy/MM/dd"), $today.ToString("yyyy/MM/dd")

        

        $mailCredential = Get-Credential -UserName helpdesk_almpro - 

        Send-MailMessage 
            -To “pierluigi.nanni@prometeia.com” `
            -From “FAQ EBA”  `
            -Attachments $outPdfPath `
            -Subject $messageSubject `
            -Body “See the Attached pdf report” `
            -Credential (Get-Credential) `
            -SmtpServer “192.168.0.7” `
            -Port 25

    }
    Catch {
        Write-Error "Processing failed  $_.Exception.Message"
        Break
    }