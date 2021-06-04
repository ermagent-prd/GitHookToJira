using JiraTools.Engine;
using JiraToolsTest.Container;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace JiraToolsTest
{
    [TestClass]
    public class EditCustomFieldTest
    {

        [TestMethod]
        public void Execute_EditDescription_returnEditedIssue()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<EditCustomFieldEngine>();

            string issueKey = "ESJTP-14";

            string fieldName = "JDE Module";

            string fieldValue = "From SDK Call";

            var issue = engine.Execute(issueKey, fieldName, fieldValue);


            Assert.IsNotNull(issue);
        }
       
    }
}
