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

        public JiraUser Execute(string userName)
        {
            var task = getUser(userName);

            task.Wait();

            return task.Result;
        }

        #endregion


        public async Task<JiraUser> getUser(string userName)
        {
            var jira = requestFactory.Service;

            return await jira.Users.GetUserAsync(userName);
        }

        public async Task<JiraUser> getMyself()
        {
            var jira = requestFactory.Service;

            return await jira.Users.GetMyselfAsync();
        }


    }
}
