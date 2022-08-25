namespace SvnToJira.Engine
{
    internal interface ISvnToJiraEngine
    {
        ActionResult Execute(EngineInput input);
    }
}