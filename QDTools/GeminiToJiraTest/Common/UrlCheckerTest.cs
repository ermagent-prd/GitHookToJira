using System;
using GeminiToJira.Engine.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeminiToJiraTest
{
    [TestClass]
    public class UrlCheckerTest
    {
        [DataTestMethod]
        [DataRow("https://www.google.it/", true)]
        [DataRow("http://www.google.it/", true)]
        [DataRow("https:\\www.google.it/", false)]
        public void IsValid_ValidUrl_ReturnsTrue(string url, bool expected)
        {

            #region Act

            var engine = new URLChecker();

            var actual = engine.IsValid(url);

            #endregion

            #region Assert

            Assert.AreEqual(expected, actual);

            #endregion
        }
    }
}
