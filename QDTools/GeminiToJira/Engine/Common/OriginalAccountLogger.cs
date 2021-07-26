using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static GeminiToJira.Engine.JiraAccountIdEngine;

namespace GeminiToJira.Engine.Common
{
    public class OriginalAccountLogger
    {
        #region Private properties

        private readonly string userLogFile;

        private readonly List<string> log;

        private readonly HashSet<string> names;

        #endregion

        #region Constructor
        public OriginalAccountLogger(ConfigurationContainer config)
        {
            this.userLogFile = 
                config.Configuration.LogDirectory + 
                "UserLog_" + 
                config.Configuration.JiraProjectCode + "_" + 
                DateTime.Now.ToString("yyyyMMdd-hh_mm") + ".log";

            this.names = new HashSet<string>();

            this.log = new List<string>();
        }

        #endregion

        #region Public methods

        public void AddLog(string userName)
        {
            if (this.names.Contains(userName))
                return;
            
            this.log.Add(userName);

            this.names.Add(userName);

        }

        public void SaveLog()
        {
            using (TextWriter tw = new StreamWriter(this.userLogFile))
            {
                foreach (String s in this.log)
                    tw.WriteLine(s);
            }
        }

        #endregion

        #region Private methods


        #endregion
    }
}
