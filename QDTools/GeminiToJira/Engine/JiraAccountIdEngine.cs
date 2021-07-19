using Atlassian.Jira;
using GeminiTools.Service;
using JiraTools.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Engine
{
    public class JiraAccountIdEngine
    {
        private readonly List<string> userGroups = new List<string>() { "Administrators" };
        private readonly Lazy<Dictionary<string, JiraUser>> userListDictionary;

        public JiraAccountIdEngine(UserListGetter userListGetter)
        {
            this.userListDictionary = new Lazy<Dictionary<string, JiraUser>> (() => GetUsersDictionary(userListGetter));
        }

        //for test purpose
        public Dictionary<string, JiraUser> Execute()
        {
            return userListDictionary.Value;
        }

        public JiraUser Execute(string fullname, string defaultAccountname)
        {
            JiraUser userAccount;
            if (userListDictionary.Value.TryGetValue(fullname, out userAccount))
                return userAccount;

            if (string.IsNullOrWhiteSpace(defaultAccountname))
                return null;

            if (userListDictionary.Value.TryGetValue(defaultAccountname, out userAccount))
                return userAccount;

            return null;
        }


        private Dictionary<string, JiraUser> GetUsersDictionary(UserListGetter userListGetter)
        {
            Dictionary<string, JiraUser> result = new Dictionary<string, JiraUser>();
                        
            foreach (var group in userGroups)
            {
                var userList = userListGetter.Execute();      //returns all active users
                foreach (var user in userList)
                {
                    if(!result.TryGetValue(user.DisplayName, out JiraUser found))
                        result.Add(user.DisplayName, user);
                }
            }

            return result;
        }

        
    }
}
