using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prometeia.AlmProTools.UnitTestHelpers;
using SvnToJira;
using SvnToJira.Engine;
using SvnToJira.Parameters;
using System.Collections.Generic;

namespace SvnToJiraTest
{
    [TestClass]
    public class TrackingIssueCheckerTest
    {
        [TestMethod]
        public void Execute_IssueIsNull_ReturnsMessage()
        {
            #region Arrange

            var trackingIssue = "ERMAS-TestIssue";

            TrackingIssueToCheckFields jiraIssue = null;

            List<ReleasesBranchInfo> checkedBranches = null;

            var engine = new TrackingIssuePropertiesChecker();

            var expected = new ActionResult(
                false,
                string.Format(Messages.JiraIssueNotFound, trackingIssue));

            #endregion

            #region Act

            var actual = engine.Execute(
                trackingIssue,
                jiraIssue,
                checkedBranches);

            #endregion

            #region Assert

            AssertGeneric.AreEqual(expected, actual);

            #endregion

        }

        [TestMethod]
        public void Execute_IsBug_ReturnsOk()
        {
            #region Arrange

            var trackingIssue = "ERMAS-TestIssue";

            var jiraIssue = new TrackingIssueToCheckFields(trackingIssue, "10001");

            List<ReleasesBranchInfo> checkedBranches = null;

            var engine = new TrackingIssuePropertiesChecker();

            var expected = ActionResult.Passed();

            #endregion

            #region Act

            var actual = engine.Execute(
                trackingIssue,
                jiraIssue,
                checkedBranches);

            #endregion

            #region Assert

            AssertGeneric.AreEqual(expected, actual);

            #endregion


        }

        [TestMethod]
        public void Execute_IsNotABug_ReturnsError()
        {
            #region Arrange

            var trackingIssue = "ERMAS-TestIssue";

            var jiraIssue = new TrackingIssueToCheckFields(trackingIssue, "XXXXX");

            List<ReleasesBranchInfo> checkedBranches = null;

            var engine = new TrackingIssuePropertiesChecker();

            var expected = new ActionResult(
                false,
                string.Format(Messages.JiraIssueNotABug, trackingIssue));

            #endregion

            #region Act

            var actual = engine.Execute(
                trackingIssue,
                jiraIssue,
                checkedBranches);

            #endregion

            #region Assert

            AssertGeneric.AreEqual(expected, actual);

            #endregion


        }

    }
}
