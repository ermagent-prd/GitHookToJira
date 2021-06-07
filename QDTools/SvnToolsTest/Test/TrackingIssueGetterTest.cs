using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvnTools.Engine;

namespace SvnToolsTest
{
    [TestClass]
    public class TrackingIssueGetterTest
    {
        [TestMethod]
        public void Execute_SingleTrackId_returnTrackId()
        {
            const string pattern = @"((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)";

            var engine = new TrackingIssueGetter();

            string log = "Defect fix JIRA:ESJTP-14} e poi anche la issue ESJTP-34 e questo sdfsdf-567";

            var result = engine.Execute(
                pattern,
                log);

            var actual = result.ToList();

            var expected = new List<string>() { "ESJTP-14" , "ESJTP-34" };

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
