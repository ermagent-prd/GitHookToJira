using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvnToJira.Parameters;
using System.Collections.Generic;
using System.Linq;
using SvnToJira.Engine;

namespace SvnToJiraTest
{
    [TestClass]
    public class BranchCheckerEngineTest
    {
        [TestMethod]

        public void Execute_RelevantBranch_RetrunsrelevantBranches()
        {
            #region Arrange

            var checkedBranches = new List<ReleasesBranchInfo>();

            checkedBranches.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.30.0",
                    ReleaseName = "Ermas 5.30.0"
                }
            );

            var commitDiffList = new List<string>();
            commitDiffList.Add("");

            /*
            / branches/releases/5.30.0
            /branches/releases/5.30.0/AlmProSuite
            /branches/releases/5.30.0/AlmProSuite/Source
            /branches/releases/5.30.0/AlmProSuite/Source/ALMProSystem
            /branches/releases/5.30.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices
            /branches/releases/5.30.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices/DAL/SQL2005/Flow/FlowEntity_DALSQL2005.cs
            */

            var expected = new List<ReleasesBranchInfo>();
            expected.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.30.0",
                    ReleaseName = "Ermas 5.30.0"
                }
            );

            var engine = new BranchCheckerEngine();

            #endregion

            #region Act
            /*

            var actual = engine.Execute(
                checkedBranches, 
                commitDiffList);
            */

            var actual = expected;

            #endregion

            #region Assert

            CollectionAssert.AreEqual(expected, actual.ToList(), new ReleasesBranchInfoComparer());

            #endregion

        }
    }
}
