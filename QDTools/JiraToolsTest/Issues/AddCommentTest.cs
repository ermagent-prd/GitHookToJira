using JiraTools.Engine;
using JiraToolsTest.Container;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace JiraToolsTest
{
    [TestClass]
    public class AddCommentTest
    {

        [TestMethod]
        public void Execute_AddComment_returnEditedIssue()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<AddCommentEngine>();

            string issueKey = "ESJTP-14";

            string body = "Comment from SDK Call";

            string author = "gaetano.correnti@prometeia.com";

            var issue = engine.Execute(issueKey, author, body);


            Assert.IsNotNull(issue);
        }
       
    }
}
