using Atlassian.Jira;
using JiraTools.Parameters;
using JiraTools.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JiraTools.Engine
{
    public class UserListGetter
    {
        #region Private properties

        private readonly ServiceManagerContainer requestFactory;

        private readonly IJiraToolsParameters parameters;

        #endregion

        #region Constructor

        public UserListGetter(
            ServiceManagerContainer requestFactory,
            IJiraToolsParameters parameters)
        {
            this.requestFactory = requestFactory;

            this.parameters = parameters;
        }

        #endregion

        #region Public methods

        public IEnumerable<JiraUser> Execute(string groupName)
        {
            var task = getUsers(groupName);

            task.Wait();

            return task.Result;
        }

        #endregion


        public async Task<IEnumerable<JiraUser>> getUsers(string groupName)
        {
            var jira = requestFactory.Service;
            
            var users = await jira.Groups.GetUsersAsync(groupName);

            return from u in users select u;
        }

        public async Task<JiraUser> getMyself()
        {
            var jira = requestFactory.Service;

            return await jira.Users.GetMyselfAsync();
        }
    }
}
