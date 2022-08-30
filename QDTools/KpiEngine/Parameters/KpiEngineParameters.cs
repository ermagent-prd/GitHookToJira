using JiraTools.Parameters;
using SvnTools.Parameters;
using System.Collections.Generic;

namespace KpiEngine.Parameters
{
    internal class KpiEngineParameters
    {

        #region Public properties

        public SvnToolsParameters SvnParameters { get; set; }

        public JiraToolConfiguration JiraParameters { get; set; }

        public KpiCoreParameters KpiCoreParameters { get; set; }


        #endregion
    }
}
