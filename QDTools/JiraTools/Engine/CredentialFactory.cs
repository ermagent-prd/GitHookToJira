using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTools.Engine
{
    public static class CredentialFactory
    {
        public static string Execute(string UserName, string Password)
        {
            string mergedCredentials = String.Format("{0}:{1}", UserName, Password);
            byte[] byteCredentials = Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }
    }
}
