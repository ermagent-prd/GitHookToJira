namespace PlanProcess
{
    internal class SinglePlanning
    {
        public string Entity { get; }

        public string Script { get; }

        public string Code { get; }

        public SinglePlanning(string entity, string code) : this(entity, null, code)
        { }

        public SinglePlanning(string entity, string script, string code)
        {
            Entity = entity;
            Script = script;
            Code = code;
        }
        
        public string GetId()
        {
            return $"[{Entity}].[{Code}]";
        }
    }
}
