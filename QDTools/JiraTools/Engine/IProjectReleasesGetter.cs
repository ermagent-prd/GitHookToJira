using Atlassian.Jira;
using System.Collections.Generic;

namespace JiraTools.Engine
{
    public interface IProjectReleasesGetter
    {
        IEnumerable<ProjectVersion> Execute(string projectKey);
    }
}