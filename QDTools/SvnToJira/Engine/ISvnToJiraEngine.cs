namespace SvnToJira.Engine
{
    internal interface ISvnToJiraEngine
    {
        ActionResult Execute(int svnCommit);
    }
}