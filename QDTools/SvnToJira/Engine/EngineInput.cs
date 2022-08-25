using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnToJira.Engine
{
    internal class EngineInput
    {
        public EngineInput(int svnRevision, string repoFolder, string svnTransaction)
        {
            SvnRevision = svnRevision;
            RepoFolder = repoFolder;
            SvnTransaction = svnTransaction;
        }

        public int SvnRevision { get; }

        public string RepoFolder { get; }

        public string SvnTransaction { get; }
    }
}
