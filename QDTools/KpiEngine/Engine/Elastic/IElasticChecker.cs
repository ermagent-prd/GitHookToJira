namespace KpiEngine.Engine.Elastic
{
    internal interface IElasticChecker
    {
        bool Execute(string uniqueKey);
    }
}