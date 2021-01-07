using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Parameters;
using JiraTools.Service;

namespace JiraTools.Engine
{
    internal class UserGetter
    {
        #region Private properties

        private readonly ServiceManagerContainer requestFactory;

        private readonly IJiraToolsParameters parameters;

        #endregion

        #region Constructor

        public UserGetter(
            ServiceManagerContainer requestFactory,
            IJiraToolsParameters parameters)
        {
            this.requestFactory = requestFactory;

            this.parameters = parameters;
        }

        #endregion

        #region Public methods

        public IEnumerable<JiraUser> Execute()
        {
            var task = getUsers();

            task.Wait();

            return task.Result;
        }

        #endregion


        public async Task<IEnumerable<JiraUser>> getUsers()
        {
            var jira = requestFactory.Service;


            // use LINQ syntax to retrieve issues
            return await jira.Users.SearchUsersAsync("Pierluigi Nanni");
        }

    }
}
