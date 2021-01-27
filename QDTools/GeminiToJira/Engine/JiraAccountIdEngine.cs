using Atlassian.Jira;
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

        public string Execute(string fullname)
        {
            JiraUser userAccount;
            if (userListDictionary.Value.TryGetValue(fullname, out userAccount))
                return userAccount.AccountId;
            else
                return userListDictionary.Value.First().Value.AccountId;    //TODO è il deafault, da eliminare
        }


        private Dictionary<string, JiraUser> GetUsersDictionary(UserListGetter userListGetter)
        {
            Dictionary<string, JiraUser> result = new Dictionary<string, JiraUser>();
                        
            foreach (var group in userGroups)
            {
                var userList = userListGetter.Execute(group);      //TODO da fare per ogni gruppo 
                foreach (var user in userList)
                    result.Add(user.DisplayName, user);         //TODO solo se non già inserito
            }

            return result;
        }

        
    }
}
