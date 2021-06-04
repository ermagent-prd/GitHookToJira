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

        #endregion

        #region Constructor

        public UserListGetter(
            ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }

        #endregion

        #region Public methods

        public IEnumerable<JiraUser> Execute(string groupName)
        {
            var task = getUsers(groupName);

            task.Wait();

            return task.Result;
        }

        public IEnumerable<JiraUser> Execute()
        {
            var task = getUsers();

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

        public async Task<IEnumerable<JiraUser>> getUsers()
        {
            var jira = requestFactory.Service;

            List<JiraUser> result = new List<JiraUser>();

            int startIndex = 0;

            while(true)
            {
                var users = await jira.Users.SearchUsersAsync("%", JiraUserStatus.Active, 50, startIndex);

                if (!users.Any())
                    break;

                result.AddRange(users);
                startIndex += 50;
            }
            
            return from u in result select u;
        }

        public async Task<JiraUser> getMyself()
        {
            var jira = requestFactory.Service;

            return await jira.Users.GetMyselfAsync();
        }
    }
}
