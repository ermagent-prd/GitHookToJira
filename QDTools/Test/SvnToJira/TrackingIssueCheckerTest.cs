using JiraTools.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvnToJira.Engine;
using SvnToJira.Parameters;
using System;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class TrackingIssueCheckerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var checkedBranches = new List<ReleasesBranchInfo>();

            checkedBranches.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.30.0",
                    ReleaseName = "Ermas 5.30.0"
                }
            );

            string trackingIssue = "ERMAS-22694";

            //var engine = new TrackingIssueChecker();

            //var actual = engine.Execute(trackingIssue, checkedBranches);


        }
    }
}
