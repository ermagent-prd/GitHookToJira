namespace SvnToJira.Engine
{
    internal interface ISvnToJiraEngine
    {
        void Execute(int svnCommit);
    }
}