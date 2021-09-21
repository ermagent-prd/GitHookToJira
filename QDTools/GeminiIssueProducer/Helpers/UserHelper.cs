using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeminiIssueProducer.Helpers
{
    internal class UserHelper
    {
        private readonly ServiceManager serviceManager;

        public UserHelper(ServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        public List<int> GetIds(IEnumerable<string> userEmails)
        {
            try
            {
                var result =
                    new List<int>();

                List<UserDto> activeUsers =
                    serviceManager.User.GetUsers();

                if (activeUsers == null)
                    return null;

                foreach (string userMail in userEmails)
                {
                    string inLoopUserMail =
                        userMail;

                    UserDto foundUser =
                        activeUsers.Where(
                        (u) =>
                        u.Entity.Email == inLoopUserMail)
                        .FirstOrDefault();

                    if(foundUser!= null)
                        result.Add(foundUser.Entity.Id);
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void  RemoveFollower(int issueId, int userId)
        {
            try
            {
                serviceManager.Item.IssueWatchersDelete(issueId, userId);
            }
            catch (Exception) { }
        }
    }
}
