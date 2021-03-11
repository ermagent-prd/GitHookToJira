using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Parameters.Import
{
    public enum ImportCfgType
    {
        /// <summary>
        /// Test configuration
        /// </summary>
        Test = 0,

        /// <summary>
        /// ERM
        /// </summary>
        ERM = 1,

        /// <summary>
        /// SSSP
        /// </summary>
        SSSP = 2,

        /// <summary>
        /// RMS5
        /// </summary>
        RMS5 = 3,
    }
}
