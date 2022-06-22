param (
    [string]$DockerImage = "piern/msbuildtools2022:w2016",
    [string]$DockerVolPath = "c:\gocd",
    [string]$BUILD_PROJECT_PATH = "source\CalendarTools\CalendarTools.sln",
    #[string]$BUILD_PROJECT_PATH = "buildProject\CalendarTools.proj",
    [string]$GO_PIPELINE_LABEL = "10.0.0.109390"
 )
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition 
$pipelinePath = (get-item $scriptPath ).parent.FullName

$PipelineSourcePath="$DockerVolPath\source"
$ProjectFilePath = $DockerVolPath+"\"+$BUILD_PROJECT_PATH
Write-Host "projectFilePath: $ProjectFilePath"
Write-Host "pipelinepath: $pipelinePath"
Write-Host "DockerImage: $DockerImage"
Write-Host "DockerVolPath: $DockerVolPath"
Write-Host "DockerPipelineSourcePath: $PipelineSourcePath"
Write-Host "DockerProjectFilePath: $ProjectFilePath"
Write-Host "BUILD_PROJECT_PATH: $BUILD_PROJECT_PATH"
Write-Host "GO_PIPELINE_LABEL: $GO_PIPELINE_LABEL"

$logPath = "${DockerVolPath}\logs\buildLogs.log"
$errorPath = "${DockerVolPath}\logs\buildErrors.log"
$warningPath = "${DockerVolPath}\logs\buildWarnings.log"

& docker run --rm `
    -v ${pipelinePath}:${DockerVolPath} `
    ${DockerImage} `
    "msbuild ""${ProjectFilePath}"" " `
    "-t:rebuild " `
    "-restore " `
    " -clp:Summary -clp:ErrorsOnly " `
    "-verbosity:minimal " `
    "-fl1 -flp1:logfile=${logPath} " `
    "-fl2 -flp2:logfile=${errorPath} -flp2:errorsonly " `
    "-fl3 -flp3:logfile=${warningPath} -flp3:warningsonly " `
    "/p:SVNRevision=${GO_PIPELINE_LABEL} " `
    "/p:SHORT_SOURCE_PATH=${PipelineSourcePath}" 


if ($LASTEXITCODE -eq 0)
{
    Write-Host ("Solution built successfully ")
    exit 0
}
else
{
    Write-Host ("Solution build -- > failure")
    exit 1
}

