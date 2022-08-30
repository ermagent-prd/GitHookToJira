using Atlassian.Jira;
using System.Collections.Generic;

namespace JiraTools.Engine
{
    public interface IProjectGetter
    {
        Project Execute(string projectKey);

        IEnumerable<Project> Execute();
    }
}