using System;
using System.Collections.Generic;
using Atlassian.Jira;
using GeminiToJira.Engine.Common;
using JiraTools.Engine;

namespace GeminiToJira.Engine
{
    public class JiraAccountIdEngine
    {
        private readonly List<string> userGroups = new List<string>() { "Administrators" };
        private readonly Lazy<Dictionary<string, JiraUser>> userListDictionary;

        private readonly GeminiUserMapper userMapper;

        private readonly OriginalAccountLogger userLogger;

        public JiraAccountIdEngine(
            UserListGetter userListGetter,
            GeminiUserMapper userMapper,
            OriginalAccountLogger userLogger)
        {
            this.userMapper = userMapper;

            this.userLogger = userLogger;

            this.userListDictionary = new Lazy<Dictionary<string, JiraUser>>(() => GetUsersDictionary(userListGetter));
        }

        //for test purpose
        public Dictionary<string, JiraUser> Execute()
        {
            return userListDictionary.Value;
        }


        public JiraUser Execute(string fullName, string defaultAccountname)
        {
            var mappedUserName = this.userMapper.Execute(fullName);

            JiraUser userAccount;
            if (userListDictionary.Value.TryGetValue(mappedUserName, out userAccount))
                return userAccount;

            this.userLogger.AddLog(fullName);

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
                    if (!result.TryGetValue(user.DisplayName, out JiraUser found))
                        result.Add(user.DisplayName, user);
                }
            }

            return result;
        }

    }
}

